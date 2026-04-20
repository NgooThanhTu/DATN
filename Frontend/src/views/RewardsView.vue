<template>
  <NexusLayout>
    <div class="rewards-page">
      <header class="rewards-header">
        <div>
          <p class="eyebrow">Gamification</p>
          <h1>Rewards</h1>
          <p class="muted">Theo doi diem thuong, cap do va cac lan rollback khi task bi tra lai.</p>
        </div>
        <button class="refresh-btn" @click="loadRewards" :disabled="loading">
          <i class="fa-solid fa-rotate"></i> Refresh
        </button>
      </header>

      <section class="wallet-band">
        <div>
          <span class="label">Current balance</span>
          <strong>{{ wallet.totalPoints }}</strong>
          <span class="unit">points</span>
        </div>
        <div>
          <span class="label">Level</span>
          <strong>{{ wallet.level }}</strong>
          <span class="unit">{{ pointsToNextLevel }} pts to next</span>
        </div>
        <div class="progress-wrap">
          <span class="label">Level progress</span>
          <div class="progress-track"><div class="progress-fill" :style="{ width: levelProgress + '%' }"></div></div>
        </div>
      </section>

      <main class="rewards-grid">
        <section class="panel">
          <div class="panel-head"><h2>Point history</h2><span>{{ transactions.length }}</span></div>
          <div v-if="loading" class="empty">Loading rewards...</div>
          <div v-else-if="!transactions.length" class="empty">Chua co giao dich diem nao.</div>
          <article v-for="tx in transactions" :key="tx.id" class="tx-row">
            <div class="tx-icon" :class="{ negative: tx.amount < 0 }">
              <i :class="tx.amount >= 0 ? 'fa-solid fa-plus' : 'fa-solid fa-minus'"></i>
            </div>
            <div class="tx-main">
              <div class="tx-title">{{ tx.taskSequenceId || tx.taskTitle || tx.reason }}</div>
              <div class="tx-reason">{{ tx.reason }}</div>
              <time>{{ formatDate(tx.createdAt) }}</time>
            </div>
            <strong class="tx-points" :class="{ negative: tx.amount < 0 }">{{ tx.amount > 0 ? '+' : '' }}{{ tx.amount }}</strong>
          </article>
        </section>

        <section class="panel">
          <div class="panel-head"><h2>Leaderboard</h2><span>Top 20</span></div>
          <div v-if="!leaderboard.length" class="empty">Chua co bang xep hang.</div>
          <article v-for="(item, index) in leaderboard" :key="item.userId" class="leader-row">
            <span class="rank">#{{ index + 1 }}</span>
            <span class="avatar">{{ getInitials(item.userName) }}</span>
            <div class="leader-main"><strong>{{ item.userName || 'User' }}</strong><small>Level {{ item.level }}</small></div>
            <span class="leader-points">{{ item.totalPoints }} pts</span>
          </article>
        </section>
      </main>
    </div>
  </NexusLayout>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { ElMessage } from 'element-plus'
import NexusLayout from '@/components/layout/NexusLayout.vue'
import axiosClient from '@/api/axiosClient'

const loading = ref(false)
const wallet = ref({ totalPoints: 0, level: 1, nextLevelAt: 1000 })
const transactions = ref([])
const leaderboard = ref([])

const levelBase = computed(() => Math.max(0, (wallet.value.level - 1) * 1000))
const levelProgress = computed(() => Math.min(100, Math.round((Math.max(0, wallet.value.totalPoints - levelBase.value) / 1000) * 100)))
const pointsToNextLevel = computed(() => Math.max(0, wallet.value.nextLevelAt - wallet.value.totalPoints))

const formatDate = (value) => value ? new Date(value).toLocaleString('vi-VN') : ''
const getInitials = (name = '') => {
  const parts = name.trim().split(/\s+/).filter(Boolean)
  return (parts.length > 1 ? `${parts[0][0]}${parts.at(-1)[0]}` : name.slice(0, 2)).toUpperCase() || 'U'
}

const loadRewards = async () => {
  loading.value = true
  try {
    const [mine, leaders] = await Promise.all([
      axiosClient.get('/gamification/me'),
      axiosClient.get('/gamification/leaderboard')
    ])
    wallet.value = mine.data?.data?.wallet || wallet.value
    transactions.value = mine.data?.data?.transactions || []
    leaderboard.value = leaders.data?.data || []
  } catch (error) {
    ElMessage.error(error.response?.data?.message || 'Khong tai duoc diem thuong.')
  } finally {
    loading.value = false
  }
}

onMounted(loadRewards)
</script>

<style scoped>
.rewards-page { min-height: calc(100vh - 56px); background: #0d0f11; color: #e4e4e7; padding: 32px; }
.rewards-header { display: flex; justify-content: space-between; gap: 24px; align-items: flex-start; margin-bottom: 24px; }
.eyebrow { color: #60a5fa; font-size: 12px; text-transform: uppercase; font-weight: 700; margin: 0 0 8px; }
h1 { margin: 0; font-size: 28px; }
.muted, time, small { color: #a1a1aa; font-size: 13px; }
.refresh-btn { border: 1px solid #27272a; background: #16181d; color: #e4e4e7; border-radius: 6px; padding: 8px 12px; display: flex; gap: 8px; align-items: center; cursor: pointer; }
.wallet-band { display: grid; grid-template-columns: 1fr 1fr 2fr; gap: 16px; border: 1px solid #27272a; background: #111317; border-radius: 8px; padding: 20px; margin-bottom: 20px; }
.label { display: block; color: #a1a1aa; font-size: 12px; margin-bottom: 8px; }
strong { font-size: 28px; }
.unit { color: #71717a; margin-left: 8px; }
.progress-track { height: 10px; background: #27272a; border-radius: 999px; overflow: hidden; }
.progress-fill { height: 100%; background: #22c55e; transition: width .2s ease; }
.rewards-grid { display: grid; grid-template-columns: minmax(0, 1.4fr) minmax(320px, .8fr); gap: 20px; }
.panel { border: 1px solid #27272a; background: #111317; border-radius: 8px; overflow: hidden; }
.panel-head, .tx-row, .leader-row { display: flex; align-items: center; border-bottom: 1px solid #27272a; padding: 14px 16px; }
.panel-head { justify-content: space-between; }
.panel-head h2 { margin: 0; font-size: 15px; }
.empty { padding: 28px; color: #a1a1aa; text-align: center; }
.tx-row, .leader-row { gap: 12px; border-bottom-color: #1e2025; }
.tx-icon, .avatar { width: 28px; height: 28px; display: grid; place-items: center; border-radius: 6px; }
.tx-icon { background: rgba(34,197,94,.14); color: #22c55e; }
.tx-icon.negative { background: rgba(248,113,113,.14); color: #f87171; }
.tx-main, .leader-main { flex: 1; min-width: 0; }
.tx-title { font-weight: 600; }
.tx-reason { color: #a1a1aa; font-size: 13px; margin: 3px 0; }
.tx-points { color: #22c55e; font-size: 18px; }
.tx-points.negative { color: #f87171; }
.rank { width: 36px; color: #71717a; }
.avatar { background: #2563eb; font-size: 11px; font-weight: 700; }
.leader-main strong { display: block; font-size: 14px; }
.leader-points { color: #facc15; font-weight: 700; }
@media (max-width: 900px) { .wallet-band, .rewards-grid { grid-template-columns: 1fr; } .rewards-header { flex-direction: column; } }
</style>
