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
        class="feed-modal-modern"
        max-width="900px"
    >
        <!-- Modal Header -->
        <div 
            class="relative overflow-hidden rounded-t-xl"
            style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border-bottom: 3px solid rgba(255, 255, 255, 0.2);"
        >
            <div class="absolute inset-0 opacity-10" style="background-image: radial-gradient(circle at 20% 50%, rgba(255, 255, 255, 0.3) 0%, transparent 50%), radial-gradient(circle at 80% 80%, rgba(255, 255, 255, 0.3) 0%, transparent 50%);"></div>
            
            <div class="relative px-4 sm:px-6 py-4 sm:py-5">
                <!-- Close Button -->
                <div class="flex justify-end mb-3">
                    <button
                        @click="handleClose"
                        class="flex items-center justify-center w-8 h-8 sm:w-9 sm:h-9 rounded-lg bg-white/20 hover:bg-white/30 backdrop-blur-sm border border-white/30 shadow-md hover:shadow-lg transition-all duration-200 hover:scale-110"
                        title="Close"
                    >
                        <svg class="w-4 h-4 sm:w-5 sm:h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                        </svg>
                    </button>
                </div>

                <!-- Title and Date -->
                <div class="flex flex-col gap-2 sm:gap-3">
                    <div class="flex items-center gap-2 sm:gap-3">
                        <div class="flex items-center justify-center w-8 h-8 sm:w-10 sm:h-10 rounded-lg bg-white/20 backdrop-blur-sm border border-white/30 shadow-lg">
                            <svg class="w-4 h-4 sm:w-5 sm:h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 20H5a2 2 0 01-2-2V6a2 2 0 012-2h10a2 2 0 012 2v1m2 13a2 2 0 01-2-2V7m2 13a2 2 0 002-2V9a2 2 0 00-2-2h-2m-4-3H9M7 16h6M7 8h6v4H7V8z" />
                            </svg>
                        </div>
                        <h2 class="text-lg sm:text-xl lg:text-2xl font-bold text-white drop-shadow-lg leading-tight flex-1">
                            {{ feedTitle }}
                        </h2>
                    </div>
                    
                    <div v-if="post" class="flex items-center gap-2 ml-[3rem] sm:ml-[3.5rem]">
                        <svg class="w-4 h-4 text-white/90" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                        </svg>
                        <span class="text-xs sm:text-sm text-white/90 font-medium">
                            {{ formateDateTime(post.postDate) }}
                        </span>
                    </div>
                </div>
            </div>
        </div>

        <!-- Modal Content -->
        <div class="flex flex-col bg-white overflow-y-auto max-h-[calc(100vh-280px)] sm:max-h-[calc(100vh-300px)] md:max-h-[calc(100vh-320px)]">
            <!-- Post Image -->
            <div 
                v-if="post && post.postImage" 
                class="flex items-center justify-center bg-gradient-to-br from-gray-50 to-gray-100 p-3 sm:p-4 md:p-6 border-b border-gray-200"
            >
                <div class="relative w-full max-w-full rounded-xl overflow-hidden shadow-xl border border-gray-200/50 bg-white">
                    <va-image
                        fit="contain"
                        class="w-full h-auto max-h-[300px] sm:max-h-[400px] md:max-h-[500px] object-contain"
                        :src="post.postImage"
                    />
                </div>
            </div>

            <!-- Post Content -->
            <div v-if="post" class="flex flex-col p-4 sm:p-6 md:p-8">
                <div class="rounded-xl bg-gradient-to-br from-blue-50 via-purple-50 to-blue-50 border border-blue-200/50 shadow-sm p-4 sm:p-6 md:p-8">
                    <div 
                        v-html="post.postText"
                        class="prose prose-sm sm:prose-base md:prose-lg max-w-none text-gray-800 leading-relaxed break-words feed-post-content"
                        style="font-family: 'Merriweather', serif;"
                    />
                </div>
            </div>

            <!-- Empty State -->
            <div v-else class="flex flex-col items-center justify-center p-8 sm:p-12 md:p-16">
                <div class="flex items-center justify-center w-16 h-16 sm:w-20 sm:h-20 rounded-full bg-gray-100 border-4 border-gray-200 mb-4">
                    <svg class="w-8 h-8 sm:w-10 sm:h-10 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                    </svg>
                </div>
                <p class="text-sm sm:text-base text-gray-600 font-medium">No post content available</p>
            </div>
        </div>

        <!-- Modal Footer -->
        <div 
            class="relative overflow-hidden rounded-b-xl border-t border-gray-200 bg-gradient-to-r from-gray-50 to-gray-100 px-4 sm:px-6 py-4 sm:py-5"
        >
            <div class="flex flex-col sm:flex-row gap-3 sm:gap-4 justify-end">
                <button
                    v-if="post && post.postUrl"
                    @click="showFeedPost"
                    class="flex items-center justify-center gap-2 px-4 sm:px-6 py-2.5 sm:py-3 rounded-lg bg-gradient-to-br from-blue-500 to-purple-600 hover:from-blue-600 hover:to-purple-700 text-white shadow-md hover:shadow-lg transition-all duration-200 font-medium text-sm sm:text-base w-full sm:w-auto"
                >
                    <svg class="w-4 h-4 sm:w-5 sm:h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14" />
                    </svg>
                    <span>{{ $t("post.open_post") }}</span>
                </button>
                <button
                    @click="handleClose"
                    class="flex items-center justify-center gap-2 px-4 sm:px-6 py-2.5 sm:py-3 rounded-lg bg-white hover:bg-gray-50 border border-gray-300 text-gray-700 shadow-sm hover:shadow-md transition-all duration-200 font-medium text-sm sm:text-base w-full sm:w-auto"
                >
                    <svg class="w-4 h-4 sm:w-5 sm:h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                    </svg>
                    <span>{{ $t("common.close_title") }}</span>
                </button>
            </div>
        </div>
    </va-modal>
</template>

<style scoped>
/* Modal Styling */
:deep(.feed-modal-modern) {
    border-radius: 0.75rem !important;
    overflow: hidden !important;
}

:deep(.feed-modal-modern .va-modal__content) {
    padding: 0 !important;
    border-radius: 0.75rem !important;
}

:deep(.feed-modal-modern .va-modal__container) {
    padding: 1rem !important;
}

/* Post Content Styling */
.feed-post-content :deep(p) {
    margin-top: 0.75rem;
    margin-bottom: 0.75rem;
}

.feed-post-content :deep(p:first-child) {
    margin-top: 0;
}

.feed-post-content :deep(p:last-child) {
    margin-bottom: 0;
}

.feed-post-content :deep(h1),
.feed-post-content :deep(h2),
.feed-post-content :deep(h3),
.feed-post-content :deep(h4),
.feed-post-content :deep(h5),
.feed-post-content :deep(h6) {
    font-weight: 700;
    margin-top: 1.5rem;
    margin-bottom: 0.75rem;
    color: #1f2937;
}

.feed-post-content :deep(a) {
    color: #667eea;
    text-decoration: underline;
}

.feed-post-content :deep(a:hover) {
    color: #764ba2;
}

.feed-post-content :deep(ul),
.feed-post-content :deep(ol) {
    margin-top: 0.75rem;
    margin-bottom: 0.75rem;
    padding-left: 1.5rem;
}

.feed-post-content :deep(li) {
    margin-top: 0.5rem;
    margin-bottom: 0.5rem;
}

.feed-post-content :deep(img) {
    max-width: 100%;
    height: auto;
    border-radius: 0.5rem;
    margin: 1rem 0;
}

.feed-post-content :deep(blockquote) {
    border-left: 4px solid #667eea;
    padding-left: 1rem;
    margin: 1rem 0;
    font-style: italic;
    color: #4b5563;
}

.feed-post-content :deep(code) {
    background-color: #f3f4f6;
    padding: 0.125rem 0.375rem;
    border-radius: 0.25rem;
    font-size: 0.875em;
}

.feed-post-content :deep(pre) {
    background-color: #f3f4f6;
    padding: 1rem;
    border-radius: 0.5rem;
    overflow-x: auto;
    margin: 1rem 0;
}

/* Responsive adjustments */
@media (max-width: 640px) {
    :deep(.feed-modal-modern .va-modal__container) {
        padding: 0.5rem !important;
        width: calc(100% - 1rem) !important;
        max-width: 100% !important;
    }
}
</style>

