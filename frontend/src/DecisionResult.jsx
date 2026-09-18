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

      {result.fullName ? (
        <>
          <h3 className="section-title">Applicant details</h3>
          <dl className="facts">
            <div>
              <dt>Applicant name</dt>
              <dd>{result.fullName}</dd>
            </div>

            <div>
              <dt>Email</dt>
              <dd>{result.email}</dd>
            </div>

            <div>
              <dt>Phone number</dt>
              <dd>{result.phoneNumber}</dd>
            </div>
          </dl>
        </>
      ) : null}

      <h3 className="section-title">Loan details</h3>
      <dl className="facts">
        <div>
          <dt>Loan amount</dt>
          <dd>{money(result.loanAmount)}</dd>
        </div>

        <div>
          <dt>Property value</dt>
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
