const STORAGE_PREFIX = 'nexus_work_session_v1'

const safeParse = (value) => {
  if (!value) return null
  try {
    return JSON.parse(value)
  } catch {
    return null
  }
}

const getStorageKey = ({ userId, projectId, taskId }) =>
  `${STORAGE_PREFIX}:${userId || 'anonymous'}:${projectId || 'project'}:${taskId || 'task'}`

export const loadWorkSession = ({ userId, projectId, taskId }) => {
  if (!userId || !projectId || !taskId) return null
  const key = getStorageKey({ userId, projectId, taskId })
  return safeParse(localStorage.getItem(key))
}

export const saveWorkSession = ({ userId, projectId, taskId }, session) => {
  if (!userId || !projectId || !taskId) return
  const key = getStorageKey({ userId, projectId, taskId })
  localStorage.setItem(key, JSON.stringify({
    ...session,
    userId,
    projectId,
    taskId,
    updatedAt: Date.now()
  }))
}

export const clearWorkSession = ({ userId, projectId, taskId }) => {
  if (!userId || !projectId || !taskId) return
  const key = getStorageKey({ userId, projectId, taskId })
  localStorage.removeItem(key)
}

export const calculateWorkSessionHours = (session, now = Date.now()) => {
  if (!session) return 0
  const accumulatedMs = Number(session.accumulatedMs) || 0
  const runningMs = session.status === 'running' && session.startedAt
    ? Math.max(0, now - Number(session.startedAt))
    : 0
  const totalMs = accumulatedMs + runningMs

  if (totalMs <= 0) return 0

  const roundedHours = Math.round((totalMs / 3600000) * 10) / 10
  return Math.max(0.1, roundedHours)
}

export const buildFreshWorkSession = ({
  userId,
  projectId,
  taskId,
  taskTitle,
  startedAt = Date.now()
}) => ({
  userId,
  projectId,
  taskId,
  taskTitle: taskTitle || 'Work item',
  status: 'running',
  startedAt,
  accumulatedMs: 0,
  lastActivityAt: startedAt,
  pausedAt: null,
  idlePausedAt: null,
  updatedAt: startedAt
})
