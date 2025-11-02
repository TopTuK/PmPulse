import { createRouter, createWebHistory } from 'vue-router';

const Home = () => import('@/views/MainView.vue')
const Feed = () => import('@/views/FeedView.vue')
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
            title: "home_view_title",
        },
    },
    {
        path: "/about",
        name: "About",
        component: About,
        meta: {
            title: "about_view_title",
        },
    },
    {
        path: "/feed/:slug",
        name: "Feed",
        component: Feed,
        meta: {
            title: "feed_view_title",
        },
    },
]

const router = createRouter({
    history: createWebHistory(),
    routes,
    scrollBehavior(to, from, savedPosition) {
        return savedPosition || { top: 0 };
    },
});

export default router;