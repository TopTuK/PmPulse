import './main.css'

import { createApp } from 'vue'
import App from './App.vue'

import { createVuestic } from "vuestic-ui";
import 'vuestic-ui/styles/essential.css';
import 'vuestic-ui/styles/typography.css';

import router from "@/router/index.js"

import { createI18n } from 'vue-i18n'
import messages from '@/locs/messages.js'

import { createPinia } from 'pinia'

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

const gtag = createGtag({
    tagId: "G-GEC54SP4WL"
})

// Create APP
const app = createApp(App)

/* USE Section */
app.use(vuestic)
app.use(i18n)
app.use(pinia)
app.use(router)
app.use(gtag)

/* MOUNT APP */
app.mount('#app')
