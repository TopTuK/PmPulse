<script setup>
import { ref, onBeforeMount, watch, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import useFeedService from '@/services/feedService'
import { formateDateTime, truncateHtmlText } from '@/utils'
import FeedPostModalView from '@/components/FeedPostModalView.vue'

const { t } = useI18n()
const feedService = useFeedService()
const isLoading = ref(false)
const digest = ref(null)

// Update meta tags when digest is loaded
function updateDigestMetaTags() {
    if (digest.value && digest.value.length > 0) {
        const title = t('weekly_digest.title')
        // Use base description from router meta, but can be enhanced with dynamic data
        const description = t('weekly_digest.description')
        
        // Update document title (router handles this, but we can enhance it)
        if (document.title === '' || !document.title.includes(title)) {
            document.title = `${title} | PM Pulse`
        }
        
        // Update meta description if router hasn't set it yet
        let metaDescription = document.querySelector('meta[name="description"]')
        if (metaDescription && (!metaDescription.getAttribute('content') || metaDescription.getAttribute('content').trim() === '')) {
            metaDescription.setAttribute('content', description)
        }
        
        // Update Open Graph tags
        updateMetaTag('og:title', `${title} | PM Pulse`)
        updateMetaTag('og:description', description)
        updateMetaTag('og:url', window.location.href)
        updateMetaTag('og:type', 'website')
        
        // Update Twitter Card tags
        updateMetaTag('twitter:card', 'summary_large_image')
        updateMetaTag('twitter:title', `${title} | PM Pulse`)
        updateMetaTag('twitter:description', description)
        
        // Update canonical URL
        let canonical = document.querySelector('link[rel="canonical"]')
        if (canonical) {
            canonical.setAttribute('href', window.location.href)
        }
    }
}

// Helper function to update meta tags
function updateMetaTag(propertyOrName, content) {
    // Try as property first (for Open Graph)
    let tag = document.querySelector(`meta[property="${propertyOrName}"]`)
    if (!tag) {
        // Try as name (for Twitter and regular meta tags)
        tag = document.querySelector(`meta[name="${propertyOrName}"]`)
    }
    if (tag) {
        tag.setAttribute('content', content)
    } else {
        // Create new tag if it doesn't exist
        tag = document.createElement('meta')
        if (propertyOrName.startsWith('og:') || propertyOrName.startsWith('twitter:')) {
            tag.setAttribute('property', propertyOrName)
        } else {
            tag.setAttribute('name', propertyOrName)
        }
        tag.setAttribute('content', content)
        document.head.appendChild(tag)
    }
}

const isShowPost = ref(false)
const selectedPost = ref(null)
const selectedFeedTitle = ref('')

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

const handleRowClick = (rowData, feedTitle) => {
    selectedFeedTitle.value = feedTitle || t('weekly_digest.untitled_feed')
    showPost(rowData)
}

const showPost = (post_obj) => {
    selectedPost.value = post_obj
    isShowPost.value = true
}

const closePost = () => {
    selectedPost.value = null
    isShowPost.value = false
}

const showFeedPost = (url) => {
    if (url) {
        window.open(url, '_blank')
    }
}

const loadWeeklyDigest = async () => {
    isLoading.value = true

    try {
        digest.value = await feedService.getWeeklyDigest()
        console.log('WeeklyDigestView::loadWeeklyDigest: got weekly digest', digest.value)
        // Update meta tags after digest is loaded
        updateDigestMetaTags()
    }
    catch (error) {
        digest.value = null
        console.error('WeeklyDigestView::loadWeeklyDigest: error occurred', error)
    }
    finally {
        isLoading.value = false
    }
}

// Generate structured data (JSON-LD)
const structuredData = computed(() => {
    if (!digest.value || !Array.isArray(digest.value) || digest.value.length === 0) {
        return null
    }

    const items = []
    let position = 1

    digest.value.forEach((feedPosts) => {
        if (feedPosts.posts && Array.isArray(feedPosts.posts)) {
            feedPosts.posts.forEach((post) => {
                const plainText = truncateHtmlText(post.postText, 100).replace(/<[^>]*>/g, '').trim()
                if (plainText) {
                    items.push({
                        '@type': 'ListItem',
                        position: position++,
                        item: {
                            '@type': 'BlogPosting',
                            headline: plainText.substring(0, 110),
                            datePublished: new Date(post.postDate).toISOString(),
                            url: post.postUrl || '',
                            publisher: {
                                '@type': 'Organization',
                                name: feedPosts.feed?.title || t('weekly_digest.untitled_feed')
                            }
                        }
                    })
                }
            })
        }
    })

    return {
        '@context': 'https://schema.org',
        '@type': 'CollectionPage',
        name: t('weekly_digest.title'),
        description: t('weekly_digest.description'),
        url: typeof window !== 'undefined' ? window.location.href : '',
        mainEntity: {
            '@type': 'ItemList',
            numberOfItems: items.length,
            itemListElement: items
        }
    }
})

// Update structured data in head
watch(structuredData, (newData) => {
    if (typeof document === 'undefined') return

    // Remove existing structured data script
    const existingScript = document.querySelector('script[data-structured-data="weekly-digest"]')
    if (existingScript) {
        existingScript.remove()
    }

    // Add new structured data script
    if (newData) {
        const script = document.createElement('script')
        script.type = 'application/ld+json'
        script.setAttribute('data-structured-data', 'weekly-digest')
        script.textContent = JSON.stringify(newData)
        document.head.appendChild(script)
    }
}, { immediate: true })

// Watch for digest changes to update meta tags
watch(digest, () => {
    updateDigestMetaTags()
}, { deep: true })

onBeforeMount(async () => {
    await loadWeeklyDigest()
})
</script>

<template>
    <main class="flex flex-col w-full min-h-screen bg-gradient-to-br from-gray-50 via-gray-100 to-gray-50">
        <!-- Loading State -->
        <div v-if="isLoading" class="flex flex-col justify-center items-center h-screen" role="status" aria-label="Loading weekly digest">
            <div class="relative">
                <va-progress-circle 
                    indeterminate 
                    size="large"
                    :thickness="0.25"
                />
                <div class="absolute inset-0 flex items-center justify-center">
                    <div class="w-10 h-10 rounded-full bg-gradient-to-br from-blue-500 to-purple-600 animate-pulse"></div>
                </div>
            </div>
            <p class="mt-6 text-lg font-medium text-gray-600">{{ $t('feed.loading_posts') }}</p>
        </div>

        <!-- Content State -->
        <div v-else class="flex flex-col w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6 sm:py-8">
            <!-- Weekly Digest Header -->
            <header class="flex flex-col w-full mb-6 sm:mb-8">
                <div 
                    class="relative overflow-hidden rounded-t-xl"
                    style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border-bottom: 4px solid rgba(255, 255, 255, 0.2);"
                >
                    <div class="absolute inset-0 opacity-10" style="background-image: radial-gradient(circle at 20% 50%, rgba(255, 255, 255, 0.3) 0%, transparent 50%), radial-gradient(circle at 80% 80%, rgba(255, 255, 255, 0.3) 0%, transparent 50%);"></div>
                    
                    <div class="relative px-6 sm:px-8 py-6 sm:py-8">
                        <div class="flex flex-col items-center text-center">
                            <!-- Icon -->
                            <div class="flex items-center justify-center w-16 h-16 sm:w-20 sm:h-20 rounded-2xl bg-white/20 backdrop-blur-sm border border-white/30 shadow-xl mb-4" aria-hidden="true">
                                <svg class="w-10 h-10 sm:w-12 sm:h-12 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                                </svg>
                            </div>
                            
                            <!-- Title -->
                            <h1 class="text-3xl sm:text-4xl lg:text-5xl font-bold text-white mb-4 drop-shadow-lg leading-tight">
                                {{ $t('weekly_digest.title') }}
                            </h1>
                            
                            <!-- Description -->
                            <p class="text-base sm:text-lg text-white/90 max-w-3xl leading-relaxed font-medium">
                                {{ $t('weekly_digest.description') }}
                            </p>
                            
                        </div>
                    </div>
                </div>
            </header>

            <!-- Feed Posts Grid -->
            <template v-if="digest && digest.length > 0">
                <section class="flex flex-col w-full gap-6 sm:gap-8" aria-label="Weekly digest feeds">
                    <article 
                        v-for="feedPosts in digest"
                        :key="feedPosts.feed?.slug"
                        class="flex-1 rounded-xl overflow-hidden shadow-xl bg-gradient-to-br from-white to-gray-50/50 border border-gray-200/50 backdrop-blur-sm"
                        itemscope
                        itemtype="https://schema.org/Feed"
                    >
                    <!-- Feed Header -->
                    <header 
                        class="relative overflow-hidden"
                        style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border-bottom: 3px solid rgba(255, 255, 255, 0.2);"
                    >
                        <div class="absolute inset-0 opacity-10" style="background-image: radial-gradient(circle at 20% 50%, rgba(255, 255, 255, 0.3) 0%, transparent 50%), radial-gradient(circle at 80% 80%, rgba(255, 255, 255, 0.3) 0%, transparent 50%);"></div>
                        
                        <div class="relative px-4 sm:px-6 py-4 sm:py-5">
                            <div class="flex items-center gap-3">
                                <div class="flex items-center justify-center w-10 h-10 sm:w-12 sm:h-12 rounded-xl bg-white/20 backdrop-blur-sm border border-white/30 shadow-lg" aria-hidden="true">
                                    <svg class="w-6 h-6 sm:w-7 sm:h-7 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 20H5a2 2 0 01-2-2V6a2 2 0 012-2h10a2 2 0 012 2v1m2 13a2 2 0 01-2-2V7m2 13a2 2 0 002-2V9a2 2 0 00-2-2h-2m-4-3H9M7 16h6M7 8h6v4H7V8z" />
                                    </svg>
                                </div>
                                <div class="flex flex-col flex-1 min-w-0">
                                    <h2 class="text-xl sm:text-2xl font-bold text-white break-words drop-shadow-lg" itemprop="name">
                                        {{ feedPosts.feed?.title || $t('weekly_digest.untitled_feed') }}
                                    </h2>
                                    <p v-if="feedPosts.feed?.description" class="text-sm sm:text-base text-white/80 mt-1 break-words" itemprop="description">
                                        {{ feedPosts.feed.description }}
                                    </p>
                                </div>
                            </div>
                        </div>
                    </header>

                    <!-- Feed Content -->
                    <div class="p-4 sm:p-6 bg-white/50 backdrop-blur-sm">
                        <!-- Posts -->
                        <div v-if="feedPosts.posts && feedPosts.posts.length > 0" class="flex flex-col w-full">
                            <!-- Last Sync Badge -->
                            <div v-if="feedPosts.lastSyncDate" class="flex items-center gap-2 mb-4">
                                <div class="flex items-center gap-2 px-3 py-1.5 rounded-lg bg-gradient-to-r from-blue-50 to-purple-50 border border-blue-200/50 shadow-sm">
                                    <svg class="w-4 h-4 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24" aria-hidden="true">
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                                    </svg>
                                    <time :datetime="new Date(feedPosts.lastSyncDate).toISOString()" class="text-xs sm:text-sm font-semibold text-gray-700">
                                        {{ $t("feed.last_sync_date_title") }}: 
                                        <span class="text-blue-600">{{ formateDateTime(feedPosts.lastSyncDate) }}</span>
                                    </time>
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
                                        role="table"
                                        aria-label="Posts from feed"
                                    >
                                        <template #cell(postText)="{ value, rowData }">
                                            <div 
                                                @click="handleRowClick(rowData, feedPosts.feed?.title)"
                                                class="py-2 px-3 w-full cursor-pointer"
                                                role="button"
                                                tabindex="0"
                                                @keyup.enter="handleRowClick(rowData, feedPosts.feed?.title)"
                                                @keyup.space.prevent="handleRowClick(rowData, feedPosts.feed?.title)"
                                            >
                                                <div 
                                                    class="line-clamp-2 text-sm text-gray-700 leading-relaxed"
                                                    v-html="truncateHtmlText(value, 50)"
                                                />
                                            </div>
                                        </template>

                                        <template #cell(postDate)="{ value, rowData }">
                                            <div 
                                                @click="handleRowClick(rowData, feedPosts.feed?.title)"
                                                class="py-2 px-3 cursor-pointer"
                                                role="button"
                                                tabindex="0"
                                                @keyup.enter="handleRowClick(rowData, feedPosts.feed?.title)"
                                                @keyup.space.prevent="handleRowClick(rowData, feedPosts.feed?.title)"
                                            >
                                                <time :datetime="new Date(value).toISOString()" class="inline-flex items-center gap-1.5 px-2 py-1 rounded-md bg-gray-100 text-xs font-medium text-gray-700 whitespace-nowrap">
                                                    <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24" aria-hidden="true">
                                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                                                    </svg>
                                                    {{ formateDateTime(value) }}
                                                </time>
                                            </div>
                                        </template>
                                    </va-data-table>
                                </div>
                            </div>
                        </div>

                        <!-- Empty Posts State -->
                        <div v-else class="flex flex-col justify-center items-center p-8 sm:p-12 rounded-xl bg-gradient-to-br from-yellow-50 to-orange-50 border border-yellow-200/50">
                            <div class="flex flex-col items-center gap-4">
                                <div class="flex items-center justify-center w-16 h-16 rounded-full bg-yellow-100 border-4 border-yellow-200">
                                    <svg class="w-8 h-8 text-yellow-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                                    </svg>
                                </div>
                                <div class="text-center">
                                    <h3 class="text-lg sm:text-xl font-bold text-yellow-700 mb-2">
                                        {{ $t('weekly_digest.no_posts_title') }}
                                    </h3>
                                    <p class="text-sm sm:text-base text-yellow-600/80 max-w-md">
                                        {{ $t('weekly_digest.no_posts_description') }}
                                    </p>
                                </div>
                            </div>
                        </div>
                    </div>
                    </article>
                </section>
            </template>

            <!-- Empty Digest State -->
            <div v-else class="flex flex-col justify-center items-center w-full p-8 sm:p-12 rounded-xl bg-gradient-to-br from-red-50 to-orange-50 border border-red-200/50 shadow-lg">
                <div class="flex flex-col items-center gap-4">
                    <div class="flex items-center justify-center w-16 h-16 rounded-full bg-red-100 border-4 border-red-200">
                        <svg class="w-8 h-8 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                        </svg>
                    </div>
                    <div class="text-center">
                        <h3 class="text-lg sm:text-xl font-bold text-red-700 mb-2">
                            {{ $t('weekly_digest.no_digest_title') }}
                        </h3>
                        <p class="text-sm sm:text-base text-red-600/80 max-w-md">
                            {{ $t('weekly_digest.no_digest_description') }}
                        </p>
                    </div>
                </div>
            </div>
        </div>

        <!-- Post Modal -->
        <FeedPostModalView
            v-model="isShowPost"
            :post="selectedPost"
            :feed-title="selectedFeedTitle"
            @close="closePost"
            @open-post="showFeedPost"
        />
    </main>
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

/* Table Styling */
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
</style>