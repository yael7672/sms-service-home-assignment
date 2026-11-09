# SMS Service Home Assignment

This solution implements a small SMS message service with ASP.NET Core 8, Entity Framework Core and SQLite.

## What was built

- `POST /messages` validates, stores and sends a message, then returns its internal identifier.
- `GET /messages/{id}` returns a message and its current status.
- `GET /messages?status=Sent` returns all messages, optionally filtered by status.
- SQLite persistence, created automatically on first run.
- A provider abstraction with both an in-process mock and an HTTP provider adapter.
- Automated tests for provider success, provider failure, status filtering and input validation.
- Swagger UI for manually exercising the API.

## Technology choices

- **ASP.NET Core 8**: a small, strongly typed HTTP API with built-in validation and dependency injection.
- **Entity Framework Core with SQLite**: persistent local storage without an external database installation.
- **xUnit**: focused tests for the critical message flow.
- **Provider abstraction**: `ISmsProvider` keeps business logic independent of a specific SMS vendor.

## Run locally

Prerequisite: .NET 8 SDK.

From the repository root, run:

```bash
dotnet run --project src/SmsService.Api
```

Open the Swagger URL printed in the terminal, normally:

```text
https://localhost:7124/swagger
```

The SQLite file `sms.db` is created automatically. No migration or database setup command is required for this exercise.

Run the automated tests with:

```bash
dotnet test
```

## Example requests

Create a message:

```http
POST /messages
Content-Type: application/json

{
  "to": "0501234567",
  "body": "Test message"
}
```

Get the message using the returned id:

```http
GET /messages/{id}
```

Get only failed messages:

```http
GET /messages?status=Failed
```

Statuses are `Pending`, `Sent` and `Failed`.

## SMS provider configuration

The solution defaults to `MockSmsProvider`, so it runs without another process:

```json
"SmsProvider": {
  "UseMockProvider": true,
  "BaseUrl": "http://localhost:5055/"
}
```

The exact contract and executable for the external mock SMS provider were not included with the assignment materials. Therefore, the solution uses `MockSmsProvider` by default so it can run locally in one command. `ExternalSmsProvider` demonstrates how an HTTP-based provider can be integrated once its request and response contract is available.

## Assumptions

- The public contract in the assignment accepts `to` and `body`; employee lookup is therefore outside this service's responsibility.
- An Israeli mobile number is accepted in the `05XXXXXXXX` format.
- A message contains 1 to 500 characters.
- A provider acceptance is represented as `Sent`; it does not mean the handset received the message.
- The API stores a message as `Pending` before contacting the provider, so an attempted send remains visible even if the provider fails.
- Authentication and deployment are intentionally excluded as requested.

## Decisions and trade-offs

- The API currently sends to the provider synchronously. This keeps the exercise small and makes the behavior easy to follow. For campaign volume, the send operation should move to a durable queue and background workers.
- `Database.EnsureCreated()` was chosen instead of EF migrations so a reviewer can run the project in one command. A production service would use versioned migrations.
- Provider failures are persisted as a `Failed` message. The creation endpoint still returns the internal id, which allows the caller to inspect the failed attempt.
- No repository layer was added over EF Core. For this small service, `DbContext` already provides the required persistence abstraction, and an additional repository would add little value.

## Not implemented

- The optional UI, because the assignment states that it is not scored and backend correctness was prioritized.
- Authentication, deployment and employee management, because they are outside the requested scope.
- Background processing, retries and idempotency, because they require product decisions and infrastructure beyond the four-hour scope.
- Delivery receipts/webhooks, because the supplied provider contract is required before implementing them correctly.

## What I would add with more time

- Match the HTTP adapter exactly to the supplied provider and add integration tests using a stub HTTP server.
- Add an idempotency key to prevent duplicate sends.
- Add a queue, workers, retry policy and dead-letter handling.
- Add pagination to `GET /messages`.
- Add structured logs, metrics and alerts.
- Add EF Core migrations and containerized local execution.

## AI usage

AI was used to accelerate the initial project structure, generate repetitive boilerplate, suggest test scenarios, and assist with the wording of the documentation.

I reviewed the generated code, made the architectural decisions, adapted the implementation to the assignment requirements, and manually verified the API and automated tests. The complete specification used with AI is included in `SPEC.md`.

## Time spent

Approximately 4 hours of focused work, including implementation, automated tests, manual API verification, and documentation.