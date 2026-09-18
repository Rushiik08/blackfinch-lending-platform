# Blackfinch Lending Platform

A full-stack simulation of a secured lending platform developed for the **Blackfinch Engineering Candidate Technical Test – Full Stack, September 2026**.

The application allows users to submit a secured loan application and receive an instant lending decision based on the applicant's loan amount, secured asset value, and credit score.

The platform also provides statistics about submitted applications, including successful and declined applicants, total loan value, and average Loan-to-Value (LTV).

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

- Full name
- Email address
- Phone number
- Loan amount
- Property / asset value
- Credit score

The backend calculates the Loan-to-Value (LTV) and applies the required lending rules.

The system then returns:

- Whether the application is **Successful** or **Declined**
- The calculated LTV
- Relevant decision information

The platform also maintains application data and provides overall statistics.

The technical test requires an API and web frontend that simulate a basic lending platform. The supplied specification identifies loan amount, secured asset value, and credit score as the core lending inputs. :contentReference[oaicite:0]{index=0} :contentReference[oaicite:1]{index=1}

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

After submitting the application, the backend:

1. Validates the input.
2. Calculates the LTV.
3. Applies the lending rules.
4. Determines whether the application is successful or declined.
5. Stores the application.
6. Returns the result to the frontend.

---

## Platform Statistics

The application displays:

- Successful applicants
- Declined applicants
- Total applicants
- Total value of loans written
- Mean average LTV

These correspond to the outputs specified in the technical test. :contentReference[oaicite:2]{index=2}

---

## Input Validation

The application validates:

- Required fields
- Email address
- Phone number
- Loan amount
- Property / asset value
- Credit score
- Valid numeric values

The credit score must be within the specified range of **1–999**. :contentReference[oaicite:3]{index=3}

---

## REST API

The React frontend communicates with the ASP.NET Core Web API.

The API is responsible for:

- Receiving applications
- Validating data
- Applying lending rules
- Persisting applications
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
- .NET test framework

## Development Tools

- Visual Studio Code
- Git
- GitHub
- .NET CLI
- npm

---

# Architecture

The application is divided into separate responsibilities.

```text
                    ┌─────────────────────┐
                    │    React Frontend   │
                    │                     │
                    │ Application Form    │
                    │ Result              │
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
