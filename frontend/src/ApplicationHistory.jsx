function money(value) {
  return new Intl.NumberFormat("en-GB", {
    style: "currency",
    currency: "GBP",
  }).format(value);
}

function dateTime(value) {
  return new Intl.DateTimeFormat("en-GB", {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(value));
}

export default function ApplicationHistory({ applications, isUnavailable }) {
  return (
    <section className="panel history-panel">
      <h2>Application history</h2>
      <p className="lede">All submitted applications, newest first.</p>

      {isUnavailable ? (
        <div className="status-banner warning">
          Application history is currently unavailable (backend service offline).
        </div>
      ) : null}

      {!isUnavailable && applications.length === 0 ? (
        <p className="muted">No applications have been submitted yet.</p>
      ) : null}

      {applications.length > 0 ? (
        <div className="history-list">
          {applications.map((application) => {
            const successful = application.decision === "Successful";

            return (
              <article className="history-item" key={application.id}>
                <div className="history-heading">
                  <div>
                    <h3>{application.fullName}</h3>
                    <time dateTime={application.createdAtUtc}>
                      {dateTime(application.createdAtUtc)}
                    </time>
                  </div>
                  <strong className={successful ? "success" : "declined"}>
                    {application.decision}
                  </strong>
                </div>

                <dl className="history-facts">
                  <div>
                    <dt>Loan</dt>
                    <dd>{money(application.loanAmount)}</dd>
                  </div>
                  <div>
                    <dt>Property</dt>
                    <dd>{money(application.assetValue)}</dd>
                  </div>
                  <div>
                    <dt>LTV</dt>
                    <dd>{Number(application.ltvPercent).toFixed(2)}%</dd>
                  </div>
                  <div>
                    <dt>Credit score</dt>
                    <dd>{application.creditScore}</dd>
                  </div>
                </dl>

                {application.declineReason ? (
                  <p className="reason">{application.declineReason}</p>
                ) : null}
              </article>
            );
          })}
        </div>
      ) : null}
    </section>
  );
}
