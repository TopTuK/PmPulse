<script setup>
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const props = defineProps({
    modelValue: {
        type: Boolean,
        default: false
    }
})

const emit = defineEmits(['update:modelValue'])

const isChecked = computed({
    get: () => props.modelValue,
    set: (value) => emit('update:modelValue', value)
})

const toggle = () => {
    isChecked.value = !isChecked.value
}
</script>

<template>
    <button
        @click="toggle"
        type="button"
        class="favorite-checkbox flex items-center justify-center w-9 h-9 sm:w-10 sm:h-10 rounded-lg bg-white/20 hover:bg-white/30 backdrop-blur-sm border border-white/30 shadow-md hover:shadow-lg transition-all duration-200 hover:scale-105"
        :class="{ 'checked': isChecked }"
        :title="isChecked ? t('feed.remove_favorite_title') : t('feed.add_favorite_title')"
    >
        <!-- Filled star icon when checked -->
        <svg v-if="isChecked" class="w-5 h-5 text-yellow-300" fill="currentColor" viewBox="0 0 24 24">
            <path d="M12 2l3.09 6.26L22 9.27l-5 4.87 1.18 6.88L12 17.77l-6.18 3.25L7 14.14 2 9.27l6.91-1.01L12 2z" />
        </svg>
        <!-- Outline star icon when not checked -->
        <svg v-else class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11.049 2.927c.3-.921 1.603-.921 1.902 0l1.519 4.674a1 1 0 00.95.69h4.915c.969 0 1.371 1.24.588 1.81l-3.976 2.888a1 1 0 00-.363 1.118l1.518 4.674c.3.922-.755 1.688-1.538 1.118l-3.976-2.888a1 1 0 00-1.176 0l-3.976 2.888c-.783.57-1.838-.197-1.538-1.118l1.518-4.674a1 1 0 00-.363-1.118l-3.976-2.888c-.784-.57-.38-1.81.588-1.81h4.914a1 1 0 00.951-.69l1.519-4.674z" />
        </svg>
    </button>
</template>

<style scoped>
.favorite-checkbox {
    cursor: pointer;
    overflow: hidden;
    outline: none;
}

.favorite-checkbox:focus {
    outline: 2px solid rgba(255, 255, 255, 0.5);
    outline-offset: 2px;
}

.favorite-checkbox.checked {
    background: rgba(255, 255, 255, 0.25);
    border-color: rgba(255, 255, 255, 0.4);
}
</style>
