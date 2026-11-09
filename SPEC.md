# Development Specification Used With AI

Build a concise home-assignment solution for an SMS message service.

## Required stack

- ASP.NET Core Web API targeting .NET 8.
- Entity Framework Core with SQLite.
- xUnit tests.
- Swagger for manual API verification.
- The application must run locally without external infrastructure.

## Required API

### POST messages

Accept JSON fields `to` and `body`. Validate an Israeli mobile number in the format `05XXXXXXXX` and a non-empty body of no more than 500 characters. Persist the message before calling an SMS provider. Return HTTP 201 with the internal message id.

### GET messages by id

Return the stored message and status for a valid id. Return HTTP 404 when it does not exist.

### GET messages

Return messages ordered from newest to oldest. Accept an optional `status` query parameter.

## Data model

Store an internal GUID, recipient phone number, body, status, provider message id, failure reason, creation time and last update time. Use the statuses `Pending`, `Sent` and `Failed`.

## Architecture

- Keep controllers thin.
- Put orchestration in an injectable message service.
- Put provider integration behind an `ISmsProvider` interface.
- Include an in-process mock provider so the project runs alone.
- Include a configurable HTTP adapter that can be adjusted to the exact supplied mock-provider contract.
- Do not add a repository abstraction over EF Core for this small scope.

## Tests

Add focused automated tests for:

- Successful provider send persists `Sent` and the provider id.
- Provider failure persists `Failed` and the failure reason.
- Status filtering returns only matching rows.
- Invalid phone formats fail validation.
- A body longer than 500 characters fails validation.

## Documentation

Create a README explaining how to run and test the solution, decisions, assumptions, exclusions, future improvements, AI usage and actual time spent. Keep the solution small enough to explain and modify during an interview.
