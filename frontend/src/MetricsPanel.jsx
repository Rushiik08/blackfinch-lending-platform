function money(value) {
  return new Intl.NumberFormat('en-GB', {
    style: 'currency',
    currency: 'GBP',
  }).format(value)
}

export default function MetricsPanel({ metrics, isUnavailable }) {
  const empty = {
    successfulApplicants: 0,
    declinedApplicants: 0,
    totalApplicants: 0,
    totalValueOfLoansWritten: 0,
    meanAverageLtv: 0,
  }
  const data = metrics ?? empty

  return (
    <section className="panel">
      <h2>Platform statistics</h2>
      {isUnavailable ? (
        <div className="status-banner warning">
          Platform statistics are currently unavailable (backend service offline).
        </div>
      ) : null}
      <dl className="facts">
        <div>
          <dt>Successful applicants</dt>
          <dd>{data.successfulApplicants}</dd>
        </div>
        <div>
          <dt>Declined applicants</dt>
          <dd>{data.declinedApplicants}</dd>
        </div>
        <div>
          <dt>Total applicants</dt>
          <dd>{data.totalApplicants}</dd>
        </div>
        <div>
          <dt>Total value of loans written</dt>
          <dd>{money(data.totalValueOfLoansWritten)}</dd>
        </div>
        <div>
          <dt>Mean average LTV</dt>
          <dd>{Number(data.meanAverageLtv).toFixed(2)}%</dd>
        </div>
      </dl>
    </section>
  )
}
