<script setup>
import { ref, computed, onBeforeMount, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { storeToRefs } from 'pinia'
import useFeedService from '@/services/feedService'
import signalRService from '@/services/signalRService'
import { formateDateTime, truncateHtmlText } from '@/utils'
import FeedPostModalView from './FeedPostModalView.vue'
import { useFeedBlockStore } from '@/stores/feedBlockStore'

const { t } = useI18n()

const props = defineProps({
    feed: {
        type: Object,
        required: true,
    },
})

const feedBlockStore = useFeedBlockStore()
const { favoriteFeeds } = storeToRefs(feedBlockStore)

const isFavorite = computed(() => favoriteFeeds.value.includes(props.feed.slug))

const columns = [
    {
        key: 'postText',
        label: t('post.post_column_title'),
        width: '70%'
    },
    {
        key: 'postDate',
        label: t('post.post_date_column_title'),
    }
]

const handleRowClick = (rowData) => {
    showPost(rowData)
}

const feedService = useFeedService()

const isLoading = ref(false)
const feedPosts = ref(null)

const isShowPost = ref(false)
const post = ref(null)

const router = useRouter()

const loadFeed = async () => {
    isLoading.value = true
    feedPosts.value = null

    try {
        const posts = await feedService.getBlockFeedPosts(props.feed.slug)
        if (posts) {
            console.log('BlockFeedNews::loadFeed: got feed posts. Slug:', props.feed.slug, 'LastSyncDate:', posts.lastSyncDate, 'PostsCount:', posts.posts.length)
            feedPosts.value = posts
        }
        else {
            console.error('BlockFeedNews::loadFeed: no posts received for slug:', props.feed.slug)
            feedPosts.value = null
        }
    }
    catch (error) {
        console.error('BlockFeedNews::loadFeed: error occured. Msg: ', error)
        feedPosts.value = null
    }
    finally {
        isLoading.value = false
    }
}

const viewFeed = () => {
    if (props.feed.url) {
        const url = `https://t.me/s/${props.feed.url}`
        window.open(url, '_blank')
    }
}

const showFeedPost = (url) => {
    if (url) {
        window.open(url, '_blank')
    }
}

const showFeed = () => {
    router.push({ name: 'Feed', params: { slug: props.feed.slug } })
}

const showPost = (post_obj) => {
    post.value = post_obj
    isShowPost.value = true
}

const closePost = () => {
    post.value = null
    isShowPost.value = false
}

const addRemoveFavoriteFeed = () => {
    if (favoriteFeeds.value.includes(props.feed.slug)) {
        favoriteFeeds.value = favoriteFeeds.value.filter(slug => slug !== props.feed.slug)
        console.log('BlockFeedNews::addRemoveFavoriteFeed: removed favorite feed. Slug:', props.feed.slug)
    }
    else {
        favoriteFeeds.value.push(props.feed.slug)
        console.log('BlockFeedNews::addRemoveFavoriteFeed: added favorite feed. Slug:', props.feed.slug)
    }
}

// SignalR connection handler
let feedUpdateUnsubscribe = null

const handleFeedUpdate = (data) => {
    console.log('BlockFeedNews: received FeedUpdated event', data)
    
    // Only refresh if the update is for this specific feed
    if (data && data.slug === props.feed.slug) {
        console.log('BlockFeedNews: feed update matches current feed, refreshing posts...')
        loadFeed()
    }
}

onBeforeMount(async () => {
    await loadFeed()
    
    // Start SignalR connection and listen for feed updates
    try {
        await signalRService.start()
        
        // Subscribe to feed update events
        feedUpdateUnsubscribe = signalRService.on('FeedUpdated', handleFeedUpdate)
        
        console.log('BlockFeedNews: SignalR connection established and listening for feed updates')
    } catch (error) {
        console.error('BlockFeedNews: error connecting to SignalR', error)
        // Continue even if SignalR connection fails - feed will still work via manual refresh
    }
})

onUnmounted(() => {
    // Clean up SignalR listener
    if (feedUpdateUnsubscribe) {
        feedUpdateUnsubscribe()
        feedUpdateUnsubscribe = null
    }
})
</script>

<template>
    <div class="flex-1 rounded-xl overflow-hidden shadow-xl bg-gradient-to-br from-white to-gray-50/50 border border-gray-200/50 backdrop-blur-sm">
        <FeedPostModalView
            v-model="isShowPost"
            :post="post"
            :feed-title="props.feed.title"
            @close="closePost"
            @open-post="showFeedPost"
        />

        <!-- Header Section with Gradient -->
        <div 
            class="relative overflow-hidden"
            style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border-bottom: 3px solid rgba(255, 255, 255, 0.2);"
        >
            <div class="absolute inset-0 opacity-10" style="background-image: radial-gradient(circle at 20% 50%, rgba(255, 255, 255, 0.3) 0%, transparent 50%), radial-gradient(circle at 80% 80%, rgba(255, 255, 255, 0.3) 0%, transparent 50%);"></div>
            
            <div class="relative px-4 sm:px-6 py-4 sm:py-5">
                <div class="flex w-full flex-col md:flex-row md:justify-between md:items-center gap-4">
                    <!-- Title with Icon -->
                    <div class="flex items-center gap-3">
                        <div class="flex items-center justify-center w-10 h-10 sm:w-12 sm:h-12 rounded-xl bg-white/20 backdrop-blur-sm border border-white/30 shadow-lg">
                            <svg class="w-6 h-6 sm:w-7 sm:h-7 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 20H5a2 2 0 01-2-2V6a2 2 0 012-2h10a2 2 0 012 2v1m2 13a2 2 0 01-2-2V7m2 13a2 2 0 002-2V9a2 2 0 00-2-2h-2m-4-3H9M7 16h6M7 8h6v4H7V8z" />
                            </svg>
                        </div>
                        <h2 class="text-xl sm:text-2xl font-bold text-white break-words drop-shadow-lg">
                            {{ props.feed.title }}
                        </h2>
                    </div>
                    
                    <!-- Action Buttons -->
                    <div class="flex flex-row gap-2 justify-start md:justify-end feed-action-buttons">
                        <button
                            @click="loadFeed"
                            class="flex items-center justify-center w-9 h-9 sm:w-10 sm:h-10 rounded-lg bg-white/20 hover:bg-white/30 backdrop-blur-sm border border-white/30 shadow-md hover:shadow-lg transition-all duration-200 hover:scale-105 feed-action-button"
                            :title="$t('feed.refresh_title')"
                        >
                            <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
                            </svg>
                        </button>
                        <button
                            @click="addRemoveFavoriteFeed"
                            class="flex items-center justify-center w-9 h-9 sm:w-10 sm:h-10 rounded-lg bg-white/20 hover:bg-white/30 backdrop-blur-sm border border-white/30 shadow-md hover:shadow-lg transition-all duration-200 hover:scale-105 feed-action-button"
                            :title="isFavorite ? $t('feed.remove_favorite_title') : $t('feed.add_favorite_title')"
                        >
                            <!-- Filled star icon when in favorites -->
                            <svg v-if="isFavorite" class="w-5 h-5 text-yellow-300" fill="currentColor" viewBox="0 0 24 24">
                                <path d="M12 2l3.09 6.26L22 9.27l-5 4.87 1.18 6.88L12 17.77l-6.18 3.25L7 14.14 2 9.27l6.91-1.01L12 2z" />
                            </svg>
                            <!-- Outline star icon when not in favorites -->
                            <svg v-else class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11.049 2.927c.3-.921 1.603-.921 1.902 0l1.519 4.674a1 1 0 00.95.69h4.915c.969 0 1.371 1.24.588 1.81l-3.976 2.888a1 1 0 00-.363 1.118l1.518 4.674c.3.922-.755 1.688-1.538 1.118l-3.976-2.888a1 1 0 00-1.176 0l-3.976 2.888c-.783.57-1.838-.197-1.538-1.118l1.518-4.674a1 1 0 00-.363-1.118l-3.976-2.888c-.784-.57-.38-1.81.588-1.81h4.914a1 1 0 00.951-.69l1.519-4.674z" />
                            </svg>
                        </button>
                        <button
                            @click="viewFeed"
                            class="flex items-center justify-center w-9 h-9 sm:w-10 sm:h-10 rounded-lg bg-white/20 hover:bg-white/30 backdrop-blur-sm border border-white/30 shadow-md hover:shadow-lg transition-all duration-200 hover:scale-105 feed-action-button"
                            :title="$t('feed.open_in_telegram_title')"
                        >
                            <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14" />
                            </svg>
                        </button>
                        <button
                            @click="showFeed"
                            class="flex items-center justify-center w-9 h-9 sm:w-10 sm:h-10 rounded-lg bg-white/20 hover:bg-white/30 backdrop-blur-sm border border-white/30 shadow-md hover:shadow-lg transition-all duration-200 hover:scale-105 feed-action-button"
                            :title="$t('feed.view_full_feed_title')"
                        >
                            <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                            </svg>
                        </button>
                    </div>
                </div>
            </div>
        </div>

        <!-- Content Section -->
        <div class="p-4 sm:p-6 bg-white/50 backdrop-blur-sm">
            <!-- Loading State -->
            <div v-if="isLoading" class="flex flex-col justify-center items-center py-12">
                <div class="relative">
                    <va-progress-circle 
                        indeterminate 
                        size="large"
                        :thickness="0.25"
                    />
                    <div class="absolute inset-0 flex items-center justify-center">
                        <div class="w-8 h-8 rounded-full bg-gradient-to-br from-blue-500 to-purple-600 animate-pulse"></div>
                    </div>
                </div>
                <p class="mt-4 text-sm font-medium text-gray-600">{{ $t('feed.loading_posts') }}</p>
            </div>

            <!-- Content State -->
            <div v-else class="flex flex-col w-full">
                <!-- Feed Posts -->
                <div v-if="feedPosts && feedPosts.posts" class="flex flex-col w-full">
                    <!-- Last Sync Badge -->
                    <div class="flex items-center gap-2 mb-4">
                        <div class="flex items-center gap-2 px-3 py-1.5 rounded-lg bg-gradient-to-r from-blue-50 to-purple-50 border border-blue-200/50 shadow-sm">
                            <svg class="w-4 h-4 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                            <span class="text-xs sm:text-sm font-semibold text-gray-700">
                                {{ $t("feed.last_sync_date_title") }}: 
                                <span class="text-blue-600">{{ formateDateTime(feedPosts.lastSyncDate) }}</span>
                            </span>
                        </div>
                    </div>

                    <!-- Data Table Container -->
                    <div class="rounded-xl overflow-hidden lg:overflow-x-hidden border border-gray-200/50 shadow-lg bg-white/80 backdrop-blur-sm">
                        <div class="overflow-hidden lg:overflow-x-hidden w-full max-w-full">
                            <va-data-table
                                :striped="true"
                                class="feed-table-modern w-full"
                                :columns="columns"
                                :items="feedPosts.posts"
                            >
                                <template #cell(postText)="{ value, rowData }">
                                    <div 
                                        @click="handleRowClick(rowData)"
                                        class="py-2 px-3 w-full cursor-pointer"
                                    >
                                        <div 
                                            class="line-clamp-2 text-sm text-gray-700 leading-relaxed"
                                            v-html="truncateHtmlText(value, 50)"
                                        />
                                    </div>
                                </template>

                                <template #cell(postDate)="{ value, rowData }">
                                    <div 
                                        @click="handleRowClick(rowData)"
                                        class="py-2 px-3 cursor-pointer"
                                    >
                                        <span class="inline-flex items-center gap-1.5 px-2 py-1 rounded-md bg-gray-100 text-xs font-medium text-gray-700 whitespace-nowrap">
                                            <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                                            </svg>
                                            {{ formateDateTime(value) }}
                                        </span>
                                    </div>
                                </template>
                            </va-data-table>
                        </div>
                    </div>
                </div>

                <!-- Error State -->
                <div v-else class="flex flex-col justify-center items-center p-8 sm:p-12 rounded-xl bg-gradient-to-br from-red-50 to-orange-50 border border-red-200/50">
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
    </div>
</template>

<style scoped>
:deep(.feed-table-modern .va-data-table__table) {
    background: transparent !important;
    width: 100% !important;
    table-layout: auto !important;
}

:deep(.feed-table-modern .va-data-table__table thead) {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%) !important;
}

:deep(.feed-table-modern .va-data-table__table thead th) {
    color: white !important;
    font-weight: 600 !important;
    padding: 1rem !important;
    border-bottom: 2px solid rgba(255, 255, 255, 0.2) !important;
}

:deep(.feed-table-modern .va-data-table__table tbody tr) {
    transition: all 0.2s ease-in-out !important;
}

:deep(.feed-table-modern .va-data-table__table tbody tr:hover) {
    background: rgba(102, 126, 234, 0.1) !important;
    transform: scale(1.01) !important;
}

:deep(.feed-table-modern .va-data-table__table tbody td) {
    border-bottom: 1px solid rgba(0, 0, 0, 0.05) !important;
    word-wrap: break-word !important;
    overflow-wrap: break-word !important;
}

:deep(.feed-table-modern .va-data-table) {
    overflow-x: auto !important;
    width: 100% !important;
    max-width: 100% !important;
}

/* Disable horizontal scrollbar on large devices */
@media (min-width: 1024px) {
    :deep(.feed-table-modern .va-data-table) {
        overflow-x: hidden !important;
    }
    
    :deep(.feed-table-modern .va-data-table__table-wrapper) {
        overflow-x: hidden !important;
    }
    
    :deep(.feed-table-modern .va-data-table__table) {
        width: 100% !important;
        max-width: 100% !important;
        table-layout: fixed !important;
    }
    
    /* Ensure table cells wrap content properly on large screens */
    :deep(.feed-table-modern .va-data-table__table tbody td) {
        word-wrap: break-word !important;
        overflow-wrap: break-word !important;
        overflow: hidden !important;
    }
    
    /* Post text column - allow wrapping */
    :deep(.feed-table-modern .va-data-table__table tbody td:first-child) {
        word-wrap: break-word !important;
        overflow-wrap: break-word !important;
        max-width: 100% !important;
    }
    
    /* Date column - prevent overflow */
    :deep(.feed-table-modern .va-data-table__table tbody td:last-child) {
        white-space: nowrap !important;
        overflow: hidden !important;
        text-overflow: ellipsis !important;
    }
}

.feed-action-buttons {
    overflow-y: hidden !important;
}

.feed-action-button {
    overflow-y: hidden !important;
    overflow-x: hidden !important;
}
</style>