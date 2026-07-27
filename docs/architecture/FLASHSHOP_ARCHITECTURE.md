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
