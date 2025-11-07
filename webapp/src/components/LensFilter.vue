<script setup>
import { ref, nextTick, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { storeToRefs } from 'pinia'
import { useFeedBlockStore } from '@/stores/feedBlockStore'

const { t } = useI18n()

const props = defineProps({
    placeholder: {
        type: String,
        default: ''
    }
})

const feedBlockStore = useFeedBlockStore()
const { feedBlockFilter } = storeToRefs(feedBlockStore)

const computedPlaceholder = computed(() => {
    return props.placeholder || t('search.placeholder')
})

const isExpanded = ref(false)
const inputRef = ref(null)

const toggleExpand = async () => {
    isExpanded.value = !isExpanded.value
    
    if (isExpanded.value) {
        // Wait for DOM update, then focus input
        await nextTick()
        inputRef.value?.focus()
    }
}

const handleBlur = () => {
    // Only collapse if input is empty
    if (!feedBlockFilter.value || feedBlockFilter.value.trim() === '') {
        isExpanded.value = false
    }
}
</script>

<template>
    <div class="relative">
        <!-- Lens Icon Button (always visible) -->
        <button
            @click="toggleExpand"
            class="flex items-center justify-center w-10 h-10 rounded-lg bg-white/20 hover:bg-white/30 backdrop-blur-sm border border-white/30 shadow-md hover:shadow-lg transition-all duration-200 hover:scale-105 focus:outline-none focus:ring-2 focus:ring-white/50"
            type="button"
            :aria-label="isExpanded ? $t('search.close_search') : $t('search.open_search')"
        >
            <svg 
                class="w-5 h-5 text-white" 
                fill="none" 
                stroke="currentColor" 
                viewBox="0 0 24 24"
            >
                <path 
                    stroke-linecap="round" 
                    stroke-linejoin="round" 
                    stroke-width="2" 
                    d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" 
                />
            </svg>
        </button>

        <!-- Tooltip Input Container -->
        <div
            v-if="isExpanded"
            class="absolute top-full left-0 mt-2 z-50 flex items-center gap-2 min-w-[300px] p-2 rounded-lg bg-white border border-gray-200 shadow-lg"
        >
            <!-- Lens Icon in tooltip -->
            <div class="flex items-center justify-center w-10 h-10 rounded-lg bg-gray-100 border border-gray-200 shadow-sm flex-shrink-0">
                <svg 
                    class="w-5 h-5 text-gray-600" 
                    fill="none" 
                    stroke="currentColor" 
                    viewBox="0 0 24 24"
                >
                    <path 
                        stroke-linecap="round" 
                        stroke-linejoin="round" 
                        stroke-width="2" 
                        d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" 
                    />
                </svg>
            </div>

            <!-- Input Field -->
            <input
                ref="inputRef"
                v-model="feedBlockFilter"
                @blur="handleBlur"
                @keydown.escape="isExpanded = false"
                type="text"
                :placeholder="computedPlaceholder"
                class="flex-1 h-10 px-4 rounded-lg bg-gray-50 border border-gray-200 text-gray-900 placeholder-gray-400 shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-all duration-200"
            />
        </div>
    </div>
</template>

<style scoped>
/* Ensure smooth transitions */
input::placeholder {
    color: rgba(255, 255, 255, 0.6);
}

input:focus::placeholder {
    color: rgba(255, 255, 255, 0.4);
}
</style>