<script setup>
import { ref, onBeforeMount } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import useFeedService from '@/services/feedService'
import { formateDateTime, truncateHtmlText } from '@/utils'
import FeedPostModalView from './FeedPostModalView.vue'

const { t } = useI18n()

const props = defineProps({
    feed: {
        type: Object,
        required: true,
    },
})

const columns = [
    {
        key: 'postText',
        label: t('post.post_column_title'),
        width: '70%'
    },
    {
        key: 'postDate',
        label: t('post.post_date_column_title'),
    },
    {
        key: 'showPost',
        label: ' ',
        width: '10px'
    }
]

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
        feedPosts.value = await feedService.getBlockFeedPosts(props.feed.slug)
        console.log('BlockFeedNews::loadFeed: got feed posts.')
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

onBeforeMount(async () => {
    await loadFeed()
})
</script>

<template>
    <div class="flex-1 border border-gray-400">
        <FeedPostModalView
            v-model="isShowPost"
            :post="post"
            :feed-title="props.feed.title"
            @close="closePost"
            @open-post="showFeedPost"
        />

        <va-card>
            <va-card-title>
                <div class="flex w-full flex-col md:flex-row md:justify-between md:items-center gap-3">
                    <span class="flex text-lg md:text-xl font-bold break-words">
                        {{ props.feed.title }}
                    </span>
                    <div class="flex flex-row gap-2 justify-start md:justify-end">
                        <va-button
                            size="small" 
                            round
                            icon="refresh" 
                            @click="loadFeed"
                        />
                        <va-button
                            size="small" 
                            round
                            icon="open_in_new" 
                            @click="viewFeed"
                        />
                        <va-button
                            size="small" 
                            round
                            icon="web" 
                            @click="showFeed"
                        />
                    </div>
                </div>
            </va-card-title>

            <va-card-content>
                <div class="flex flex-col justify-center items-center">
                    <div v-if="isLoading" class="flex justify-center items-center py-8">
                        <va-progress-circle indeterminate />
                    </div>
                    <div v-else class="flex flex-col w-full">
                        <div v-if="feedPosts" class="flex flex-col w-full">
                            <div class="flex flex-col mb-3">
                                <span class="text-xs md:text-sm font-bold break-words">
                                    {{ $t("feed.last_sync_date_title") }}: {{ formateDateTime(feedPosts.syncDate) }}
                                </span>
                            </div>
                            <div class="flex flex-row w-full overflow-x-auto">
                                <va-data-table
                                    :striped="true"
                                    class="flex flex-1 min-w-full md:min-w-0"
                                    :columns="columns"
                                    :items="feedPosts.posts"
                                >
                                    <template #cell(postText)="{ value }">
                                        <div class="flex line-clamp-1 max-w-[200px] sm:max-w-[300px] md:max-w-none">
                                            <div v-html="truncateHtmlText(value, 45)" />
                                        </div>
                                    </template>

                                    <template #cell(postDate)="{ value }">
                                        <span class="text-xs md:text-sm whitespace-nowrap">
                                            {{ formateDateTime(value) }}
                                        </span>
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

                        <div v-else class="flex flex-col justify-center items-center p-4 md:p-6">
                            <div class="flex flex-col md:flex-row gap-2 md:gap-4 justify-center items-center">
                                <va-icon name="error_outline" size="32px" color="danger" />
                                <span class="text-sm md:text-base font-semibold text-red-600 text-center">
                                    {{ $t(feed.feed_error_load_title) }}
                                </span>
                            </div>
                            <div class="flex flex-row justify-center items-center mt-2">
                                <span class="text-xs md:text-sm text-gray-500 text-center px-2">
                                    {{ $t(feed.feed_error_load_description) }}
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
            </va-card-content>
        </va-card>
    </div>
</template>