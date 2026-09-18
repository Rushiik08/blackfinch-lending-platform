export async function submitApplication(payload) {
  let response
  try {
    response = await fetch('/api/applications', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload),
    })
  } catch {
    throw new Error(
      'Unable to connect to the lending service. Please ensure the backend server is running.'
    )
  }

  const data = await response.json().catch(() => ({}))

  if (!response.ok) {
    let message = ''

    if (Array.isArray(data.errors)) {
      message = data.errors.join(' ')
    } else if (data.errors && typeof data.errors === 'object') {
      message = Object.values(data.errors).flat().join(' ')
    }

    if (!message) {
      message = data.detail || data.title || 'The application could not be submitted.'
    }

    throw new Error(message)
  }

  return data
}

export async function getMetrics() {
  let response
  try {
    response = await fetch('/api/applications/metrics')
  } catch {
    throw new Error('Unable to connect to the lending service API.')
  }

  const data = await response.json().catch(() => ({}))

  if (!response.ok) {
    throw new Error('Platform statistics could not be loaded.')
  }

  return data
}

export async function getHistory() {
  let response
  try {
    response = await fetch('/api/applications/history')
  } catch {
    throw new Error('Unable to connect to the lending service API.')
  }

  const data = await response.json().catch(() => [])

  if (!response.ok) {
    throw new Error('Application history could not be loaded.')
  }

  return data
}
