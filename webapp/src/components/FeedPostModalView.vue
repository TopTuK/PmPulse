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
        class="flex"
    >
        <div class="flex flex-col gap-3 max-h-[85vh] sm:max-h-[80vh] overflow-y-auto">
            <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-1 sm:gap-2 mb-1 sm:mb-2">
                <span class="flex text-lg sm:text-xl leading-snug">
                    {{ $t("feed.feed_title") }}: {{ feedTitle }}
                </span>
                <span v-if="post" class="flex text-sm text-gray-600">
                    {{ formateDateTime(post.postDate) }}
                </span>
            </div>
            <div v-if="post && post.postImage" class="flex items-center justify-center border-2 border-indigo-500 rounded-xl shadow-md p-2 sm:p-4 my-2 sm:my-4">
                <va-image
                    fit="contain"
                    class="w-full max-w-xs sm:max-w-md max-h-80 sm:max-h-[28rem] object-contain"
                    :src="post.postImage"
                />                    
            </div>
            <div v-if="post" class="flex flex-col border-2 border-indigo-500 bg-gradient-to-br from-indigo-50 to-blue-100 rounded-xl shadow-md p-3 sm:p-4 my-2 sm:my-4 max-h-[60vh] ">
                <div 
                    v-html="post.postText"
                    class="prose max-w-none text-base sm:text-lg text-gray-800 leading-relaxed break-words"
                    style="font-family: 'Merriweather', serif;"
                />
            </div>
        </div>

        <template #footer>
            <div class="flex flex-col sm:flex-row gap-2 w-full">
                <va-button class="w-full sm:w-auto" @click="showFeedPost">
                    {{ $t("post.open_post") }}
                </va-button>
                <va-button class="w-full sm:w-auto" @click="handleClose">
                    {{ $t("common.close_title") }}
                </va-button>
            </div>
        </template>
    </va-modal>
</template>

