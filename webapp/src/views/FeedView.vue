<script setup>
import { ref, onBeforeMount } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import useFeedService from '@/services/feedService'
import { formateDateTime, truncateHtmlText } from '@/utils'
import FeedPostModalView from '@/components/FeedPostModalView.vue'

const { t } = useI18n()

const route = useRoute()
const slug = route.params.slug

const router = useRouter()

const feedService = useFeedService()

const isLoading = ref(false)
const feed = ref(null)

const isShowPost = ref(false)
const post = ref(null)

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

const loadFeed = async () => {
    isLoading.value = true
    try {
        feed.value = await feedService.getFeedPosts(slug)
    }
    catch (error) {
        console.error('FeedView::loadFeed: error: ', error)
    }
    finally {
        isLoading.value = false
    }
}

const goBack = () => {
    router.back()
}

const showPost = (post_obj) => {
    post.value = post_obj
    isShowPost.value = true
}

const closePost = () => {
    post.value = null
    isShowPost.value = false
}

const showFeedPost = (url) => {
    if (url) {
        window.open(url, '_blank')
    }
}

onBeforeMount(async () => {
    await loadFeed()
})
</script>

<template>
    <div class="flex flex-col w-full min-h-screen bg-gradient-to-br from-gray-50 via-gray-100 to-gray-50">
        <FeedPostModalView
            v-model="isShowPost"
            :post="post"
            :feed-title="feed?.feed?.title || ''"
            @close="closePost"
            @open-post="showFeedPost"
        />

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
            <p class="mt-6 text-lg font-medium text-gray-600">Loading feed posts...</p>
        </div>

        <!-- Content State -->
        <div v-else class="flex flex-col w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6 sm:py-8">
            <div v-if="feed" class="flex flex-col w-full">
                <!-- Header Section with Gradient -->
                <div 
                    class="relative overflow-hidden rounded-t-xl mb-6"
                    style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border-bottom: 4px solid rgba(255, 255, 255, 0.2);"
                >
                    <div class="absolute inset-0 opacity-10" style="background-image: radial-gradient(circle at 20% 50%, rgba(255, 255, 255, 0.3) 0%, transparent 50%), radial-gradient(circle at 80% 80%, rgba(255, 255, 255, 0.3) 0%, transparent 50%);"></div>
                    
                    <div class="relative px-4 sm:px-6 lg:px-8 py-6 sm:py-8">
                        <div class="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4 sm:gap-6">
                            <!-- Title and Description -->
                            <div class="flex-1">
                                <div class="flex items-center gap-3 mb-3">
                                    <div class="flex items-center justify-center w-10 h-10 sm:w-12 sm:h-12 rounded-xl bg-white/20 backdrop-blur-sm border border-white/30 shadow-lg">
                                        <svg class="w-6 h-6 sm:w-7 sm:h-7 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 20H5a2 2 0 01-2-2V6a2 2 0 012-2h10a2 2 0 012 2v1m2 13a2 2 0 01-2-2V7m2 13a2 2 0 002-2V9a2 2 0 00-2-2h-2m-4-3H9M7 16h6M7 8h6v4H7V8z" />
                                        </svg>
                                    </div>
                                    <h1 class="text-2xl sm:text-3xl lg:text-4xl font-bold text-white drop-shadow-lg leading-tight">
                                        {{ feed.feed.title }}
                                    </h1>
                                </div>
                                <p class="text-sm sm:text-base text-white/90 ml-[3.5rem] sm:ml-[4rem] max-w-2xl leading-relaxed">
                                    {{ feed.feed.description }}
                                </p>
                            </div>
                            
                            <!-- Action Buttons -->
                            <div class="flex flex-row gap-2 items-center">
                                <button
                                    @click="goBack"
                                    class="flex items-center justify-center w-9 h-9 sm:w-10 sm:h-10 rounded-lg bg-white/20 hover:bg-white/30 backdrop-blur-sm border border-white/30 shadow-md hover:shadow-lg transition-all duration-200 hover:scale-105"
                                    title="Go Back"
                                >
                                    <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
                                    </svg>
                                </button>
                                <button
                                    @click="loadFeed"
                                    class="flex items-center justify-center w-9 h-9 sm:w-10 sm:h-10 rounded-lg bg-white/20 hover:bg-white/30 backdrop-blur-sm border border-white/30 shadow-md hover:shadow-lg transition-all duration-200 hover:scale-105"
                                    title="Refresh"
                                >
                                    <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
                                    </svg>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Last Sync Badge -->
                <div class="flex items-center gap-2 mb-6">
                    <div class="flex items-center gap-2 px-4 py-2 rounded-lg bg-gradient-to-r from-blue-50 to-purple-50 border border-blue-200/50 shadow-sm">
                        <svg class="w-4 h-4 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                        </svg>
                        <span class="text-xs sm:text-sm font-semibold text-gray-700">
                            {{ $t("feed.last_sync_date_title") }}: 
                            <span class="text-blue-600">{{ formateDateTime(feed.feedPosts.syncDate) }}</span>
                        </span>
                    </div>
                </div>

                <!-- Data Table Container -->
                <div class="rounded-xl overflow-hidden border border-gray-200/50 shadow-lg bg-white/80 backdrop-blur-sm">
                    <div class="overflow-hidden w-full">
                        <va-data-table
                            :striped="true"
                            class="feed-table-modern w-full"
                            :columns="columns"
                            :items="feed.feedPosts.posts"
                        >
                            <template #cell(postText)="{ value, rowData }">
                                <div 
                                    @click="handleRowClick(rowData)"
                                    class="py-3 px-4 w-full cursor-pointer"
                                >
                                    <div 
                                        class="line-clamp-3 text-sm text-gray-700 leading-relaxed"
                                        v-html="truncateHtmlText(value, 100)"
                                    />
                                </div>
                            </template>

                            <template #cell(postDate)="{ value, rowData }">
                                <div 
                                    @click="handleRowClick(rowData)"
                                    class="py-3 px-4 cursor-pointer"
                                >
                                    <span class="inline-flex items-center gap-1.5 px-3 py-1.5 rounded-md bg-gray-100 text-xs font-medium text-gray-700 whitespace-nowrap">
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
                        <p class="text-sm sm:text-base text-red-600/80 max-w-md mb-4">
                            {{ $t('feed.feed_error_load_description') }}
                        </p>
                        <button
                            @click="$router.back()"
                            class="flex items-center justify-center gap-2 px-4 py-2 rounded-lg bg-gradient-to-br from-blue-500 to-purple-600 hover:from-blue-600 hover:to-purple-700 text-white shadow-md hover:shadow-lg transition-all duration-200 font-medium"
                        >
                            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
                            </svg>
                            {{ $t('common.close_title') }}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
/* Feed Table Modern Styling */
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
    overflow-x: hidden !important;
    width: 100% !important;
}

/* Prevent horizontal overflow */
:deep(*) {
    max-width: 100%;
    box-sizing: border-box;
}
</style>