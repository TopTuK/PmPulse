<script setup>
import FeedBlockSelector from '@/components/FeedBlockSelector.vue'
</script>

<template>
    <div class="flex flex-row items-center justify-between h-18 min-h-18 bg-gray-800 p-2 w-full">
        <div class="text-white font-extrabold">
            <router-link to="/" class="no-underline hover:text-white hover:no-underline">
                <span class="text-2xl pl-2">
                    {{ $t('common.app_name') }}
                </span>
            </router-link>
        </div>
        <div class="pr-2">
            <FeedBlockSelector />
        </div>
    </div>
</template>