import './main.css'

import { createApp } from 'vue'
import App from './App.vue'

import * as Sentry from '@sentry/vue'

import { createVuestic } from "vuestic-ui";
import 'vuestic-ui/styles/essential.css';
import 'vuestic-ui/styles/typography.css';

import router from "@/router/index.js"

import { createI18n } from 'vue-i18n'
import messages from '@/locs/messages.js'

import { createPinia } from 'pinia'
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate'

import { createGtag } from 'vue-gtag';

/* CREATE Section */
const vuestic = createVuestic()

const i18n = createI18n({
    legacy: false,
    locale: 'ru',
    fallbackLocale: 'en',
    messages, // import messages
})

const pinia = createPinia()
pinia.use(piniaPluginPersistedstate)

const gtag = createGtag({
    tagId: "G-GEC54SP4WL"
})

// Create APP
const app = createApp(App)

// Initialize Sentry only in production
if (import.meta.env.PROD) {
  Sentry.init({
    app,
    dsn: "https://2c9c8782d3842c6f71014a79fdbf03ee@o1286219.ingest.us.sentry.io/4510416084467712",
    integrations: [
      Sentry.browserTracingIntegration(),
      Sentry.replayIntegration({
        maskAllText: true,
        blockAllMedia: true,
      }),
    ],
    // Performance Monitoring
    tracesSampleRate: 1.0, // Capture 100% of the transactions
    // Session Replay
    replaysSessionSampleRate: 0.1, // Sample 10% of sessions
    replaysOnErrorSampleRate: 1.0, // Sample 100% of sessions with errors
    // Set the environment
    environment: 'production',
  })
}

/* USE Section */
app.use(vuestic)
app.use(i18n)
app.use(pinia)
app.use(router)
app.use(gtag)

/* MOUNT APP */
app.mount('#app')
