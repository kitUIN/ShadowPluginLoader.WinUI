<script setup lang="ts">
import { computed, onBeforeUnmount, ref, watch } from 'vue'
import { createSteps, phases, scenarios, type Locale, type Scenario, type Source } from './loadingFlow'

const props = withDefaults(defineProps<{ locale?: Locale }>(), { locale: 'zh' })
const t = (text: [string, string]) => text[props.locale === 'zh' ? 0 : 1]
const scenario = ref<Scenario>('enabled')
const source = ref<Source>('archive')
const index = ref(0)
const playing = ref(false)
const steps = computed(() => createSteps(scenario.value, source.value))
const current = computed(() => steps.value[index.value])
const finished = computed(() => index.value === steps.value.length - 1)
const events = computed(() => steps.value.slice(0, index.value + 1).filter(step => step.event))
const phaseSteps = computed(() => steps.value.map((step, i) => ({ ...step, index: i })).filter(step => step.phase === current.value.phase))
const kinds = { work: ['内部处理', 'Internal work'], callback: ['回调 / 钩子', 'Callback / hook'], event: ['公共事件', 'Public event'], error: ['异常中断', 'Exception'] } as const
const kindLabel = computed(() => kinds[current.value.kind][props.locale === 'zh' ? 0 : 1])
let timer: ReturnType<typeof setInterval> | undefined
function pause() {
  clearInterval(timer)
  timer = undefined
  playing.value = false
}
function go(next: number) {
  pause()
  index.value = Math.max(0, Math.min(steps.value.length - 1, next))
}
function play() {
  if (playing.value) return pause()
  if (finished.value) index.value = 0
  playing.value = true
  timer = setInterval(() => {
    index.value++
    if (finished.value) pause()
  }, 1600)
}
function phaseIndex(phase: number) { return steps.value.findIndex(step => step.phase === phase) }
watch([scenario, source], () => go(0), { flush: 'sync' })
onBeforeUnmount(pause)
</script>

<template>
  <section class="loading-flow" :aria-label="t(['插件加载交互演示', 'Interactive plugin loading flow'])">
    <header class="flow-heading">
      <div><span class="eyebrow">PLUGIN LIFECYCLE</span><h2>{{ t(['跟随一个插件的加载旅程', 'Follow a plugin through loading']) }}</h2></div>
      <span class="demo-label">{{ t(['源码流程演示', 'Source-based simulation']) }}</span>
    </header>
    <p class="intro">{{ t(['从输入文件到启用，逐步查看每个调用与事件。选择阶段可直接跳转。', 'Step from an input file to an enabled plugin. Select a phase to jump to its calls and events.']) }}</p>

    <div class="flow-options">
      <label>{{ t(['演示场景', 'Scenario']) }}<select v-model="scenario"><option v-for="item in scenarios" :key="item.value" :value="item.value">{{ t(item.label) }}</option></select></label>
      <label>{{ t(['插件来源', 'Input source']) }}<select v-model="source"><option value="archive">{{ t(['本地压缩包 (.sdow)', 'Local archive (.sdow)']) }}</option><option value="local">{{ t(['本地 plugin.json', 'Local plugin.json']) }}</option><option value="http">{{ t(['HTTP 压缩包', 'HTTP archive']) }}</option></select></label>
    </div>

    <nav class="phase-track" :aria-label="t(['加载阶段', 'Loading phases'])">
      <button v-for="(phase, p) in phases" :key="p" type="button" :disabled="phaseIndex(p) < 0" :aria-current="current.phase === p ? 'step' : undefined" :class="{ active: current.phase === p, passed: current.phase > p }" @click="go(phaseIndex(p))">
        <span class="phase-number">{{ current.phase > p ? '✓' : String(p + 1).padStart(2, '0') }}</span><span>{{ t(phase) }}</span>
      </button>
    </nav>

    <div class="flow-body">
      <ol class="step-list" :aria-label="t(['当前阶段的步骤', 'Steps in this phase'])">
        <li v-for="step in phaseSteps" :key="step.id"><button type="button" :aria-current="index === step.index ? 'step' : undefined" :class="{ selected: index === step.index, reached: index > step.index }" @click="go(step.index)"><span class="step-mark">{{ index > step.index ? '✓' : String(step.index + 1).padStart(2, '0') }}</span><span>{{ t(step.title) }}</span><span v-if="step.event" class="event-dot" :aria-label="t(['公共事件', 'Public event'])">●</span></button></li>
      </ol>
      <div class="step-detail" aria-live="polite" aria-atomic="true">
        <div class="detail-meta"><span :class="['kind', current.kind]">{{ kindLabel }}</span><span>{{ index + 1 }} / {{ steps.length }}</span></div>
        <h3>{{ t(current.title) }}</h3>
        <p>{{ t(current.detail) }}</p>
        <pre class="call-code"><code>{{ current.code }}</code></pre>
        <div class="signal"><span>{{ t(['本步公共事件', 'Public event at this step']) }}</span><strong>{{ current.event || t(['无', 'None']) }}</strong></div>
        <div v-if="current.progress" class="signal"><span>{{ t(['进度通知', 'Progress notification']) }}</span><code>{{ current.progress }}</code></div>
        <a class="source-link" :href="`https://github.com/kitUIN/ShadowPluginLoader.WinUI/blob/master/ShadowPluginLoader.WinUI/${current.file}`" target="_blank" rel="noreferrer">{{ t(['查看实现', 'View implementation']) }} ↗ <span>{{ current.file }}</span></a>
      </div>
    </div>

    <div class="playback">
      <button type="button" class="play" @click="play">{{ playing ? t(['暂停', 'Pause']) : finished ? t(['重新播放', 'Replay']) : t(['自动播放', 'Play']) }}</button>
      <button type="button" :disabled="index === 0" @click="go(index - 1)">{{ t(['上一步', 'Previous']) }}</button>
      <button type="button" :disabled="finished" @click="go(index + 1)">{{ t(['下一步', 'Next']) }}</button>
      <button type="button" :disabled="index === 0 && !playing" @click="go(0)">{{ t(['重置', 'Reset']) }}</button>
      <span class="playback-state">{{ finished ? current.kind === 'error' ? t(['流程已中断', 'Flow interrupted']) : t(['演示完成', 'Demo complete']) : t(['执行至当前步骤', 'Executed through this step']) }}</span>
    </div>

    <section class="event-trace" :aria-label="t(['已触发的公共事件', 'Emitted public events'])">
      <div class="trace-heading"><h3>{{ t(['事件轨迹', 'Event trace']) }}</h3><span>IPluginEventService · {{ events.length }}</span></div>
      <p v-if="!events.length" class="empty-trace">{{ t(['尚未触发公共事件。读取文件、加载 DLL 和创建实例不会发送 PluginLoaded。', 'No public events yet. Reading files, loading the DLL, and creating the instance do not emit PluginLoaded.']) }}</p>
      <ol v-else class="event-list"><li v-for="(event, e) in events" :key="event.id"><span class="event-order">{{ e + 1 }}</span><div><strong>{{ event.event }}</strong><span>sender = {{ event.sender }} · Status = {{ event.event?.replace('Plugin', '') }}</span></div></li></ol>
    </section>
    <p class="flow-note">{{ t(['本演示不执行真实插件。步骤编号表示教学顺序，不代表 SDK 进度百分比；多插件预处理和预加载可能并发。', 'This simulation does not execute plugins. Step numbers indicate teaching order, not SDK progress percentages; preprocessing and preloading may run concurrently across plugins.']) }}</p>
  </section>
</template>

<style scoped>
.loading-flow { margin: 28px 0; border: 1px solid var(--vp-c-divider); border-radius: 16px; background: var(--vp-c-bg); overflow: hidden; color: var(--vp-c-text-1); }
.flow-heading { display: flex; align-items: center; justify-content: space-between; gap: 16px; padding: 24px 24px 0; flex-wrap: wrap; }
.eyebrow { color: var(--vp-c-brand-1); font-size: 11px; letter-spacing: .15em; font-weight: 600; }
.loading-flow h2 { margin: 5px 0 0; padding: 0; border: 0; font-size: 22px; line-height: 1.4; letter-spacing: -.02em; }
.loading-flow h3 { margin: 0; padding: 0; font-size: 18px; line-height: 1.5; }
.demo-label, .kind { background: var(--vp-c-default-soft); border-radius: 6px; padding: 3px 8px; font-size: 12px; }
.loading-flow .intro { margin: 10px 24px 20px; color: var(--vp-c-text-2); font-size: 14px; }
.flow-options { display: flex; flex-wrap: wrap; gap: 12px; padding: 0 24px 24px; }
.flow-options label { display: grid; gap: 5px; flex: 1 1 180px; min-width: 0; font-size: 12px; color: var(--vp-c-text-2); }
.flow-options select { width: 100%; min-height: 40px; border: 1px solid var(--vp-c-divider); border-radius: 8px; padding: 8px 10px; background: var(--vp-c-bg-alt); color: var(--vp-c-text-1); font: inherit; font-size: 14px; appearance: auto; }
.phase-track { display: grid; grid-template-columns: repeat(6, minmax(0, 1fr)); gap: 4px; padding: 14px; border-block: 1px solid var(--vp-c-divider); background: var(--vp-c-bg-alt); }
.phase-track button { display: flex; flex-direction: column; align-items: center; gap: 6px; padding: 10px 3px; border-radius: 8px; font-size: 12px; line-height: 1.4; }
.phase-number { display: grid; place-items: center; width: 28px; height: 28px; border-radius: 50%; font: 12px var(--vp-font-family-mono); background: var(--vp-c-default-soft); }
.phase-track .active { background: var(--vp-c-brand-soft); color: var(--vp-c-brand-1); font-weight: 600; }
.active .phase-number { background: var(--vp-c-brand-3); color: var(--vp-c-white); }
.passed .phase-number { color: var(--vp-c-brand-1); }
.flow-body { display: grid; grid-template-columns: minmax(160px, .8fr) minmax(0, 1.4fr); }
.loading-flow .step-list { list-style: none; margin: 0; padding: 16px 10px; background: var(--vp-c-bg-alt); }
.step-list li + li { margin-top: 4px; }
.step-list button { width: 100%; text-align: start; display: flex; align-items: baseline; gap: 8px; padding: 10px; border-radius: 7px; font-size: 13px; line-height: 1.6; }
.step-mark { font: 11px var(--vp-font-family-mono); color: var(--vp-c-text-2); }
.step-list .selected { color: var(--vp-c-brand-1); background: var(--vp-c-brand-soft); }
.step-list .reached .step-mark, .event-dot { color: var(--vp-c-brand-1); }
.event-dot { margin-left: auto; font-size: 10px; }
.step-detail { padding: 22px; min-width: 0; }
.detail-meta { display: flex; align-items: center; justify-content: space-between; gap: 12px; margin-bottom: 12px; color: var(--vp-c-text-2); font-size: 12px; font-variant-numeric: tabular-nums; }
.kind.event { color: var(--vp-c-brand-1); background: var(--vp-c-brand-soft); }
.kind.error { color: var(--vp-c-danger-1); background: var(--vp-c-danger-soft); }
.kind.callback { color: var(--vp-c-warning-1); background: var(--vp-c-warning-soft); }
.step-detail p { font-size: 14px; line-height: 1.8; margin: 12px 0; }
.loading-flow .call-code { margin: 16px 0; padding: 14px; background: var(--vp-code-block-bg); border-radius: 8px; white-space: pre-wrap; overflow-wrap: anywhere; font-size: 12px; line-height: 1.8; }
.call-code code { background: transparent; padding: 0; color: var(--vp-c-text-1); font-size: inherit; }
.signal { display: flex; flex-wrap: wrap; justify-content: space-between; gap: 6px 14px; padding: 8px 0; font-size: 12px; border-top: 1px solid var(--vp-c-divider); }
.signal > span { color: var(--vp-c-text-2); }
.signal code { white-space: normal; overflow-wrap: anywhere; }
.source-link { display: block; margin-top: 12px; font-size: 12px; overflow-wrap: anywhere; }
.source-link span { display: block; color: var(--vp-c-text-2); font-size: 11px; }
.playback { display: flex; flex-wrap: wrap; align-items: center; gap: 8px; padding: 16px 20px; border-top: 1px solid var(--vp-c-divider); }
.playback button { border: 1px solid var(--vp-c-divider); border-radius: 7px; min-height: 36px; padding: 6px 12px; font-size: 13px; }
.playback .play { color: var(--vp-c-white); background: var(--vp-c-brand-3); border-color: transparent; }
.playback-state { margin-left: auto; font-size: 12px; color: var(--vp-c-text-2); }
.loading-flow button { cursor: pointer; transition: background .15s; }
.loading-flow button:disabled { cursor: default; opacity: .4; }
.loading-flow button:not(:disabled):hover { background: var(--vp-c-default-soft); }
.loading-flow .play:not(:disabled):hover { background: var(--vp-c-brand-2); }
.loading-flow :is(button, select):focus-visible { outline: 2px solid var(--vp-c-brand-1); outline-offset: 2px; }
.event-trace { margin: 0 20px; padding: 16px 0; border-top: 1px solid var(--vp-c-divider); }
.trace-heading { display: flex; flex-wrap: wrap; align-items: center; justify-content: space-between; gap: 8px; }
.trace-heading h3 { font-size: 14px; }
.trace-heading > span { font: 11px var(--vp-font-family-mono); color: var(--vp-c-text-2); }
.loading-flow .empty-trace { font-size: 13px; color: var(--vp-c-text-2); margin: 10px 0 0; }
.loading-flow .event-list { list-style: none; margin: 12px 0 0; padding: 0; display: flex; flex-wrap: wrap; gap: 16px; }
.event-list li { display: flex; align-items: flex-start; gap: 8px; margin: 0; }
.event-order { color: var(--vp-c-brand-1); font: 12px var(--vp-font-family-mono); padding-top: 4px; }
.event-list strong { display: block; font: 12px var(--vp-font-family-mono); }
.event-list li div > span { display: block; font-size: 11px; color: var(--vp-c-text-2); }
.loading-flow .flow-note { padding: 12px 20px; margin: 0; background: var(--vp-c-bg-alt); color: var(--vp-c-text-2); font-size: 12px; line-height: 1.7; }
@media (max-width: 600px) {
  .flow-heading { padding: 18px 16px 0; }
  .loading-flow h2 { font-size: 20px; }
  .loading-flow .intro { margin-inline: 16px; }
  .flow-options { padding: 0 16px 16px; }
  .flow-options select { font-size: 16px; }
  .phase-track { grid-template-columns: repeat(3, minmax(0, 1fr)); padding: 8px; }
  .flow-body { grid-template-columns: minmax(0, 1fr); }
  .loading-flow .step-list { padding: 12px; }
  .step-detail { padding: 18px 16px; }
  .playback { padding: 14px 16px; }
  .playback-state { width: 100%; margin-left: 0; }
  .playback button, .step-list button { min-height: 44px; }
}
@media (prefers-reduced-motion: reduce) { .loading-flow button { transition: none; } }
</style>
