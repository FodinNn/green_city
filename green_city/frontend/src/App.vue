<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref } from 'vue'
import { checkAnswer, getRandomNotification, type CheckResult, type NotificationDto } from './api'

const notification = ref<NotificationDto | null>(null)
const result = ref<CheckResult | null>(null)
const choice = ref<boolean | null>(null)
const loading = ref(false)
const submitting = ref(false)
const error = ref('')
const answered = ref(0)
const correct = ref(0)

const initial = computed(() => notification.value?.appName.trim().charAt(0).toUpperCase() ?? '')

const time = computed(() => {
  const raw = notification.value?.timestamp
  if (!raw) return ''
  const date = new Date(raw)
  return Number.isNaN(date.getTime())
    ? ''
    : date.toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' })
})

const verdict = computed(() => {
  if (!result.value) return ''
  return result.value.actualIsFake ? 'это фейк' : 'это настоящее'
})

function reason(e: unknown, fallback: string) {
  return e instanceof Error && e.message ? e.message : fallback
}

async function load(excludeId?: number) {
  if (loading.value) return

  loading.value = true
  error.value = ''
  result.value = null
  choice.value = null
  notification.value = null

  try {
    let next = await getRandomNotification()
    for (let attempt = 0; attempt < 2 && next.id === excludeId; attempt += 1) {
      next = await getRandomNotification()
    }
    notification.value = next
  } catch (e) {
    error.value = reason(e, 'Не удалось загрузить уведомление')
  } finally {
    loading.value = false
  }
}

async function answer(isFake: boolean) {
  const current = notification.value
  if (!current || result.value || submitting.value) return

  submitting.value = true
  choice.value = isFake
  error.value = ''

  try {
    result.value = await checkAnswer(current.id, isFake)
    answered.value += 1
    if (result.value.correct) correct.value += 1
  } catch (e) {
    error.value = reason(e, 'Не удалось проверить ответ')
  } finally {
    submitting.value = false
  }
}

function retryAnswer() {
  if (choice.value !== null) answer(choice.value)
}

function onKeydown(event: KeyboardEvent) {
  if (event.metaKey || event.ctrlKey || event.altKey) return

  if (!notification.value || result.value) {
    if (result.value && notification.value && (event.key === 'Enter' || event.key === ' ')) {
      event.preventDefault()
      load(notification.value.id)
    }
    return
  }

  if (event.key === '1' || event.key === '2') {
    event.preventDefault()
    answer(event.key === '1')
  }
}

onMounted(() => {
  load()
  window.addEventListener('keydown', onKeydown)
})

onUnmounted(() => {
  window.removeEventListener('keydown', onKeydown)
})
</script>

<template>
  <main class="game">
    <header class="top">
      <div class="brand"><span class="dot" />Фейк или нет</div>
      <div class="score">
        <span class="score-value">{{ correct }}/{{ answered }}</span>
        <span class="score-label">верно</span>
      </div>
    </header>

    <p class="tagline">Проверьте уведомление и решите, настоящее оно или поддельное.</p>

    <div v-if="loading" class="card skeleton" aria-hidden="true">
      <div class="sk-row">
        <div class="sk-avatar" />
        <div class="sk-lines">
          <div class="sk-line w-40" />
          <div class="sk-line w-24" />
        </div>
      </div>
      <div class="sk-line w-70" />
      <div class="sk-line w-90" />
    </div>

    <template v-else-if="notification">
      <article class="card">
        <div class="notif-head">
          <div class="app-icon">{{ initial }}</div>
          <div class="meta">
            <span class="app">{{ notification.appName }}</span>
            <span class="sender">{{ notification.sender }}</span>
          </div>
          <time class="time">{{ time }}</time>
        </div>

        <h2 class="title">{{ notification.title }}</h2>
        <p class="text">{{ notification.text }}</p>

        <div v-if="result" class="result" aria-live="polite">
          <p class="verdict" :class="result.correct ? 'ok' : 'bad'">
            <span class="mark">{{ result.correct ? '✓' : '✕' }}</span>
            <span>{{ result.correct ? 'Верно' : 'Ошибка' }}</span>
            <span class="verdict-tail">· {{ verdict }}</span>
          </p>
          <p class="explanation">{{ result.explanation }}</p>
        </div>
      </article>

      <p v-if="error" class="error" role="alert">
        {{ error }}
        <button class="retry" type="button" @click="retryAnswer">Повторить</button>
      </p>

      <div v-if="!result" class="actions">
        <button
          class="answer fake"
          type="button"
          :disabled="submitting"
          :class="{ 'is-chosen': choice === true }"
          @click="answer(true)"
        >
          Фейк
        </button>
        <button
          class="answer real"
          type="button"
          :disabled="submitting"
          :class="{ 'is-chosen': choice === false }"
          @click="answer(false)"
        >
          Настоящее
        </button>
      </div>

      <button v-else class="next" type="button" @click="load(notification.id)">Следующее</button>
    </template>

    <p v-else-if="error" class="error" role="alert">
      {{ error }}
      <button class="retry" type="button" @click="load()">Повторить</button>
    </p>

    <footer class="hint">
      <kbd>1</kbd> фейк
      <kbd>2</kbd> настоящее
      <kbd>Enter</kbd> дальше
    </footer>
  </main>
</template>

<style scoped>
.game {
  width: min(100%, 460px);
  display: flex;
  flex-direction: column;
  gap: 18px;
}

.top {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
}

.brand {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  font-weight: 600;
}

.dot {
  width: 7px;
  height: 7px;
  border-radius: 50%;
  background: var(--accent);
  box-shadow: 0 0 10px rgba(121, 217, 155, 0.6);
}

.score {
  display: flex;
  align-items: baseline;
  gap: 6px;
  font-size: 12px;
  color: var(--text-dim);
}

.score-value {
  font-size: 13px;
  font-weight: 600;
  color: var(--text);
  font-variant-numeric: tabular-nums;
}

.tagline {
  margin: -6px 0 0;
  font-size: 13px;
  color: var(--text-dim);
}

.card {
  background: var(--surface);
  border: 1px solid var(--line);
  border-radius: var(--radius);
  padding: 18px;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.notif-head {
  display: flex;
  align-items: center;
  gap: 10px;
}

.app-icon {
  flex: none;
  width: 34px;
  height: 34px;
  border-radius: 10px;
  display: grid;
  place-items: center;
  background: var(--surface-2);
  border: 1px solid var(--line);
  color: var(--accent);
  font-size: 14px;
  font-weight: 600;
}

.meta {
  flex: 1;
  min-width: 0;
  display: flex;
  flex-direction: column;
  line-height: 1.35;
}

.app {
  font-size: 13px;
  font-weight: 600;
}

.sender {
  font-size: 12px;
  color: var(--text-dim);
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.time {
  flex: none;
  font-size: 12px;
  color: var(--text-dim);
  font-variant-numeric: tabular-nums;
}

.title {
  margin: 2px 0 0;
  font-size: 15px;
  font-weight: 600;
  line-height: 1.4;
}

.text {
  margin: 0;
  font-size: 14px;
  line-height: 1.55;
  color: #c9d2cb;
  overflow-wrap: anywhere;
}

.result {
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin-top: 2px;
  padding-top: 14px;
  border-top: 1px solid var(--line);
}

.verdict {
  margin: 0;
  display: flex;
  align-items: baseline;
  flex-wrap: wrap;
  gap: 8px;
  font-size: 14px;
  font-weight: 600;
}

.mark {
  font-size: 12px;
}

.verdict-tail {
  font-weight: 400;
  color: var(--text-dim);
}

.ok {
  color: var(--accent);
}

.bad {
  color: var(--danger);
}

.explanation {
  margin: 0;
  font-size: 13px;
  line-height: 1.55;
  color: #c9d2cb;
}

.actions {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 10px;
}

.answer,
.next {
  padding: 11px 12px;
  border-radius: 12px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition:
    border-color 0.15s,
    background 0.15s,
    color 0.15s;
}

.answer {
  border: 1px solid var(--line);
  background: var(--surface);
}

.answer:hover:not(:disabled) {
  background: var(--surface-2);
  border-color: #35403a;
}

.answer:disabled {
  cursor: default;
  opacity: 0.55;
}

.answer.is-chosen {
  opacity: 1;
  background: var(--surface-2);
  border-color: #39463f;
}

.answer.fake.is-chosen {
  color: var(--danger);
  border-color: rgba(231, 152, 152, 0.4);
}

.answer.real.is-chosen {
  color: var(--accent);
  border-color: rgba(121, 217, 155, 0.4);
}

.next {
  width: 100%;
  border: 1px solid rgba(121, 217, 155, 0.28);
  background: var(--accent-dim);
  color: var(--accent);
}

.next:hover {
  background: rgba(121, 217, 155, 0.18);
  border-color: rgba(121, 217, 155, 0.45);
}

.error {
  margin: 0;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  padding: 10px 12px;
  border: 1px solid rgba(231, 152, 152, 0.35);
  background: var(--danger-dim);
  border-radius: 12px;
  color: #f0cbcb;
  font-size: 13px;
}

.retry {
  flex: none;
  padding: 0;
  border: none;
  background: none;
  color: inherit;
  font: inherit;
  font-size: 13px;
  text-decoration: underline;
  cursor: pointer;
}

.hint {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 6px;
  font-size: 12px;
  color: var(--text-dim);
}

kbd {
  padding: 4px 6px;
  border: 1px solid var(--line);
  border-bottom-width: 2px;
  border-radius: 5px;
  background: var(--surface);
  font-family: inherit;
  font-size: 11px;
  line-height: 1;
  color: var(--text-dim);
}

kbd + kbd,
kbd + kbd ~ * {
  margin-left: 4px;
}

.skeleton {
  gap: 12px;
}

.sk-row {
  display: flex;
  align-items: center;
  gap: 10px;
}

.sk-avatar {
  width: 34px;
  height: 34px;
  border-radius: 10px;
}

.sk-lines {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.sk-avatar,
.sk-line {
  background: linear-gradient(90deg, var(--surface-2) 25%, #232b27 50%, var(--surface-2) 75%);
  background-size: 200% 100%;
  animation: shimmer 1.4s ease-in-out infinite;
}

.sk-line {
  height: 10px;
  border-radius: 5px;
}

.w-24 {
  width: 24%;
}

.w-40 {
  width: 40%;
}

.w-70 {
  width: 70%;
}

.w-90 {
  width: 90%;
}

@keyframes shimmer {
  from {
    background-position: 200% 0;
  }
  to {
    background-position: -200% 0;
  }
}

@media (hover: none) {
  .hint {
    display: none;
  }
}

@media (prefers-reduced-motion: reduce) {
  .sk-avatar,
  .sk-line {
    animation: none;
  }
}
</style>
