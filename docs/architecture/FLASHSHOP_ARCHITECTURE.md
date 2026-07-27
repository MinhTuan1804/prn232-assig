# FlashShop Architecture

## 1. Architecture Overview

FlashShop is a .NET 8 microservices backend for an e-commerce and flash-sale platform. The system combines an API Gateway, independently deployable business services, synchronous service-to-service calls, asynchronous domain events, background jobs, and logically isolated databases.

The primary architectural styles are:

- Microservices organized around business capabilities.
- API Gateway as the public backend entry point.
- Layered organization inside each service.
- REST/JSON for external clients.
- gRPC for synchronous internal operations.
- RabbitMQ and MassTransit for asynchronous workflows.
- Database-per-service at the logical database level.
- Hangfire for recurring and background processing.

This document describes the architecture represented by the source code and deployment configuration in this repository.

## 2. System Context and Actors

FlashShop serves three business actors:

- Guest: browses categories, products, testimonials, and flash-sale campaigns; registers and signs in.
- Customer: manages a profile and FlashPay wallet, maintains a cart, checks out, pays for orders, and reviews order history.
- Administrator: manages users, roles, categories, products, campaigns, stock, orders, and notification templates.

The web frontend or another HTTP API client is the access channel. It sends REST requests to the API Gateway and includes a JWT for protected operations. Gmail SMTP is an external delivery system for transactional emails and administrative reports. Product and testimonial records may reference images hosted by Unsplash.

SQL Server, RabbitMQ, and Hangfire are supporting runtime infrastructure rather than business actors.

## 3. Service Boundaries

The backend is divided into five business services:

| Service | Primary responsibilities |
| --- | --- |
| Identity | Authentication, JWT issuance, profiles, roles, account status, wallets, and wallet transactions |
| Catalog | Categories, products, product images, product discovery, testimonials, and flash-sale campaigns |
| Ordering | Shopping carts, checkout, order state, payment initiation, cancellation, and order history |
| Inventory | Available stock, reserved stock, reservations, release/confirmation, and stock history |
| Notification | Notification templates, delivery logs, transactional email, and scheduled reports |

Each service has a dedicated startup entry point, controllers, application services, entity model, Entity Framework Core context, migrations, and Dockerfile. Shared code is limited to cross-cutting primitives and communication contracts.

## 4. API Gateway and Routing

The YARP-based API Gateway is the intended public backend entry point. It maps resource-oriented URL prefixes to internal services:

- Authentication, users, and wallets route to Identity.
- Categories, products, testimonials, and flash sales route to Catalog.
- Stock routes to Inventory.
- Cart and order routes to Ordering.
- Notification routes to Notification.

The Gateway validates JWT issuer, audience, signature, and token lifetime. It also provides cross-origin configuration, aggregated Swagger endpoints, and a health endpoint that checks the availability of all five services.

Keeping routing at the Gateway prevents clients from depending on internal container names and gives the backend a central location for public access policies.

## 5. Data Ownership

Each service owns a separate Entity Framework Core context and logical SQL Server database:

| Database | Owning service | Core data |
| --- | --- | --- |
| FlashShop_IdentityDb | Identity | ASP.NET Identity records, wallets, and wallet transactions |
| FlashShop_CatalogDb | Catalog | Categories, products, images, campaigns, and campaign items |
| FlashShop_InventoryDb | Inventory | Stock, reservations, and stock history |
| FlashShop_OrderingDb | Ordering | Carts, orders, and order items |
| FlashShop_NotificationDb | Notification | Templates, delivery logs, and Hangfire state |

The databases currently share one SQL Server container for development convenience, but their schemas and migrations are owned by separate services. Cross-service business operations use APIs or messages rather than direct table access.

## 6. Synchronous Communication

External clients communicate with the backend through REST and JSON. This keeps the public API compatible with browsers, Swagger, Postman, and mobile clients.

Ordering uses gRPC for internal operations that require an immediate result:

- WalletGrpc.PayWithWallet asks Identity to deduct a customer's FlashPay balance and returns success or failure.
- WalletGrpc.GetWalletBalance reads the current balance.
- CatalogGrpc.DeductStock updates catalog product and flash-sale quantities.

The Protobuf contracts live in the shared MessageContracts project, while Identity and Catalog host the gRPC servers. Ordering receives generated strongly typed clients through dependency injection. HTTP/2 endpoints are separated from the HTTP/1 REST endpoints in the container configuration.

## 7. Asynchronous Event Workflow

RabbitMQ and MassTransit decouple order processing from inventory and notification side effects. Ordering publishes order lifecycle events, and consumers react independently.

The principal messages are OrderCreated, OrderPaid, OrderCancelled, InventoryReserved, and InventoryReservationFailed.

For a deferred-payment order, the workflow is:

1. Ordering stores the order and publishes OrderCreated.
2. Inventory consumes the event and attempts a serializable stock reservation.
3. Inventory publishes either InventoryReserved or InventoryReservationFailed.
4. Ordering updates the order state from the reservation result.
5. Notification consumes the same result and sends the appropriate email.

This publish-subscribe design allows additional consumers to be introduced without adding direct dependencies to Ordering.

## 8. Security Model

Identity uses ASP.NET Core Identity to store users, hash passwords, enforce password rules, and manage roles. Successful authentication produces a signed JWT containing the user identifier, email, display name, and role claims.

Two business roles are defined:

- Customer for authenticated shopping operations.
- Admin for user, catalog, campaign, stock, and notification administration.

The Gateway and individual services both configure JWT validation. Public catalog and authentication endpoints remain anonymous, while personal profiles, wallets, carts, and orders require authentication. Administrative mutations use role-based authorization attributes.

For production, only the Gateway should be publicly reachable, credentials should be supplied through a secret manager, CORS should allow known frontend origins, and operational dashboards should require administrator authentication.

## 9. Deployment and Operations

Docker Compose provisions SQL Server, RabbitMQ, the five business services, and the API Gateway on a shared bridge network. Health checks ensure SQL Server and RabbitMQ are available before dependent services start.

Ordering and Notification use Hangfire with SQL Server storage. Ordering runs a recurring timeout job every minute to cancel orders that exceeded their payment deadline. Notification runs a daily sales-report job at 23:59 and sends the result through the configured SMTP provider.

Each service applies its own Entity Framework Core migrations at startup. Identity, Catalog, and Notification also seed the initial roles, users, product catalog, notification templates, and sample content required by the application.

The Gateway exposes an aggregated health endpoint and Swagger documentation for all backend APIs. RabbitMQ additionally exposes its management interface for local operational inspection.

## 10. Architectural Risks and Evolution

The current implementation demonstrates the intended distributed architecture, but several areas should be strengthened before production use:

- Inventory and Catalog both hold stock-related values; Inventory should become the authoritative stock owner.
- Wallet payment can complete before the order transaction is durable; a saga or compensating refund is required.
- Database writes and event publication are separate operations; an outbox/inbox pattern would prevent lost or duplicated messages.
- Event consumers should implement explicit idempotency using message or aggregate identifiers.
- Order administration endpoints must enforce the Admin role rather than authentication alone.
- Notification test endpoints and Hangfire dashboards must not remain anonymously accessible.
- Internal service ports should not be publicly published in production.
- Distributed tracing, correlation identifiers, centralized logs, metrics, and health probes should be added for operational diagnosis.

These improvements preserve the existing service boundaries while making cross-service workflows safer and more observable.
