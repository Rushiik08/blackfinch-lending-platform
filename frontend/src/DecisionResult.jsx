function money(value) {
  return new Intl.NumberFormat("en-GB", {
    style: "currency",
    currency: "GBP",
  }).format(value);
}

export default function DecisionResult({ result, error }) {
  if (error) {
    return (
      <section className="panel">
        <h2>Application result</h2>
        <p className="error">{error}</p>
      </section>
    );
  }

  if (!result) {
    return (
      <section className="panel">
        <h2>Application result</h2>
        <p className="muted">
          Your loan application decision will appear here after submission.
        </p>
      </section>
    );
  }

  const successful = result.decision === "Successful";

  return (
    <section className="panel">
      <h2>Application result</h2>

      <p className={successful ? "decision success" : "decision declined"}>
        {result.decision}
      </p>

      <dl className="facts">
        <div>
          <dt>Loan amount</dt>
          <dd>{money(result.loanAmount)}</dd>
        </div>

        <div>
          <dt>Asset value</dt>
          <dd>{money(result.assetValue)}</dd>
        </div>

        <div>
          <dt>Credit score</dt>
          <dd>{result.creditScore}</dd>
        </div>

        <div>
          <dt>LTV</dt>
          <dd>{result.ltvPercent.toFixed(2)}%</dd>
        </div>
      </dl>

      {result.declineReason ? (
        <p className="reason">{result.declineReason}</p>
      ) : null}
    </section>
  );
}
