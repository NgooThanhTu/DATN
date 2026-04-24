export const isExpectedNetworkError = (error) => {
  const code = error?.code || ''
  const message = `${error?.message || ''}`.toLowerCase()
  const status = error?.response?.status

  return (
    code === 'ERR_NETWORK' ||
    code === 'ECONNABORTED' ||
    status === 401 ||
    message.includes('network error') ||
    message.includes('failed to fetch') ||
    message.includes('connection refused')
  )
}

export const reportExpectedError = (label, error) => {
  if (isExpectedNetworkError(error)) {
    console.warn(`${label}: temporary backend/auth issue`)
    return
  }

  console.error(label, error)
}
