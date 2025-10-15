<script setup>
import Header from './HeaderLayout.vue'
import Footer from './FooterLayout.vue'
</script>

<template>
    <div class="flex flex-col h-screen">
        <Header />

        <!-- Main Content -->
        <main class="flex-grow bg-gray-100 p-2.5 overflow-auto">
            <router-view></router-view>
        </main>

        <Footer />
    </div>
</template>