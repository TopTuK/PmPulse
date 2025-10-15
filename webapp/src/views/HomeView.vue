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
    <div class="flex flex-col m-2">
        <div v-if="isLoading" class="flex justify-center items-center h-full">
            <VaProgressCircle indeterminate />
        </div>
        <div v-else class="flex flex-col">
            <div v-if="feedBlock" class="flex flex-col bg-gray-200 border border-gray-300 rounded-lg p-4">
                <div class="flex flex-col items-center justify-center mb-4">
                    <h1 class="text-3xl font-bold text-gray-800 px-2 py-1">
                        {{ feedBlock.title }}
                    </h1>
                </div>
                <div class="flex flex-row items-center justify-center mb-4">
                    <p class="text-base text-gray-700 font-medium px-2 py-1">
                        {{ feedBlock.description }}
                    </p>
                </div>
                <div class="flex flex-row border-t border-gray-400 my-4"></div>
                <div class="flex flex-row justify-center items-center mx-4 my-2">
                    <div
                        v-if="feedBlock.feeds.length > 0"
                        class="flex-1 grid gap-2 grid-cols-2 bg-white"
                    >
                        <BlockFeedNews 
                            v-for="feed in feedBlock.feeds"
                            :key="feed.slug"
                            :feed="feed"
                        />
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>