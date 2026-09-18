function Field({ id, label, hint, ...props }) {
  return (
    <label className="field" htmlFor={id}>
      <span className="field-label">{label}</span>
      <input id={id} {...props} />
      {hint ? <span className="field-hint">{hint}</span> : null}
    </label>
  );
}

export default function ApplicationForm({
  values,
  onChange,
  onSubmit,
  submitting,
}) {
  return (
    <form className="panel" onSubmit={onSubmit}>
      <h2>New application</h2>

      <p className="lede">
        Enter your loan details below to submit an application.
      </p>

      <Field
        id="loanAmount"
        label="Loan amount (GBP)"
        hint="Must be greater than zero. Amounts under £100,000 or over £1.5 million are declined."
        type="number"
        min="0.01"
        step="0.01"
        required
        value={values.loanAmount}
        onChange={(event) => onChange("loanAmount", event.target.value)}
      />

      <Field
        id="assetValue"
        label="Asset value (GBP)"
        hint="Value of the asset securing the loan. Must be greater than zero."
        type="number"
        min="0.01"
        step="0.01"
        required
        value={values.assetValue}
        onChange={(event) => onChange("assetValue", event.target.value)}
      />

      <Field
        id="creditScore"
        label="Credit score"
        hint="Integer from 1 to 999."
        type="number"
        min="1"
        max="999"
        step="1"
        required
        value={values.creditScore}
        onChange={(event) => onChange("creditScore", event.target.value)}
      />

      <button type="submit" disabled={submitting}>
        {submitting ? "Submitting…" : "Submit application"}
      </button>
    </form>
  );
}
