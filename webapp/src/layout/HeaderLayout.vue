<script setup>
import FeedBlockSelector from '@/components/FeedBlockSelector.vue'
</script>

<template>
    <div class="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-2 sm:gap-0 h-18 min-h-18 bg-gray-800 p-2 w-full">
        <div class="text-white font-extrabold w-full sm:w-auto">
            <router-link to="/" class="no-underline hover:text-white hover:no-underline">
                <span class="text-xl sm:text-2xl pl-0 sm:pl-2">
                    {{ $t('common.app_name') }}
                </span>
            </router-link>
        </div>
        <div class="pr-0 sm:pr-2 w-full sm:w-auto">
            <FeedBlockSelector class="w-full" />
        </div>
    </div>
</template>