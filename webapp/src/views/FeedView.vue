<script setup>
import { ref, onBeforeMount } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import useFeedService from '@/services/feedService'
import { formateDateTime, truncateHtmlText } from '@/utils'

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
        width: '1000px'
    },
    {
        key: 'postDate',
        label: t('post.post_date_column_title'),
    },
    {
        key: 'showPost',
        label: ' '
    }
]

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
}

const showFeedPost = () => {
    const url = post.value.postUrl;
    if (url) {
        window.open(url, '_blank')
    }
}

onBeforeMount(async () => {
    await loadFeed()
})
</script>

<template>
    <div class="flex flex-col m-2">
        <va-modal 
            v-model="isShowPost"
            @close="closePost"
            blur
            hide-default-actions
            no-dismiss
        >
            <div class="flex flex-col gap-2 m-2">
                <div class="flex flex-row justify-between items-center mb-2">
                    <span class="flex text-xl">
                        {{ $t("feed.feed_title") }}: {{ feed.feed.title }}
                    </span>
                    <span class="flex">
                        {{ formateDateTime(post.postDate) }}
                    </span>
                </div>
                <div v-if="post.postImage" class="flex flex-row items-center justify-center border-2 border-indigo-500 rounded-xl shadow-md p-4 my-4">
                    <va-image
                        fit="contain"
                        class="flex h-64 w-64"
                        :src="post.postImage"
                    />                    
                </div>
                <div class="flex flex-row border-2 border-indigo-500 bg-gradient-to-br from-indigo-50 to-blue-100 rounded-xl shadow-md p-4 my-4">
                    <div 
                        v-html="post.postText"
                        class="prose max-w-none text-lg text-gray-800 leading-relaxed"
                        style="font-family: 'Merriweather', serif;"
                    />
                </div>
            </div>

            <template #footer>
                <div class="flex flex-row gap-2">
                    <va-button @click="showFeedPost">
                        {{ $t("post.open_post") }}
                    </va-button>
                    <va-button @click="isShowPost = false">
                        {{ $t("common.close_title") }}
                    </va-button>
                </div>
            </template>
        </va-modal>

        <div v-if="isLoading" class="flex justify-center items-center h-full">
            <VaProgressCircle indeterminate />
        </div>
        <div v-else class="flex flex-col m-4">
            <div v-if="feed" class="flex flex-col">
                <div
                    class="flex flex-row items-center justify-between px-8 py-6 rounded-xl shadow-lg"
                    style="background: linear-gradient(90deg, #5e6272 0%, #899498 100%); border-bottom: 4px solid #b3bebe;"
                >
                    <p
                        class="text-3xl font-extrabold tracking-wide"
                        style="color: #f5e6e8; text-shadow: 0 2px 12px rgba(80, 50, 100, 0.25); font-family: 'Merriweather', serif;"
                    >
                        {{ feed.feed.title }}
                    </p>
                    <div class="flex flex-row gap-3 items-center justify-center">
                        <va-button
                            class="flex bg-gradient-to-br from-[#767a8a] to-[#a8b4b2] shadow"
                            icon="arrow_back"
                            size="small"
                            color="#36454F"
                            round
                            @click="goBack"
                        />
                        <va-button 
                            class="flex bg-gradient-to-br from-[#7b858c] to-[#b3bebe] shadow"
                            icon="refresh"
                            size="small"
                            color="#36454F"
                            round
                            @click="loadFeed"
                        />
                        <span
                            class="flex items-center text-xs font-bold px-3 py-1 rounded-full shadow-md"
                            style="background: #b3bebe; color: #2d3440; font-family: 'Merriweather', serif;"
                        >
                            {{ $t("feed.last_sync_date_title") }}: {{ formateDateTime(feed.feedPosts.syncDate) }}
                        </span>
                    </div>
                </div>
                <div
                    class="border-t border-b-4 border-b-[#b3bebe] border-t-[#899498] my-4"
                ></div>
                
                <div class="flex flex-row items-center ml-2">
                    <div class="flex flex-col">
                        <div class="flex flex-row">
                            <p class="text-base text-[#59405c] font-medium">
                                {{ feed.feed.description }}
                            </p>
                        </div>
                    </div>
                </div>
                <div class="border-t border-b-4 border-b-[#b3bebe] border-t-[#899498] my-4"></div>

                <div class="flex flex-row justify-center border-1 rounded-xl">
                    <div class="flex flex-col">
                        <div class="flex flex-row">
                            <va-data-table
                                :striped="true"
                                class="flex rounded-xl"
                                :columns="columns"
                                :items="feed.feedPosts.posts"
                            >
                                <template #cell(postText)="{ value }">
                                    <div v-html="truncateHtmlText(value, 100)" />
                                </template>

                                <template #cell(postDate)="{ value }">
                                    {{ formateDateTime(value) }}
                                </template>

                                <template #cell(showPost)="{ rowData }">
                                    <div class="flex items-center justify-center">
                                        <va-button
                                            gradient
                                            size="small" 
                                            icon="article"
                                            @click="showPost(rowData)"
                                        />
                                    </div>
                                </template>
                            </va-data-table>
                        </div>
                    </div>
                </div>
            </div>

            <div v-else class="flex flex-col justify-center items-center p-4">
                <div class="flex flex-col justify-center items-center gap-4">
                    <va-icon
                        name="error_outline"
                        size="48px"
                        color="danger"
                    />
                    <span class="text-xl font-semibold text-red-600">
                        {{ $t('feed.feed_error_load_title') }}
                    </span>
                    <span class="text-base text-gray-500">
                        {{ $t('feed.feed_error_load_description') }}
                    </span>
                    <va-button
                        class="mt-4"
                        icon="arrow_back"
                        color="primary"
                        @click="$router.back()"
                    >
                        {{ $t('common.close_title') }}
                    </va-button>
                </div>
            </div>
        </div>
    </div>
</template>