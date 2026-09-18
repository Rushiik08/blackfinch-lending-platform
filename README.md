# Blackfinch Lending Platform

A full-stack simulation of a secured lending platform developed for the **Blackfinch Engineering Candidate Technical Test – Full Stack, September 2026**.

The application allows users to submit a secured loan application and receive an instant lending decision based on the applicant's loan amount, secured asset value, and credit score.

The platform also provides statistics about submitted applications, including successful and declined applicants, total value of loans written, and mean average Loan-to-Value (LTV).

---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Business Rules](#business-rules)
- [Loan-to-Value Calculation](#loan-to-value-calculation)
- [Application Flow](#application-flow)
- [Database](#database)
- [API](#api)
- [Frontend](#frontend)
- [Validation](#validation)
- [How to Run](#how-to-run)
- [Testing](#testing)
- [Example Test Cases](#example-test-cases)
- [Statistics](#statistics)
- [Design Decisions](#design-decisions)
- [Assumptions](#assumptions)
- [Error Handling](#error-handling)
- [AI-Assisted Development](#ai-assisted-development)
- [Production Considerations](#production-considerations)
- [Technical Test Requirements](#technical-test-requirements)
- [Submission](#submission)

---

# Overview

The Blackfinch Lending Platform is a small full-stack application that simulates the decision-making process for secured lending.

A user provides:

- Full Name
- Email Address
- Phone Number
- Loan Amount
- Property / Asset Value
- Credit Score

The backend calculates the Loan-to-Value (LTV) and applies the required lending rules.

The system then returns:

- Whether the application is **Successful** or **Declined**
- The calculated LTV
- Relevant decision information

The platform also stores application data and provides overall lending statistics.

The core lending inputs required by the technical test are:

- Loan amount in GBP
- Asset value securing the loan
- Applicant credit score

---

# Features

## Applicant Application

Users can submit a new secured loan application with:

- Full Name
- Email Address
- Phone Number
- Loan Amount in GBP
- Property / Asset Value in GBP
- Credit Score

---

## Instant Lending Decision

After submitting an application, the backend:

1. Validates the input.
2. Calculates the LTV.
3. Applies the lending rules.
4. Determines whether the application is successful or declined.
5. Stores the application.
6. Returns the result to the frontend.

The lending decision is handled by the backend rather than the React frontend.

---

## Platform Statistics

The application displays:

- Successful applicants
- Declined applicants
- Total applicants
- Total value of loans written
- Mean average LTV

These statistics are calculated from the applications stored in the database.

---

## Input Validation

The application validates:

- Required fields
- Email address
- Phone number
- Loan amount
- Property / asset value
- Credit score
- Numeric values

The credit score must be a whole number between **1 and 999**.

---

## REST API

The React frontend communicates with the ASP.NET Core Web API.

The API is responsible for:

- Receiving applications
- Validating input
- Applying lending rules
- Persisting application data
- Calculating statistics
- Returning responses to the frontend

---

# Technology Stack

## Backend

- **C#**
- **ASP.NET Core 8 Web API**
- **Entity Framework Core**
- **SQLite**
- **Swagger / OpenAPI**

## Frontend

- **React**
- **Vite**
- **JavaScript**
- **CSS**

## Testing

- **xUnit**
- **.NET test framework**

## Development Tools

- Visual Studio Code
- Git
- GitHub
- .NET CLI
- npm

---

# Architecture

The application separates presentation, API, domain logic, and persistence responsibilities.

```text
                    ┌─────────────────────┐
                    │    React Frontend   │
                    │                     │
                    │ Application Form    │
                    │ Decision Result     │
                    │ Statistics          │
                    └──────────┬──────────┘
                               │
                               │ HTTP / REST API
                               ▼
                    ┌─────────────────────┐
                    │ ASP.NET Core API    │
                    │                     │
                    │ Controllers         │
                    │ Validation          │
                    │ Services            │
                    │ Persistence         │
                    └──────────┬──────────┘
                               │
                               ▼
                    ┌─────────────────────┐
                    │ Domain Layer        │
                    │                     │
                    │ Lending Rules       │
                    │ LTV Calculation     │
                    │ Decision Logic      │
                    └──────────┬──────────┘
                               │
                               ▼
                    ┌─────────────────────┐
                    │ SQLite Database     │
                    │                     │
                    │ Applications        │
                    └─────────────────────┘
```

### Separation of Responsibilities

### React Frontend

Responsible for:

- Displaying the application form
- Collecting user input
- Calling the API
- Displaying the lending decision
- Displaying platform statistics

The frontend does not contain the core lending decision rules.

### ASP.NET Core API

Responsible for:

- HTTP requests and responses
- Request validation
- API contracts
- Persistence
- Connecting the frontend with the domain layer

### Domain Layer

Responsible for:

- Lending rules
- LTV calculation
- Lending decision logic

This keeps the core business logic independent from the HTTP and UI layers.

---

# Project Structure

```text
Blackfinch-Lending-Platform/
│
├── backend/
│   │
│   ├── Blackfinch.Lending.Domain/
│   │   ├── Entities/
│   │   ├── Services/
│   │   └── ...
│   │
│   ├── Blackfinch.Lending.Api/
│   │   ├── Contracts/
│   │   ├── Controllers/
│   │   ├── Data/
│   │   ├── Properties/
│   │   ├── Services/
│   │   ├── Validation/
│   │   ├── Program.cs
│   │   └── appsettings.json
│   │
│   └── Blackfinch.Lending.sln
│
├── frontend/
│   ├── src/
│   │   ├── assets/
│   │   ├── api.js
│   │   ├── App.jsx
│   │   ├── ApplicationForm.jsx
│   │   ├── DecisionResult.jsx
│   │   ├── MetricsPanel.jsx
│   │   ├── index.css
│   │   └── main.jsx
│   │
│   ├── public/
│   ├── package.json
│   ├── vite.config.js
│   └── index.html
│
├── tests/
│   └── Blackfinch.Lending.Domain.Tests/
│
├── README.md
└── .gitignore
```

---

# Business Rules

The lending decision follows the business rules provided in the technical test.

## General Loan Limits

An application is declined if:

```text
Loan Amount < £100,000
```

or:

```text
Loan Amount > £1,500,000
```

---

## Loans of £1 Million or More

If:

```text
Loan Amount >= £1,000,000
```

both of the following conditions must be satisfied:

```text
LTV <= 60%
```

and:

```text
Credit Score >= 950
```

If either condition is not satisfied, the application is declined.

---

## Loans Below £1 Million

If:

```text
Loan Amount < £1,000,000
```

the applicable credit-score requirement depends on the LTV.

| LTV                  | Required Credit Score |
| -------------------- | --------------------: |
| Less than 60%        |                >= 750 |
| 60% to less than 80% |                >= 800 |
| 80% to less than 90% |                >= 900 |
| 90% or more          |              Declined |

The overlapping wording in the supplied brief was interpreted as mutually exclusive LTV bands so that each application falls into one applicable band.

---

# Loan-to-Value Calculation

Loan-to-Value (LTV) represents the loan amount as a percentage of the secured asset value.

The formula is:

```text
LTV = (Loan Amount / Asset Value) × 100
```

### Example

```text
Loan Amount = £500,000

Asset Value = £1,000,000

LTV = (500,000 / 1,000,000) × 100

LTV = 50%
```

---

# Application Flow

```text
User enters application details
             │
             ▼
       Frontend validation
             │
             ▼
       Submit application
             │
             ▼
       ASP.NET Core API
             │
             ▼
        Validate input
             │
             ▼
       Calculate LTV
             │
             ▼
      Apply lending rules
             │
             ▼
   ┌─────────┴─────────┐
   │                   │
   ▼                   ▼
Successful          Declined
   │                   │
   └─────────┬─────────┘
             │
             ▼
      Save application
             │
             ▼
       Return result
             │
             ▼
       React displays
             │
             ▼
       Update statistics
```

---

# Database

The application uses **SQLite** with **Entity Framework Core**.

SQLite was selected for this technical-test implementation because it:

- Requires no separate database server
- Is simple to configure
- Supports relational data
- Works with Entity Framework Core
- Makes local setup straightforward

The application creates and uses a local SQLite database file:

```text
lending.db
```

No separate SQL Server or PostgreSQL installation is required for local development.

---

# API

The backend is implemented using **ASP.NET Core 8 Web API**.

The API handles:

```text
Request
   ↓
Validation
   ↓
Business Logic
   ↓
Database
   ↓
Response
```

The API is responsible for processing applications and providing the platform statistics used by the frontend.

---

## Swagger

When running in development mode, Swagger can be accessed at:

```text
http://localhost:5157/swagger
```

Swagger provides an interface for inspecting and testing the API.

---

# Frontend

The frontend is implemented using **React and Vite**.

The application provides a simple interface for submitting lending applications.

## New Application

The form collects:

- Full Name
- Email Address
- Phone Number
- Loan Amount
- Property Value
- Credit Score

---

## Application Result

After submitting an application, the result section displays the lending decision returned by the backend.

The backend is responsible for determining whether the application is successful or declined.

---

## Platform Statistics

The statistics section displays:

- Successful applicants
- Declined applicants
- Total applicants
- Total value of loans written
- Mean average LTV

---

# Validation

Validation is performed before an application is processed.

Important validation rules include:

## Loan Amount

The loan amount must be within the supported range:

```text
Minimum: £100,000
Maximum: £1,500,000
```

## Credit Score

The credit score must be:

```text
1 to 999
```

and must be a whole number.

## Property Value

The property / asset value must be provided and must be valid for calculating LTV.

## Contact Information

The application validates:

- Name
- Email
- Phone number

before processing the application.

---

# How to Run

## Prerequisites

Install the following:

- .NET 8 SDK
- Node.js
- npm
- Visual Studio Code

Verify the installations:

```bash
dotnet --version
```

```bash
node --version
```

```bash
npm --version
```

---

# Step 1 - Clone the Repository

Clone the public GitHub repository:

```bash
git clone <YOUR_GITHUB_REPOSITORY_URL>
```

Navigate into the project:

```bash
cd Blackfinch-Lending-Platform
```

---

# Step 2 - Run the Backend

Open a terminal at the project root.

Run:

```bash
dotnet run --project backend/Blackfinch.Lending.Api --launch-profile http
```

The API will run at:

```text
http://localhost:5157
```

Swagger will be available at:

```text
http://localhost:5157/swagger
```

The SQLite database is initialized by the backend.

---

# Step 3 - Run the Frontend

Open a **second terminal**.

Navigate to the frontend:

```bash
cd frontend
```

Install the required npm packages:

```bash
npm install
```

Start the React development server:

```bash
npm run dev
```

Vite will start the frontend at:

```text
http://localhost:5173
```

---

# Step 4 - Open the Application

Open the following URL in your browser:

```text
http://localhost:5173
```

The React frontend will communicate with the ASP.NET Core backend.

---

# Running Both Applications

The backend and frontend run as two separate processes.

## Terminal 1 - Backend

From the project root:

```bash
dotnet run --project backend/Blackfinch.Lending.Api --launch-profile http
```

## Terminal 2 - Frontend

```bash
cd frontend
npm install
npm run dev
```

Then open:

```text
http://localhost:5173
```

---

# Testing

Automated tests are included for the lending domain logic.

Run all tests from the project root:

```bash
dotnet test backend/Blackfinch.Lending.sln
```

You can also run the domain test project directly:

```bash
dotnet test tests/Blackfinch.Lending.Domain.Tests
```

The tests focus on important lending-rule scenarios and boundary conditions.

---

# Example Test Cases

The following examples can be used to manually test the application.

---

## Test Case 1 - Loan Below Minimum

### Input

```text
Loan Amount: £50,000
Property Value: £100,000
Credit Score: 800
```

### Expected Result

```text
Declined
```

Reason:

The loan amount is below the minimum supported amount of £100,000.

---

## Test Case 2 - Loan Above Maximum

### Input

```text
Loan Amount: £2,000,000
Property Value: £3,000,000
Credit Score: 999
```

### Expected Result

```text
Declined
```

Reason:

The loan amount is above the maximum of £1,500,000.

---

## Test Case 3 - Low LTV Application

### Input

```text
Loan Amount: £500,000
Property Value: £1,000,000
Credit Score: 800
```

### LTV

```text
LTV = (500,000 / 1,000,000) × 100

LTV = 50%
```

### Expected Result

```text
Successful
```

The LTV is below 60% and the credit score satisfies the applicable threshold.

---

## Test Case 4 - High LTV Application

### Input

```text
Loan Amount: £500,000
Property Value: £500,000
Credit Score: 999
```

### LTV

```text
LTV = (500,000 / 500,000) × 100

LTV = 100%
```

### Expected Result

```text
Declined
```

An LTV of 90% or more is declined for loans below £1 million.

---

## Test Case 5 - £1 Million Loan

### Input

```text
Loan Amount: £1,000,000
Property Value: £2,000,000
Credit Score: 950
```

### LTV

```text
LTV = (1,000,000 / 2,000,000) × 100

LTV = 50%
```

### Expected Result

```text
Successful
```

The LTV is within 60% and the credit score meets the 950 requirement.

---

## Test Case 6 - £1 Million Loan With Insufficient Credit Score

### Input

```text
Loan Amount: £1,000,000
Property Value: £2,000,000
Credit Score: 900
```

### LTV

```text
LTV = 50%
```

### Expected Result

```text
Declined
```

The credit score is below the required 950 for loans of £1 million or more.

---

# Statistics

The platform calculates statistics from submitted applications.

## Successful Applicants

The number of applications that received a successful lending decision.

---

## Declined Applicants

The number of applications that received a declined lending decision.

---

## Total Applicants

The total number of submitted applications:

```text
Successful Applicants + Declined Applicants
```

---

## Total Value of Loans Written

The total value of loans from successful applications.

---

## Mean Average LTV

The mean LTV across all submitted applications.

---

# Design Decisions

## 1. Domain Logic Is Separate From Controllers

The core lending rules are implemented in the domain layer instead of directly inside API controllers.

This keeps the controllers focused on handling HTTP requests and responses.

---

## 2. Backend Is the Source of Truth

The lending decision is calculated by the backend.

The React frontend collects information and displays the result but does not independently determine whether an application should be approved or declined.

---

## 3. SQLite Database

SQLite was selected for the technical-test implementation because it provides relational persistence without requiring a separate database server.

---

## 4. Simple Architecture

The project uses a simple layered structure rather than introducing unnecessary architectural complexity.

The implementation does not add CQRS, microservices, or other patterns that are not required for the scope of this test.

---

## 5. LTV Bands

The technical test contains overlapping wording for the LTV conditions below £1 million.

The implementation interprets these as mutually exclusive bands:

```text
LTV < 60%
60% <= LTV < 80%
80% <= LTV < 90%
LTV >= 90%
```

This ensures that exactly one lending rule applies to an application.

---

## 6. Application Data

Applicant contact details are stored as part of the application because they are part of the application flow implemented in the frontend.

---

# Assumptions

The following assumptions were made during implementation:

1. SQLite is suitable for the technical-test implementation.
2. The backend is the source of truth for lending decisions.
3. The frontend is responsible for presentation and user interaction.
4. Authentication is not implemented because it is not specified as a requirement in the supplied technical test.
5. The lending decision is calculated immediately after application submission.
6. Applications are stored locally in SQLite.
7. The LTV conditions for loans below £1 million are interpreted as mutually exclusive bands.
8. "Loans written" refers to successful applications.
9. Mean LTV is calculated across all applications.
10. The application is intended as a technical-test implementation and not as a production lending system.

---

# Error Handling

The application validates input before processing the lending decision.

Examples of invalid input include:

- Missing required fields
- Invalid email address
- Invalid phone number
- Invalid loan amount
- Invalid property value
- Invalid credit score
- Invalid numeric input

Invalid requests are rejected rather than being processed as lending decisions.

A valid application can still receive a **Declined** lending decision when it does not satisfy the lending business rules.

This keeps input validation separate from the lending decision.

---

# AI-Assisted Development

AI tools were used during the development process.

The AI assistance was used for activities including:

- Understanding the technical-test requirements
- Analysing the lending business rules
- Discussing application architecture
- Generating implementation ideas
- Debugging development issues
- Reviewing code structure
- Suggesting validation approaches
- Creating test scenarios
- Reviewing design decisions
- Improving documentation

AI-generated suggestions were reviewed against the actual requirements and implementation.

Important decisions and corrections were made after reviewing the AI output, including:

- Interpreting the overlapping LTV conditions as mutually exclusive bands
- Keeping the lending decision in the domain layer
- Separating validation from lending decisions
- Using SQLite for simple local development
- Avoiding unnecessary architectural complexity

A detailed AI log containing the key prompts, iterations, corrections, questioned output, and verification process is submitted separately as the required PDF document.

---

# Production Considerations

The application is designed as a technical-test implementation rather than a production lending system.

For a production version, I would consider the following improvements.

## Security

- Authentication
- Authorization
- HTTPS
- Secure secret management
- API security
- Protection of personal and financial information

## Database

- Production relational database
- Database backups
- High availability
- Migration management
- Monitoring

## Testing

- Additional unit tests
- Integration tests
- API tests
- End-to-end tests
- Load testing

## Monitoring

- Structured logging
- Application monitoring
- Health checks
- Error tracking
- Performance monitoring

## Deployment

- CI/CD pipeline
- Containerization
- Environment-specific configuration
- Automated deployments
- Production monitoring

## Additional Lending Features

A production system could also include:

- User authentication
- Application history
- Application status tracking
- Document upload
- Underwriter workflows
- Audit trails
- Notifications
- Reporting
- User roles

These features are outside the scope of the technical test.

---

# Technical Test Requirements

This project was developed for the **Blackfinch Engineering Candidate Technical Test - Full Stack, September 2026**.

The technical test asks for an API and web frontend that simulate a basic lending platform.

The assessment focuses on:

- Correctness of business logic
- Clarity and maintainability of code
- Separation of concerns
- Modularity
- Effective use of AI
- Quality of prompting
- Iteration and critical review of AI output
- Ability to explain reasoning, assumptions, and trade-offs

The required lending inputs are:

- Loan amount
- Asset value
- Applicant credit score

The required outputs include:

- Loan decision
- Total number of applicants grouped by success status
- Total value of loans written
- Mean average LTV

---

# Submission

The project is submitted as a public Git repository.

The repository contains:

- Backend source code
- Frontend source code
- Domain logic
- Automated tests
- README documentation
- `.gitignore`

The README provides instructions for running the backend and frontend locally.

The detailed AI log is provided separately as a PDF document through the submission form.

---

# Local URLs

When running the application locally:

## Frontend

```text
http://localhost:5173
```

## Backend API

```text
http://localhost:5157
```

## Swagger

```text
http://localhost:5157/swagger
```

---

# Quick Start

## Terminal 1 - Backend

From the project root:

```bash
dotnet run --project backend/Blackfinch.Lending.Api --launch-profile http
```

## Terminal 2 - Frontend

```bash
cd frontend
npm install
npm run dev
```

Open the application:

```text
http://localhost:5173
```

---

# Author

**Rushikesh Kurukale**

Blackfinch Full Stack Engineering Technical Test

**September 2026**
