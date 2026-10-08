import DefaultTheme from 'vitepress/theme'
import './style/index.css'
import 'virtual:group-icons.css'
import type { Theme } from 'vitepress'
import PluginLoadingFlow from './components/PluginLoadingFlow.vue'
export default {
  extends: DefaultTheme,
  enhanceApp({ app }) {
    app.component('PluginLoadingFlow', PluginLoadingFlow)
  },
} satisfies Theme
