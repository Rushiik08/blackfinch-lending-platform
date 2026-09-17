# Blackfinch Lending Platform

A small full-stack simulation of a secured lending platform, built for the **Blackfinch Engineering Candidate Technical Test — Full Stack, September 2026**.

The API is the source of truth for lending decisions. The React UI only collects inputs and displays results.

## Technology stack

- C# / ASP.NET Core 8 Web API
- Domain class library for lending rules
- Entity Framework Core with **SQLite**
- React (Vite)
- xUnit tests for business logic

SQL Server LocalDB was not available on the development machine. SQLite is a relational store that needs no extra server, which keeps the test easy to run. Switching the EF provider to SQL Server would not change the domain model.

## Project structure

```
backend/Blackfinch.Lending.Domain   Lending rules (no HTTP, no EF)
backend/Blackfinch.Lending.Api      REST API, validation, persistence, metrics
tests/Blackfinch.Lending.Domain.Tests
frontend/                    React application
README.md
AI_LOG.md
```

## How to run the backend

From the repository root:

```bash
dotnet run --project backend/Blackfinch.Lending.Api --launch-profile http
```

The API listens on **http://localhost:5157**. Swagger is at `/swagger` in Development.

A SQLite file `lending.db` is created next to the API on first run (`EnsureCreated`).

## How to run the frontend

In a second terminal:

```bash
cd frontend
npm install
npm run dev
```

Open **http://localhost:5173**. Vite proxies `/api` to the backend.

## Database setup

No separate database install is required.

- Provider: SQLite
- Connection string: `ConnectionStrings:Lending` in `backend/Blackfinch.Lending.Api/appsettings.json`
- Default: `Data Source=lending.db`
- Schema: one `LoanApplications` table

To reset statistics, stop the API and delete `lending.db`.

## How to run tests

```bash
dotnet test backend/Blackfinch.Lending.sln
```

The tests cover amount gates, the £1 million split, LTV and credit boundaries, LTV ≥ 90%, and invalid input.

## API endpoints

### `POST /api/applications`

Request:

```json
{
  "loanAmount": 200000,
  "assetValue": 400000,
  "creditScore": 750
}
```

Response includes the decision (`Successful` or `Declined`), LTV, optional decline reason, stored amounts, and updated platform metrics.

Invalid input returns **400** with an `errors` array. A valid application that fails lending rules returns **200** with `"decision": "Declined"`.

### `GET /api/applications/metrics`

Returns:

- `successfulApplicants`
- `declinedApplicants`
- `totalApplicants`
- `totalValueOfLoansWritten` (sum of **successful** loan amounts)
- `meanAverageLtv` (mean LTV of **all** applications; `0` when none exist)

## Business rules

LTV = (loan amount / asset value) × 100.

1. Decline if loan amount &lt; £100,000 or &gt; £1.5 million.
2. If loan amount ≥ £1,000,000: LTV must be ≤ 60% and credit score ≥ 950.
3. If loan amount &lt; £1,000,000:
   - LTV ≥ 90% → decline
   - LTV &lt; 60% → credit ≥ 750
   - 60% ≤ LTV &lt; 80% → credit ≥ 800
   - 80% ≤ LTV &lt; 90% → credit ≥ 900

The brief lists overlapping “LTV less than 60/80/90” conditions. They are implemented as **exclusive bands** so each credit threshold can apply. That is documented in `LoanDecisionService` and in `AI_LOG.md`.

## Assumptions

- The overlapping LTV rules are exclusive bands, evaluated in order.
- “Loans written” means approved applications only.
- Mean LTV includes declined applications.
- No authentication, applicant names, or loan products.
- Credit score 1–999 is validation; failing a credit *threshold* is a decline.
- Asset value ≤ 0 is invalid because LTV is undefined.
- Empty platform mean LTV is `0`.

## Design decisions

- **Domain project** owns the decision. Controllers do not contain lending if/else trees. The frontend never decides.
- **Validation vs decline:** malformed input is 400; rule failure is a stored Declined application.
- **Thin API:** one table, two endpoints, SQLite, no CQRS/auth/queues.
- **Decline reasons** are stored so the UI can explain the outcome; they are not required by the brief.

## Production improvements (not in this test)

- Authentication and authorisation
- SQL Server (or similar) with migrations and backups
- Idempotency keys and concurrency control
- Structured logging and metrics
- Versioned lending rules with an audit trail
- Pagination of application history

## AI usage summary

AI assistance was used to analyse the brief, propose a small architecture, draft domain/API/React code, and write this README. The LTV band interpretation, endpoint set, and SQLite choice were reviewed and kept deliberately small. See `AI_LOG.md` for prompts, iterations, and corrections.
