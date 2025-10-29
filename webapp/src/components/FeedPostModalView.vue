<script setup>
import { useI18n } from 'vue-i18n'
import { formateDateTime } from '@/utils'

const { t } = useI18n()

const props = defineProps({
    modelValue: {
        type: Boolean,
        required: true,
    },
    post: {
        type: Object,
        default: null,
    },
    feedTitle: {
        type: String,
        required: true,
    },
})

const emit = defineEmits(['update:modelValue', 'close', 'openPost'])

const showFeedPost = () => {
    if (props.post && props.post.postUrl) {
        emit('openPost', props.post.postUrl)
    }
}

const closeModal = (value = false) => {
    emit('update:modelValue', value)
    if (!value) {
        emit('close')
    }
}

const handleClose = () => {
    closeModal(false)
}
</script>

<template>
    <va-modal 
        :model-value="props.modelValue"
        @update:model-value="closeModal"
        blur
        hide-default-actions
        no-dismiss
    >
        <div class="flex flex-col gap-2">
            <div class="flex flex-row justify-between items-center mb-2">
                <span class="flex text-xl">
                    {{ $t("feed.feed_title") }}: {{ feedTitle }}
                </span>
                <span v-if="post" class="flex">
                    {{ formateDateTime(post.postDate) }}
                </span>
            </div>
            <div v-if="post && post.postImage" class="flex flex-row items-center justify-center border-2 border-indigo-500 rounded-xl shadow-md p-4 my-4">
                <va-image
                    fit="contain"
                    class="flex h-64 w-64"
                    :src="post.postImage"
                />                    
            </div>
            <div v-if="post" class="flex flex-row border-2 border-indigo-500 bg-gradient-to-br from-indigo-50 to-blue-100 rounded-xl shadow-md p-4 my-4">
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
                <va-button @click="handleClose">
                    {{ $t("common.close_title") }}
                </va-button>
            </div>
        </template>
    </va-modal>
</template>

