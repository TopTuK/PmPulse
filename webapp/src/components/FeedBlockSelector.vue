<script setup>
import { ref, onBeforeMount } from 'vue'
import { storeToRefs } from 'pinia'
// import { useI18n } from 'vue-i18n'
import { useFeedBlockStore } from '@/stores/feedBlockStore'
import useFeedBlockService from '@/services/feedBlockService'
import LensFilter from '@/components/LensFilter.vue'
import FavoriteCheckbox from './FavoriteCheckbox.vue'

// const { t } = useI18n()

const feedBlockStore = useFeedBlockStore()
const { currentBlockSlug, showFavoriteFeeds } = storeToRefs(feedBlockStore)

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
    <div class="feed-block-selector w-full">
        <!-- Loading State -->
        <div v-if="isLoading" class="flex justify-center items-center py-2 sm:py-2.5 md:py-3">
            <VaProgressCircle 
                indeterminate 
                size="small"
                class="text-white"
            />
            <span class="ml-2 sm:ml-2.5 md:ml-3 text-xs sm:text-sm md:text-base text-gray-300">
                {{ $t('feed_block.loading') }}
            </span>
        </div>
        
        <!-- Select and checkbox Component -->
         <div
            v-else-if="blocks && blocks.length > 0"
            class="feed-block-select flex items-center justify-center gap-2 sm:gap-2.5 md:gap-3 flex-wrap sm:flex-nowrap"
         >
            <LensFilter class="flex-shrink-0" />
            <VaSelect
                v-model="currentBlockSlug"
                :options="blocks"
                text-by="title"
                value-by="slug"
                :placeholder="$t('feed_block.select_placeholder')"
                class="flex-1 min-w-0"
            />
            <FavoriteCheckbox 
                v-model="showFavoriteFeeds"
                class="flex-shrink-0"
            />
         </div>

        <!-- Empty State -->
        <div v-else class="flex items-center justify-center py-2 sm:py-2.5 md:py-3">
            <div class="flex items-center gap-2">
                <svg 
                    class="w-4 h-4 sm:w-4.5 sm:h-4.5 md:w-5 md:h-5 text-gray-400" 
                    fill="none" 
                    stroke="currentColor" 
                    viewBox="0 0 24 24"
                >
                    <path 
                        stroke-linecap="round" 
                        stroke-linejoin="round" 
                        stroke-width="2" 
                        d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" 
                    />
                </svg>
                <span class="text-xs sm:text-sm text-gray-400">
                    {{ $t('feed_block.no_blocks_found') }}
                </span>
            </div>
        </div>
    </div>
</template>

<style scoped>
/* Modern Select Styling */
:deep(.feed-block-select) {
    width: 100% !important;
    position: relative !important;
    z-index: 100 !important;
}

:deep(.feed-block-select .va-input-wrapper__field) {
    background: rgba(255, 255, 255, 0.1) !important;
    border: 1px solid rgba(255, 255, 255, 0.2) !important;
    border-radius: 0.5rem !important;
    padding: 0.5rem 0.75rem !important;
    transition: all 0.2s ease-in-out !important;
    min-height: 2.5rem !important;
}

:deep(.feed-block-select .va-input-wrapper__field:hover) {
    background: rgba(255, 255, 255, 0.15) !important;
    border-color: rgba(255, 255, 255, 0.3) !important;
}

:deep(.feed-block-select .va-input-wrapper__field--focused) {
    background: rgba(255, 255, 255, 0.15) !important;
    border-color: rgba(99, 102, 241, 0.5) !important;
    box-shadow: 0 0 0 3px rgba(99, 102, 241, 0.1) !important;
}

:deep(.feed-block-select .va-input-wrapper__text) {
    color: white !important;
    font-size: 0.875rem !important;
    font-weight: 500 !important;
}

:deep(.feed-block-select .va-input-wrapper__text::placeholder) {
    color: rgba(255, 255, 255, 0.5) !important;
}

/* Dropdown Icon */
:deep(.feed-block-select .va-input-wrapper__append-inner) {
    color: rgba(255, 255, 255, 0.7) !important;
}

:deep(.feed-block-select .va-input-wrapper__append-inner:hover) {
    color: white !important;
}

/* Dropdown Menu */
:deep(.feed-block-select .va-dropdown__content) {
    background: rgba(31, 41, 55, 0.98) !important;
    backdrop-filter: blur(12px) !important;
    border: 1px solid rgba(255, 255, 255, 0.1) !important;
    border-radius: 0.5rem !important;
    box-shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.5), 0 10px 10px -5px rgba(0, 0, 0, 0.2) !important;
    margin-top: 0.5rem !important;
    max-height: 16rem !important;
    overflow-y: auto !important;
    z-index: 9999 !important;
}

/* Ensure dropdown appears above header (which is z-50) */
:deep(.feed-block-select .va-dropdown__content-wrapper) {
    z-index: 9999 !important;
}

/* Dropdown Container/Wrapper */
:deep(.feed-block-select .va-dropdown) {
    z-index: 9999 !important;
}

:deep(.feed-block-select .va-dropdown__anchor) {
    z-index: 9999 !important;
}

/* Dropdown Portal - if Vuestic renders dropdown in a portal */
:global(.va-dropdown__content-wrapper) {
    z-index: 9999 !important;
}

/* Ensure dropdown overlay appears above everything */
:deep(.feed-block-select) {
    isolation: isolate !important;
}

/* Dropdown Options */
:deep(.feed-block-select .va-select-option) {
    color: rgba(255, 255, 255, 0.9) !important;
    padding: 0.75rem 1rem !important;
    transition: all 0.15s ease-in-out !important;
    cursor: pointer !important;
}

:deep(.feed-block-select .va-select-option:hover) {
    background: rgba(99, 102, 241, 0.2) !important;
    color: white !important;
}

:deep(.feed-block-select .va-select-option--selected) {
    background: rgba(99, 102, 241, 0.3) !important;
    color: white !important;
    font-weight: 600 !important;
}

/* Mobile-specific adjustments (below 640px) */
@media (max-width: 639px) {
    :deep(.feed-block-select .va-input-wrapper__field) {
        padding: 0.625rem 0.875rem !important;
        min-height: 2.75rem !important;
        font-size: 0.875rem !important;
    }
    
    :deep(.feed-block-select .va-input-wrapper__text) {
        font-size: 0.875rem !important;
    }
    
    :deep(.feed-block-select .va-dropdown__content) {
        max-height: 12rem !important;
        margin-top: 0.375rem !important;
    }
    
    :deep(.feed-block-select .va-select-option) {
        padding: 0.875rem 1rem !important;
        font-size: 0.875rem !important;
        min-height: 2.75rem !important;
    }
    
    /* Ensure select takes available space on mobile */
    :deep(.feed-block-select .va-select) {
        flex: 1 1 0% !important;
        min-width: 0 !important;
    }
}

/* Tablet-specific adjustments (640px - 1023px) */
@media (min-width: 640px) and (max-width: 1023px) {
    :deep(.feed-block-select .va-input-wrapper__field) {
        padding: 0.5rem 0.875rem !important;
        min-height: 2.5rem !important;
    }
    
    :deep(.feed-block-select .va-dropdown__content) {
        max-height: 14rem !important;
    }
}

/* Ensure full width on all screen sizes */
.feed-block-selector {
    min-width: 0 !important;
    width: 100% !important;
    position: relative !important;
    z-index: 100 !important;
}

/* Scrollbar styling for dropdown */
:deep(.feed-block-select .va-dropdown__content::-webkit-scrollbar) {
    width: 6px !important;
}

:deep(.feed-block-select .va-dropdown__content::-webkit-scrollbar-track) {
    background: rgba(255, 255, 255, 0.05) !important;
    border-radius: 3px !important;
}

:deep(.feed-block-select .va-dropdown__content::-webkit-scrollbar-thumb) {
    background: rgba(255, 255, 255, 0.2) !important;
    border-radius: 3px !important;
}

:deep(.feed-block-select .va-dropdown__content::-webkit-scrollbar-thumb:hover) {
    background: rgba(255, 255, 255, 0.3) !important;
}
</style>