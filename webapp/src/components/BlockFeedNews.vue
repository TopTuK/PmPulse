<script setup>
import { ref, onBeforeMount } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import useFeedService from '@/services/feedService'
import { formateDateTime, truncateHtmlText } from '@/utils'

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
        width: '400px'
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

const showFeedPost = () => {
    const url = post.value.postUrl;
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
}

onBeforeMount(async () => {
    await loadFeed()
})
</script>

<template>
    <div class="flex-1 border border-gray-400">
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
                        {{ $t("feed.feed_title") }}: {{ props.feed.title }}
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

        <va-card>
            <va-card-title>
                <div class="flex w-full flex-row justify-between items-center">
                    <span class="flex text-xl font-bold">
                        {{ props.feed.title }}
                    </span>
                    <div class="flex flex-row gap-2">
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
                <div class="flex flex-row justify-center items-center">
                    <div v-if="isLoading" class="flex justify-center items-center">
                        <va-progress-circle indeterminate />
                    </div>
                    <div v-else class="flex flex-row justify-between items-center">
                        <div v-if="feedPosts" class="flex flex-col">
                            <div class="flex flex-row">
                                <span class="text-sm font-bold">
                                    {{ $t("feed.last_sync_date_title") }}: {{ formateDateTime(feedPosts.syncDate) }}
                                </span>
                            </div>
                            <div class="flex flex-row">
                                <va-data-table
                                    :striped="true"
                                    class="flex"
                                    :columns="columns"
                                    :items="feedPosts.posts"
                                >
                                    <template #cell(postText)="{ value }">
                                        <div v-html="truncateHtmlText(value, 45)" />
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

                        <div v-else class="flex flex-col justify-center items-center p-4">
                            <div class="flex flex-row gap-4 justify-center items-center">
                                <va-icon name="error_outline" size="36px" color="danger" />
                                <span class="mt-2 text-base font-semibold text-red-600">
                                    {{ $t(feed.feed_error_load_title) }}
                                </span>
                            </div>
                            <div class="flex flex-row justify-center items-center">
                                <span class="text-sm text-gray-500 mt-1">
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