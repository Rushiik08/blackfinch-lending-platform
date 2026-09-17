# AI log — Blackfinch Full Stack Technical Test (September 2026)

This log records the actual AI-assisted process for this submission. It is not a reconstructed fiction of a solo implementation.

## Session 1 — Requirements analysis (no implementation)

**Prompt (summary):** Analyse `Blackfinch Engineering Candidate Tests - Fullstack.pdf` before writing code. Identify functional requirements, business rules, inputs/outputs, APIs, database design, frontend, validation, edge cases, assumptions, and a small-test architecture.

**What the AI produced:** A full requirements breakdown, including a proposed two-endpoint API, a single `LoanApplications` table, and a domain-service-first architecture.

**Output that was questioned:** The brief lists overlapping LTV conditions for loans under £1 million (`LTV < 60%`, `< 80%`, `< 90%`). A literal nested reading would make the 800 and 900 thresholds unreachable for some bands, or apply the wrong score.

**Correction / decision:** Treat them as mutually exclusive bands:

- LTV &lt; 60% → credit ≥ 750
- 60% ≤ LTV &lt; 80% → credit ≥ 800
- 80% ≤ LTV &lt; 90% → credit ≥ 900
- LTV ≥ 90% → decline

This matches the original console challenge’s intent and was written into `LoanDecisionService.DecideStandardLoan` with a comment. Boundary tests lock that interpretation.

**Other analysis decisions kept:**

- “Loans written” = sum of successful loan amounts only
- Mean LTV = all applications
- Frontend must not decide
- Do not add auth, CQRS, or microservices

## Session 2 — Implementation

**Prompt (summary):** Implement the September 2026 full-stack test with C# and React. Keep it simple. Domain logic not in the controller or UI. Tests for boundaries. README + AI_LOG. Propose structure first, then build in stages.

**Structure proposed and used:**

```
backend/Domain + Api
tests/
frontend/
README.md
AI_LOG.md
```

**What the AI generated:** Solution scaffolding, `LoanDecisionService`, xUnit tests, ASP.NET controller/service/EF model, React form + metrics UI, README.

**Iterations and review:**

1. **Database:** The earlier analysis assumed SQL Server. This machine has no LocalDB. Using Docker SQL Server would add setup the brief does not need. **Correction:** SQLite via EF Core. Documented as a trade-off, not hidden.
2. **HTTPS redirection:** The default Web API template redirects to HTTPS, which breaks a local Vite proxy on HTTP. **Correction:** Removed `UseHttpsRedirection`; HTTP-only launch profile on port 5157.
3. **WeatherForecast template:** Deleted leftover sample controller/files so the repo only contains lending code.
4. **Validation vs rules:** Domain throws `ArgumentOutOfRangeException` for invalid input; the API validator maps that class of error to 400 *before* calling Decide. Rule failures still persist as Declined.
5. **Metrics query:** Loaded the small table in memory and aggregated in the service. Fine for this test; a SQL `GROUP BY` would be the production version.
6. **Frontend:** Used the Vite React template, then replaced the demo page with three components (`ApplicationForm`, `DecisionResult`, `MetricsPanel`) and a proxy to `/api`.

**How output was verified:**

- `dotnet test` — 24 domain tests covering the brief’s boundary list
- `dotnet run` + HTTP POST/GET against the API
- `npm run build` for the frontend
- Browser check of submit → decision → metrics update

**Design decisions influenced by AI, then accepted or rejected:**

| Suggestion | Outcome |
|---|---|
| Separate Domain project | Accepted — keeps rules testable |
| Two API endpoints | Accepted |
| Optional application history list | Rejected — not in the brief |
| FluentValidation package | Rejected — a small validator class is enough |
| Exclusive LTV bands | Accepted after explicit review of the overlapping wording |

## Prompts worth noting

The useful prompts were specification-heavy: paste the brief, forbid extra features, require tests for named boundaries, and require an honest AI log. Vague “build a lending app” prompts would have invited auth, dashboards, and extra entities that the test does not ask for.
