import api from './apiService'

export default function useFeedBlockService() {
    const GET_FEED_BLOCKS_ACTION = 'FeedBlock/GetAllFeedBlocks'
    const GET_FEED_BLOCK_ACTION = 'FeedBlock/GetFeedBlock'

    const getFeedBlocks = async () => {
        console.log('FeedBlockService::getFeedBlocks: start get feed blocks')

        try {
            const response = await api.get(GET_FEED_BLOCKS_ACTION)
            if (response.status === 200) {
                console.log('FeedBlockService::getFeedBlocks: successfully got feed blocks')
                return response.data
            }
            else {
                console.error('FeedBlockService::getFeedBlocks: error get feed blocks. Status:', response.status)
                return null
            }
        }
        catch (error) {
            console.error('FeedBlockService::getFeedBlocks: exception raised. Error=', error)
            return null
        }
    }

    const getFeedBlock = async (slug) => {
        console.log('FeedBlockService::getFeedBlock: start get feed block. Slug:', slug)

        try {
            const params = {
                slug: slug
            }

            const response = await api.get(GET_FEED_BLOCK_ACTION, { params: params })
            if (response.status === 200) {
                console.log('FeedBlockService::getFeedBlock: successfully got feed block')
                return response.data
            }
            else {
                console.error('FeedBlockService::getFeedBlock: error get feed blocks. Status:', response.status)
                return null
            }
        }
        catch (error) {
            console.error('FeedBlockService::getFeedBlock: exception raised. Error=', error)
            return null
        }
    }

    return { 
        getFeedBlocks, getFeedBlock
    }
}