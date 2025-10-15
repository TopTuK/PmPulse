import { defineStore } from 'pinia'
import { ref } from 'vue'
import Cookies from 'js-cookie'

export const useFeedBlockStore = defineStore('feedBlocksStore', () => {
    const CURRENT_BLOCK_SLUG_COOKIE = 'block_slug'

    const currentBlockSlug = ref(Cookies.get(CURRENT_BLOCK_SLUG_COOKIE) ?? 'main')

    const setCurrentBlockSlug = (blockSlug) => {
        Cookies.set(CURRENT_BLOCK_SLUG_COOKIE, blockSlug)
        currentBlockSlug.value = blockSlug
    }
    
    return {
        currentBlockSlug, setCurrentBlockSlug
    }
})