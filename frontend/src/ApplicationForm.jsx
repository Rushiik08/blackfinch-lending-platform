function Field({ id, label, hint, error, required, ...props }) {
  return (
    <label className="field" htmlFor={id}>
      <span className="field-label">
        {label}
        {required ? <span className="required-star" aria-hidden="true">*</span> : null}
      </span>
      <input
        id={id}
        required={required}
        className={error ? "has-error" : ""}
        {...props}
      />
      {error ? <span className="field-error">{error}</span> : null}
      {hint ? <span className="field-hint">{hint}</span> : null}
    </label>
  );
}

export default function ApplicationForm({
  values,
  errors,
  onChange,
  onSubmit,
  submitting,
}) {
  return (
    <form className="panel" onSubmit={onSubmit} noValidate>
      <h2>New application</h2>

      <p className="lede">
        Enter applicant information and secured loan details below.
      </p>

      <Field
        id="fullName"
        label="Full Name"
        type="text"
        required
        placeholder="e.g. Rahul Patil"
        value={values.fullName}
        onChange={(event) => onChange("fullName", event.target.value)}
        error={errors?.fullName}
      />

      <Field
        id="email"
        label="Email Address"
        type="email"
        required
        placeholder="e.g. rahul@example.com"
        value={values.email}
        onChange={(event) => onChange("email", event.target.value)}
        error={errors?.email}
      />

      <Field
        id="phoneNumber"
        label="Phone Number"
        type="tel"
        required
        placeholder="e.g. 9876543210"
        hint="Indian numbers (e.g. 9876543210) or international format"
        value={values.phoneNumber}
        onChange={(event) => onChange("phoneNumber", event.target.value)}
        error={errors?.phoneNumber}
      />

      <Field
        id="loanAmount"
        label="Loan Amount (GBP)"
        type="number"
        min="0.01"
        step="0.01"
        required
        placeholder="e.g. 500000"
        hint="Standard approval between £100,000 and £1,500,000"
        value={values.loanAmount}
        onChange={(event) => onChange("loanAmount", event.target.value)}
        error={errors?.loanAmount}
      />

      <Field
        id="assetValue"
        label="Property Value (GBP)"
        type="number"
        min="0.01"
        step="0.01"
        required
        placeholder="e.g. 750000"
        hint="Market value of the collateral asset securing the loan"
        value={values.assetValue}
        onChange={(event) => onChange("assetValue", event.target.value)}
        error={errors?.assetValue}
      />

      <Field
        id="creditScore"
        label="Credit Score"
        type="number"
        min="1"
        max="999"
        step="1"
        required
        placeholder="e.g. 750"
        hint="Whole integer between 1 and 999"
        value={values.creditScore}
        onChange={(event) => onChange("creditScore", event.target.value)}
        error={errors?.creditScore}
      />

      <button type="submit" disabled={submitting}>
        {submitting ? "Submitting…" : "Submit application"}
      </button>
    </form>
  );
}
