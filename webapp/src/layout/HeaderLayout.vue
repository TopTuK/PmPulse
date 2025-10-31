<script setup>
import FeedBlockSelector from '@/components/FeedBlockSelector.vue'
</script>

<template>
    <header class="sticky top-0 z-50 w-full backdrop-blur-md bg-gray-900/95 border-b border-gray-700/50 shadow-lg">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
            <div class="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4 py-4 sm:py-5">
                <!-- Logo Section -->
                <div class="flex-shrink-0 w-full sm:w-auto">
                    <router-link 
                        to="/" 
                        class="group flex items-center gap-2 sm:gap-3 no-underline transition-all duration-200 hover:opacity-90"
                    >
                        <!-- Modern logo icon -->
                        <div class="flex items-center justify-center w-10 h-10 sm:w-12 sm:h-12 rounded-xl bg-gradient-to-br from-blue-500 to-purple-600 shadow-lg shadow-blue-500/30 group-hover:shadow-blue-500/50 transition-shadow duration-200">
                            <svg class="w-6 h-6 sm:w-7 sm:h-7 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 20H5a2 2 0 01-2-2V6a2 2 0 012-2h10a2 2 0 012 2v1m2 13a2 2 0 01-2-2V7m2 13a2 2 0 002-2V9a2 2 0 00-2-2h-2m-4-3H9M7 16h6M7 8h6v4H7V8z" />
                            </svg>
                        </div>
                        <div class="flex flex-col">
                            <h1 class="text-xl sm:text-2xl lg:text-3xl font-bold text-white tracking-tight group-hover:text-gray-100 transition-colors">
                                {{ $t('common.app_name') }}
                            </h1>
                            <div class="hidden sm:block h-0.5 w-0 bg-gradient-to-r from-blue-500 to-purple-600 group-hover:w-full transition-all duration-300 mt-0.5"></div>
                        </div>
                    </router-link>
                </div>

                <!-- Controls Section -->
                <div class="w-full sm:w-auto flex items-center justify-end">
                    <div class="w-full sm:w-auto bg-gray-800/50 backdrop-blur-sm rounded-xl px-4 py-3 border border-gray-700/50 shadow-inner">
                        <FeedBlockSelector class="w-full sm:w-auto min-w-[220px]" />
                    </div>
                </div>
            </div>
        </div>
    </header>
</template>