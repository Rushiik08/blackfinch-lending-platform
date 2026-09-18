import { useEffect, useState } from "react";

import ApplicationForm from "./ApplicationForm.jsx";
import DecisionResult from "./DecisionResult.jsx";
import MetricsPanel from "./MetricsPanel.jsx";
import { getHistory, getMetrics, submitApplication } from "./api.js";
import "./App.css";
import ApplicationHistory from "./ApplicationHistory.jsx";

const emptyForm = {
  fullName: "",
  email: "",
  phoneNumber: "",
  loanAmount: "",
  assetValue: "",
  creditScore: "",
};

export default function App() {
  const [values, setValues] = useState(emptyForm);
  const [errors, setErrors] = useState({});
  const [submitting, setSubmitting] = useState(false);
  const [result, setResult] = useState(null);
  const [error, setError] = useState("");
  const [metrics, setMetrics] = useState(null);
  const [metricsUnavailable, setMetricsUnavailable] = useState(false);
  const [history, setHistory] = useState([]);
  const [historyUnavailable, setHistoryUnavailable] = useState(false);

  useEffect(() => {
    getMetrics()
      .then((data) => {
        setMetrics(data);
        setMetricsUnavailable(false);
      })
      .catch(() => {
        setMetrics(null);
        setMetricsUnavailable(true);
      });

    getHistory()
      .then((data) => {
        setHistory(data);
        setHistoryUnavailable(false);
      })
      .catch(() => {
        setHistory([]);
        setHistoryUnavailable(true);
      });
  }, []);

  function validate(formData) {
    const errs = {};

    if (!formData.fullName || !formData.fullName.trim()) {
      errs.fullName = "Full name is required.";
    } else if (formData.fullName.trim().length > 100) {
      errs.fullName = "Full name cannot exceed 100 characters.";
    }

    const emailPattern = /^[^@\s]+@[^@\s]+\.[a-zA-Z0-9-]{2,}$/;
    if (!formData.email || !formData.email.trim()) {
      errs.email = "Email is required.";
    } else if (!emailPattern.test(formData.email.trim())) {
      errs.email = "Please enter a valid email address.";
    } else if (formData.email.trim().length > 254) {
      errs.email = "Email cannot exceed 254 characters.";
    }

    if (!formData.phoneNumber || !formData.phoneNumber.trim()) {
      errs.phoneNumber = "Phone number is required.";
    } else {
      const trimmedPhone = formData.phoneNumber.trim();
      const digitsOnly = trimmedPhone.replace(/\D/g, "");
      const validChars = /^(\+)?[0-9\s\-()]+$/;

      if (!validChars.test(trimmedPhone) || digitsOnly.length < 7 || digitsOnly.length > 15 || /^0+$/.test(digitsOnly)) {
        errs.phoneNumber = "Please enter a valid phone number.";
      }
    }

    const loan = Number(formData.loanAmount);
    if (!formData.loanAmount || isNaN(loan) || loan <= 0) {
      errs.loanAmount = "Loan amount must be greater than zero.";
    }

    const asset = Number(formData.assetValue);
    if (!formData.assetValue || isNaN(asset) || asset <= 0) {
      errs.assetValue = "Property value must be greater than zero.";
    }

    const rawScore = String(formData.creditScore ?? "").trim();
    if (!rawScore) {
      errs.creditScore = "Credit score is required.";
    } else if (!/^\d+$/.test(rawScore)) {
      errs.creditScore = "Credit score must be a whole integer (no decimals).";
    } else {
      const score = parseInt(rawScore, 10);
      if (score < 1 || score > 999) {
        errs.creditScore = "Credit score must be between 1 and 999.";
      }
    }

    return errs;
  }

  function handleChange(name, value) {
    setValues((current) => ({ ...current, [name]: value }));
    if (errors[name]) {
      setErrors((current) => {
        const next = { ...current };
        delete next[name];
        return next;
      });
    }
  }

  async function handleSubmit(event) {
    event.preventDefault();

    const clientErrors = validate(values);
    if (Object.keys(clientErrors).length > 0) {
      setErrors(clientErrors);
      return;
    }

    setErrors({});
    setSubmitting(true);
    setError("");

    try {
      const payload = {
        fullName: values.fullName.trim(),
        email: values.email.trim(),
        phoneNumber: values.phoneNumber.trim(),
        loanAmount: Number(values.loanAmount),
        assetValue: Number(values.assetValue),
        creditScore: parseInt(values.creditScore, 10),
      };

      const response = await submitApplication(payload);

      setResult(response);
      setMetrics(response.metrics);
      setMetricsUnavailable(false);
      const updatedHistory = await getHistory();
      setHistory(updatedHistory);
      setHistoryUnavailable(false);
    } catch (submitError) {
      setResult(null);
      setError(submitError.message);
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <main className="page">
      <header>
        <h1>Lending Platform</h1>

        <p className="lede">
          Apply for a secured loan and receive an instant lending decision based
          on your application details.
        </p>
      </header>

      <div className="layout">
        <ApplicationForm
          values={values}
          errors={errors}
          onChange={handleChange}
          onSubmit={handleSubmit}
          submitting={submitting}
        />

        <div className="stack">
          <DecisionResult result={result} error={error} />
          <MetricsPanel metrics={metrics} isUnavailable={metricsUnavailable} />
        </div>
      </div>

      <ApplicationHistory
        applications={history}
        isUnavailable={historyUnavailable}
      />
    </main>
  );
}
