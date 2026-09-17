export async function submitApplication(payload) {
  const response = await fetch('/api/applications', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  })

  const data = await response.json().catch(() => ({}))

  if (!response.ok) {
    const message = data.errors?.join(' ') || data.title || 'The application could not be submitted.'
    throw new Error(message)
  }

  return data
}

export async function getMetrics() {
  const response = await fetch('/api/applications/metrics')
  const data = await response.json().catch(() => ({}))

  if (!response.ok) {
    throw new Error('Platform statistics could not be loaded.')
  }

  return data
}
