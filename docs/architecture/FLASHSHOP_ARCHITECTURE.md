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
