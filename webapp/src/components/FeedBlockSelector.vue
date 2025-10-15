<script setup>
import { ref, onBeforeMount } from 'vue'
import { storeToRefs } from 'pinia'
import { useFeedBlockStore } from '@/stores/feedBlockStore'
import useFeedBlockService from '@/services/feedBlockService'

const feedBlockStore = useFeedBlockStore()
const { currentBlockSlug } = storeToRefs(feedBlockStore)

const blocks = ref(null)
const isLoading = ref(false)

const feedBlockService = useFeedBlockService()

const loadFeedBlocks = async () => {
    isLoading.value = true

    try {
        const blockList = await feedBlockService.getFeedBlocks()
        console.log('FeedBlockSelector::loadFeedBlocks: got feed blocks:', blockList)
        blocks.value = blockList
    }
    catch (error) {
        console.error('FeedBlockSelector::loadFeedBlocks: error: ', error)
        blocks.value = null
    }
    finally {
        isLoading.value = false
    }
}

onBeforeMount(async () => {
    await loadFeedBlocks()
    console.log('Current slug: ', currentBlockSlug.value)
})
</script>

<template>
    <div class="flex">
        <div v-if="isLoading" class="flex justify-center items-center">
            <VaProgressCircle indeterminate />
        </div>
        <div v-else class="flex">
            <VaSelect
                v-if="(blocks) && (blocks.length > 0)"
                class="flex text-white"
                v-model="currentBlockSlug"
                :options="blocks" 
                text-by="title"
                value-by="slug"
            />
            <span v-else class="text-white">
                No feed blocks found
            </span>
        </div>
    </div>
</template>