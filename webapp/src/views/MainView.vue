<script setup>
import { ref, watch, onMounted } from 'vue'
import { storeToRefs } from 'pinia'
import { useFeedBlockStore } from '@/stores/feedBlockStore'
import useFeedBlockService from '@/services/feedBlockService'
import BlockFeedNews from '@/components/BlockFeedNews.vue'

const feedBlockStore = useFeedBlockStore()
const feedBlockService = useFeedBlockService()

const { currentBlockSlug } = storeToRefs(feedBlockStore)

const isLoading = ref(false)
const feedBlock = ref(null)

watch(currentBlockSlug, async (newVal) => {
    if (newVal) {
        await loadFeedBlock()
    }
})

const loadFeedBlock = async () => {
    console.log('HomeView::loadFeedBlock: load feed block')

    isLoading.value = true
    try {
        feedBlock.value = await feedBlockService.getFeedBlock(feedBlockStore.currentBlockSlug)
        console.log('HomeView::loadFeedBlock: got feed block. ', feedBlock.value)
    }
    catch (error) {
        console.error('HomeView::loadFeedBlock: error: ', error)
    }
    finally {
        isLoading.value = false
    }
}

onMounted(async () => {
    await loadFeedBlock()
})
</script>


<template>
    <div class="flex flex-col w-full min-h-screen bg-gradient-to-br from-gray-50 via-gray-100 to-gray-50">
        <!-- Loading State -->
        <div v-if="isLoading" class="flex flex-col justify-center items-center h-screen">
            <div class="relative">
                <VaProgressCircle 
                    indeterminate 
                    size="large"
                    :thickness="0.25"
                />
                <div class="absolute inset-0 flex items-center justify-center">
                    <div class="w-10 h-10 rounded-full bg-gradient-to-br from-blue-500 to-purple-600 animate-pulse"></div>
                </div>
            </div>
            <p class="mt-6 text-lg font-medium text-gray-600">Loading feed block...</p>
        </div>

        <!-- Content State -->
        <div v-else class="flex flex-col w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6 sm:py-8">
            <!-- Feed Block Header -->
            <div v-if="feedBlock" class="flex flex-col w-full">
                <!-- Header Section with Gradient -->
                <div 
                    class="relative overflow-hidden rounded-t-xl mb-6"
                    style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border-bottom: 4px solid rgba(255, 255, 255, 0.2);"
                >
                    <div class="absolute inset-0 opacity-10" style="background-image: radial-gradient(circle at 20% 50%, rgba(255, 255, 255, 0.3) 0%, transparent 50%), radial-gradient(circle at 80% 80%, rgba(255, 255, 255, 0.3) 0%, transparent 50%);"></div>
                    
                    <div class="relative px-6 sm:px-8 py-6 sm:py-8">
                        <div class="flex flex-col items-center text-center">
                            <!-- Icon -->
                            <div class="flex items-center justify-center w-16 h-16 sm:w-20 sm:h-20 rounded-2xl bg-white/20 backdrop-blur-sm border border-white/30 shadow-xl mb-4">
                                <svg class="w-10 h-10 sm:w-12 sm:h-12 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10" />
                                </svg>
                            </div>
                            
                            <!-- Title -->
                            <h1 class="text-3xl sm:text-4xl lg:text-5xl font-bold text-white mb-4 drop-shadow-lg leading-tight">
                                {{ feedBlock.title }}
                            </h1>
                            
                            <!-- Description -->
                            <p class="text-base sm:text-lg text-white/90 max-w-3xl leading-relaxed font-medium">
                                {{ feedBlock.description }}
                            </p>
                        </div>
                    </div>
                </div>

                <!-- Feeds Grid -->
                <div v-if="feedBlock.feeds && feedBlock.feeds.length > 0" class="flex flex-col w-full">
                    <div class="flex flex-row grid grid-cols-1 xl:grid-cols-2 gap-4 sm:gap-6 w-full">
                        <BlockFeedNews 
                            v-for="feed in feedBlock.feeds"
                            :key="feed.slug"
                            :feed="feed"
                            class="w-full"
                        />
                    </div>
                </div>

                <!-- Empty Feeds State -->
                <div v-else class="flex flex-col justify-center items-center w-full p-8 sm:p-12 rounded-xl bg-gradient-to-br from-red-50 to-orange-50 border border-red-200/50 shadow-lg">
                    <div class="flex flex-col items-center gap-4">
                        <div class="flex items-center justify-center w-16 h-16 rounded-full bg-red-100 border-4 border-red-200">
                            <svg class="w-8 h-8 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                            </svg>
                        </div>
                        <div class="text-center">
                            <h3 class="text-lg sm:text-xl font-bold text-red-700 mb-2">
                                No feeds available
                            </h3>
                            <p class="text-sm sm:text-base text-red-600/80 max-w-md">
                                This feed block doesn't contain any feeds at the moment.
                            </p>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Error State - No Feed Block -->
            <div v-else class="flex flex-col justify-center items-center w-full p-8 sm:p-12 rounded-xl bg-gradient-to-br from-red-50 to-orange-50 border border-red-200/50 shadow-lg">
                <div class="flex flex-col items-center gap-4">
                    <div class="flex items-center justify-center w-16 h-16 rounded-full bg-red-100 border-4 border-red-200">
                        <svg class="w-8 h-8 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                        </svg>
                    </div>
                    <div class="text-center">
                        <h3 class="text-lg sm:text-xl font-bold text-red-700 mb-2">
                            {{ $t('feed.feed_error_load_title') }}
                        </h3>
                        <p class="text-sm sm:text-base text-red-600/80 max-w-md">
                            {{ $t('feed.feed_error_load_description') }}
                        </p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
/* Ensure no horizontal overflow */
:deep(*) {
    max-width: 100%;
    box-sizing: border-box;
}

/* Prevent any child components from causing horizontal scroll */
:deep(.flex-1),
:deep([class*="grid"]),
:deep([class*="flex"]) {
    max-width: 100%;
    overflow-x: hidden;
}
</style>