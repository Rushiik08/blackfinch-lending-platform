import { useEffect, useState } from 'react'
import ApplicationForm from './ApplicationForm.jsx'
import DecisionResult from './DecisionResult.jsx'
import MetricsPanel from './MetricsPanel.jsx'
import { getMetrics, submitApplication } from './api.js'
import './App.css'

const emptyForm = {
  loanAmount: '',
  assetValue: '',
  creditScore: '',
}

export default function App() {
  const [values, setValues] = useState(emptyForm)
  const [submitting, setSubmitting] = useState(false)
  const [result, setResult] = useState(null)
  const [error, setError] = useState('')
  const [metrics, setMetrics] = useState(null)

  useEffect(() => {
    getMetrics()
      .then(setMetrics)
      .catch(() => setMetrics(null))
  }, [])

  function handleChange(name, value) {
    setValues((current) => ({ ...current, [name]: value }))
  }

  async function handleSubmit(event) {
    event.preventDefault()
    setSubmitting(true)
    setError('')

    try {
      const payload = {
        loanAmount: Number(values.loanAmount),
        assetValue: Number(values.assetValue),
        creditScore: Number(values.creditScore),
      }
      const response = await submitApplication(payload)
      setResult(response)
      setMetrics(response.metrics)
    } catch (submitError) {
      setResult(null)
      setError(submitError.message)
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <main className="page">
      <header>
        <p className="eyebrow">Blackfinch technical test</p>
        <h1>Lending platform</h1>
        <p className="lede">
          Submit a secured loan application. Lending rules run on the API, not in this page.
        </p>
      </header>
      <div className="layout">
        <ApplicationForm
          values={values}
          onChange={handleChange}
          onSubmit={handleSubmit}
          submitting={submitting}
        />
        <div className="stack">
          <DecisionResult result={result} error={error} />
          <MetricsPanel metrics={metrics} />
        </div>
      </div>
    </main>
  )
}
