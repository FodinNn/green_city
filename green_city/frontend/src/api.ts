export interface NotificationDto {
  id: number
  appName: string
  sender: string
  title: string
  text: string
  timestamp: string
}

export interface CheckResult {
  correct: boolean
  actualIsFake: boolean
  explanation: string
}

const BASE = import.meta.env.VITE_API_URL || '/api'

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const res = await fetch(`${BASE}${path}`, init)

  if (!res.ok) {
    let message = `Сервер ответил ошибкой ${res.status}`
    try {
      const body = (await res.json()) as { message?: string }
      if (body?.message) message = body.message
    } catch {
      // тело без JSON, оставляем код статуса
    }
    throw new Error(message)
  }

  return (await res.json()) as T
}

export function getRandomNotification(): Promise<NotificationDto> {
  return request<NotificationDto>('/notifications/random')
}

export function checkAnswer(id: number, isFake: boolean): Promise<CheckResult> {
  return request<CheckResult>(`/notifications/${id}/check`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ isFake }),
  })
}
