# Blackfinch Lending Platform

Blackfinch Lending Platform is a full-stack secured-loan application created for the **Blackfinch Engineering Candidate Technical Test — Full Stack, September 2026**.

An applicant enters their contact information and secured-loan details in the React application. The ASP.NET Core API validates the request, applies the lending rules, stores the application in SQLite, and returns an immediate `Successful` or `Declined` decision. The frontend displays the decision and the current platform statistics.

The backend is the single source of truth. Lending rules are implemented in the domain project and are not duplicated in the controller or used to make decisions in the frontend.

## Main features

- Applicant form for full name, email, phone number, loan amount, property value, and credit score
- Client-side feedback for common input errors
- Server-side validation for every request
- Instant secured-lending decision
- LTV calculation and decline explanation
- SQLite persistence for all submitted applications
- Platform metrics updated after every submission
- Complete application history visible in the UI, with accepted and declined decisions
- Swagger documentation in Development
- Automated xUnit tests for rules, boundaries, and validation
- CORS configuration for the local Vite frontend

## Technology

| Area | Technology |
| --- | --- |
| Backend API | ASP.NET Core 8 Web API |
| Lending domain | .NET class library with plain C# rules |
| Persistence | Entity Framework Core with SQLite |
| Frontend | React and Vite |
| Tests | xUnit |
| API documentation | Swagger / OpenAPI |

SQLite was chosen so the project can run without a separate database server. The lending domain is independent of Entity Framework and can be moved to another relational provider later.

## Repository structure

```text
backend/
  Blackfinch.Lending.Domain/
    LoanDecisionService.cs       Lending rules and LTV calculation
    LoanDecision.cs               Decision value returned by the domain
    LoanDecisionStatus.cs         Successful or Declined status
  Blackfinch.Lending.Api/
    Contracts/                    HTTP request and response models
    Controllers/                  API endpoints
    Data/                         EF Core context and database entity
    Services/                     Application submission and metrics
    Validation/                   Contact and financial input validation
    Program.cs                    Dependency injection, SQLite, CORS, Swagger
tests/
  Blackfinch.Lending.Domain.Tests/
    LoanDecisionServiceTests.cs
    ApplicationRequestValidatorTests.cs
frontend/
  src/
    App.jsx                       Form state, submission, and metrics loading
    ApplicationForm.jsx           Applicant input form
    DecisionResult.jsx            Decision and application details
    MetricsPanel.jsx              Platform statistics
    api.js                        API client
```

## Requirements

Install:

- .NET 8 SDK
- Node.js and npm

No SQL Server, LocalDB, Docker, or separate database installation is required.

## Run locally

### 1. Start the backend

From the repository root:

```bash
dotnet run --project backend/Blackfinch.Lending.Api --launch-profile http
```

The API runs at `http://localhost:5157`. In Development, Swagger UI is available at `http://localhost:5157/swagger`.

On first startup, Entity Framework creates the `LoanApplications` table in the SQLite database configured by `ConnectionStrings:Lending` in `backend/Blackfinch.Lending.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Lending": "Data Source=lending.db"
  }
}
```

The database path is relative to the API process working directory. To clear all applications and metrics, stop the API and delete the generated `lending.db` file.

### 2. Start the frontend

In a second terminal:

```bash
cd frontend
npm install
npm run dev
```

Open `http://localhost:5173`. Vite proxies `/api` requests to the backend. The API also allows the Vite preview origin `http://localhost:4173`.

### 3. Run tests

From the repository root:

```bash
dotnet test backend/Blackfinch.Lending.sln
```

The tests cover:

- Minimum and maximum loan amounts
- The £1 million large-loan threshold
- LTV boundaries at 60%, 80%, and 90%
- Credit-score thresholds of 750, 800, 900, and 950
- Invalid financial inputs
- Required and malformed applicant contact details

The frontend can be production-built with:

```bash
cd frontend
npm run build
```

## Application flow

1. The frontend loads existing platform metrics from `GET /api/applications/metrics`.
2. The user completes the application form.
3. The frontend performs immediate field validation and prevents submission when input is clearly invalid.
4. The frontend sends the normalized form values to `POST /api/applications`.
5. The API validates contact and financial fields again.
6. `LoanDecisionService` calculates LTV and applies the lending rules.
7. The API stores both successful and declined applications.
8. The API returns the application result and refreshed metrics.
9. The frontend displays the result and updates the statistics panel.

Client-side validation improves usability but is not trusted for correctness; all important validation is repeated on the server.

## API reference

### `POST /api/applications`

Submit an application.

Request:

```json
{
  "fullName": "Rahul Patil",
  "email": "rahul@example.com",
  "phoneNumber": "9876543210",
  "loanAmount": 200000,
  "assetValue": 400000,
  "creditScore": 750
}
```

For valid input, the API returns `200 OK`:

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "fullName": "Rahul Patil",
  "email": "rahul@example.com",
  "phoneNumber": "9876543210",
  "loanAmount": 200000,
  "assetValue": 400000,
  "creditScore": 750,
  "ltvPercent": 50,
  "decision": "Successful",
  "declineReason": null,
  "metrics": {}
}
```

An application that fails a lending rule is still valid input and returns `200 OK` with `"decision": "Declined"` and a `declineReason`. It is persisted and included in the metrics.

Malformed or invalid input returns `400 Bad Request`:

```json
{
  "errors": [
    "Email is required.",
    "Asset value must be greater than zero so LTV can be calculated."
  ]
}
```

### `GET /api/applications/metrics`

Returns:

```json
{
  "successfulApplicants": 1,
  "declinedApplicants": 2,
  "totalApplicants": 3,
  "totalValueOfLoansWritten": 200000,
  "meanAverageLtv": 68.3333
}
```

`totalValueOfLoansWritten` includes successful loans only. `meanAverageLtv` includes successful and declined applications. When there are no applications, all counters and values are zero.

### `GET /api/applications/history`

Returns every submitted application, newest first. Each history item includes the applicant name, loan details, LTV, credit score, decision, optional decline reason, and submission time. Both successful and declined applications are included.

## Validation rules

The API rejects the request with `400 Bad Request` when:

- Full name is missing or longer than 100 characters
- Email is missing, malformed, or longer than 254 characters
- Phone number is missing or invalid
- Loan amount is not greater than zero
- Asset value is not greater than zero
- Credit score is not a whole number from 1 through 999

Phone numbers may contain digits, spaces, hyphens, parentheses, and an optional leading `+`. The number must contain between 7 and 15 digits and cannot consist only of zeroes.

Amounts are rounded to two decimal places before the lending decision and persistence. Invalid input is different from a lending decline: invalid input is not stored, while a valid application that fails a lending rule is stored as `Declined`.

## Lending rules

LTV is calculated as:

```text
LTV = (loan amount / asset value) × 100
```

The domain applies the following rules:

1. Loans below £100,000 are declined.
2. Loans above £1,500,000 are declined.
3. Loans of £1,000,000 or more require:
   - LTV of 60% or less
   - Credit score of at least 950
4. Loans below £1,000,000 use these exclusive LTV bands:
   - Below 60%: credit score at least 750
   - 60% to below 80%: credit score at least 800
   - 80% to below 90%: credit score at least 900
   - 90% or more: declined

The exclusive bands resolve the overlapping LTV wording in the brief and make each credit threshold reachable.

## Data model

Each application is stored in the `LoanApplications` SQLite table with:

- `Id`
- `FullName`
- `Email`
- `PhoneNumber`
- `LoanAmount`
- `AssetValue`
- `CreditScore`
- `LtvPercent`
- `IsSuccessful`
- `DeclineReason`
- `CreatedAtUtc`

The API creates the schema automatically on startup. It also checks older SQLite databases and adds the applicant contact columns when necessary.

## Architecture

- **Domain:** `LoanDecisionService` owns the lending rules and can be tested without HTTP or a database.
- **API contracts:** Separate request and response models define the public HTTP shape.
- **Validation:** `ApplicationRequestValidator` handles request-level errors before the domain is called.
- **Application service:** `LoanApplicationService` normalizes values, invokes the domain, persists the result, and calculates metrics.
- **Controller:** `ApplicationsController` exposes only the two required endpoints and remains thin.
- **Frontend:** React manages form state and presentation; it does not decide whether a loan should be approved.
- **History:** The frontend loads persisted history on startup and refreshes it after each submission.

## Deliberate scope

This project focuses on the requirements of the technical test. It does not include authentication, applicant accounts, loan products, payments, an application-history endpoint, background queues, or a separate reporting service.

For production, likely next steps would include authentication and authorization, a managed relational database with migrations and backups, idempotency and concurrency controls, structured logging and monitoring, versioned lending rules with an audit trail, and paginated application history.
