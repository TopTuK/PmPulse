import { createRouter, createWebHistory } from 'vue-router';

const Home = () => import('@/views/MainView.vue')
const Feed = () => import('@/views/FeedView.vue')
const WeeklyDigest = () => import('@/views/WeeklyDigestView.vue')
const About = () => import('@/views/AboutView.vue')

const routes = [
    {
        path: "/:catchAll(.*)",
        redirect: { name: "Home" },
    },
    {
        path: "/",
        name: "Home",
        component: Home,
        meta: {
            title: "PM Pulse: новости в мире менеджмента | Агрегатор новостей",
            description: "PmPulse - современный веб-сервис для агрегации и организации новостей. Помогает оставаться в курсе событий без лишнего шума, объединяя все новостные источники в едином интерфейсе.",
            keywords: "новости, менеджмент, агрегатор новостей, RSS, лента новостей, управление, бизнес новости",
        },
    },
    {
        path: "/about",
        name: "About",
        component: About,
        meta: {
            title: "О PmPulse | PM Pulse - Агрегатор новостей",
            description: "Узнайте больше о PmPulse - современном веб-сервисе для агрегации и организации новостей в мире менеджмента. Технологии, архитектура и возможности.",
            keywords: "PmPulse, о проекте, технологии, Vue.js, Orleans, новостной агрегатор",
        },
    },
    {
        path: "/feed/:slug",
        name: "Feed",
        component: Feed,
        meta: {
            title: "Лента новостей | PM Pulse",
            description: "Просмотр ленты новостей в PM Pulse. Оставайтесь в курсе последних событий в мире менеджмента.",
            keywords: "лента новостей, новости, менеджмент, RSS",
        },
    },
    {
        path: "/digest",
        name: "Digest",
        component: WeeklyDigest,
        meta: {
            title: "Еженедельная сводка | PM Pulse - Агрегатор новостей",
            description: "Еженедельная сводка самых важных постов из различных источников новостей в мире менеджмента за последние 7 дней. Оставайтесь в курсе ключевых событий и трендов.",
            keywords: "еженедельная сводка, дайджест новостей, новости менеджмента, агрегатор новостей, сводка новостей, управление, бизнес новости",
        }
    }
]

// Helper function to update meta tags
function updateMetaTags(route) {
    const meta = route.meta || {};
    
    // Update document title
    if (meta.title) {
        document.title = meta.title;
    }
    
    // Update or create meta description
    let metaDescription = document.querySelector('meta[name="description"]');
    if (meta.description) {
        if (metaDescription) {
            metaDescription.setAttribute('content', meta.description);
        } else {
            metaDescription = document.createElement('meta');
            metaDescription.setAttribute('name', 'description');
            metaDescription.setAttribute('content', meta.description);
            document.head.appendChild(metaDescription);
        }
    }
    
    // Update or create meta keywords
    if (meta.keywords) {
        let metaKeywords = document.querySelector('meta[name="keywords"]');
        if (metaKeywords) {
            metaKeywords.setAttribute('content', meta.keywords);
        } else {
            metaKeywords = document.createElement('meta');
            metaKeywords.setAttribute('name', 'keywords');
            metaKeywords.setAttribute('content', meta.keywords);
            document.head.appendChild(metaKeywords);
        }
    }
    
    // Update Open Graph tags
    if (meta.title) {
        updateOGTag('og:title', meta.title);
    }
    if (meta.description) {
        updateOGTag('og:description', meta.description);
    }
    if (route.fullPath) {
        const baseUrl = window.location.origin;
        updateOGTag('og:url', baseUrl + route.fullPath);
    }
    
    // Update canonical URL
    if (route.fullPath) {
        let canonical = document.querySelector('link[rel="canonical"]');
        const baseUrl = window.location.origin;
        const canonicalUrl = baseUrl + route.fullPath;
        if (canonical) {
            canonical.setAttribute('href', canonicalUrl);
        } else {
            canonical = document.createElement('link');
            canonical.setAttribute('rel', 'canonical');
            canonical.setAttribute('href', canonicalUrl);
            document.head.appendChild(canonical);
        }
    }
}

// Helper function to update Open Graph tags
function updateOGTag(property, content) {
    let ogTag = document.querySelector(`meta[property="${property}"]`);
    if (ogTag) {
        ogTag.setAttribute('content', content);
    } else {
        ogTag = document.createElement('meta');
        ogTag.setAttribute('property', property);
        ogTag.setAttribute('content', content);
        document.head.appendChild(ogTag);
    }
}

const router = createRouter({
    history: createWebHistory(),
    routes,
    scrollBehavior(to, from, savedPosition) {
        return savedPosition || { top: 0 };
    },
});

// Update meta tags on route change
router.afterEach((to) => {
    updateMetaTags(to);
});

export default router;