import { defineStore } from 'pinia'
import { ref } from 'vue'
// import Cookies from 'js-cookie'

export const useFeedBlockStore = defineStore('feedBlocksStore', () => {
    const currentBlockSlug = ref('main')

    const feedBlockFilter = ref(null)

    const showFavoriteFeeds = ref(false)
    const favoriteFeeds = ref([])

    return {
        currentBlockSlug,
        feedBlockFilter,
        showFavoriteFeeds, favoriteFeeds
    }
},
{
    persist: true,
    pick: ['currentBlockSlug', 'favoriteFeeds']
})