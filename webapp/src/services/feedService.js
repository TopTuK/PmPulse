import api from './apiService'

export default function useFeedService() {
    const GET_BLOCK_FEED_POSTS_ACTION = 'FeedPost/GetBlockFeedPosts'
    const GET_FEED_POSTS_ACTION = 'FeedPost/GetFeedPosts'
    const GET_WEEKLY_DIGEST_ACTION = 'FeedPost/GetWeeklyDigest'

    const getBlockFeedPosts = async (slug) => {
        console.log('FeedService::getBlockFeedPosts: start get feed posts for block', slug)

        try {
            const params = {
                slug: slug
            }

            const response = await api.get(GET_BLOCK_FEED_POSTS_ACTION, { params: params })
            if (response.status === 200) {
                console.log('FeedBlockService::getFeedBlock: successfully got feed posts')
                return response.data
            }
            else {
                console.error('FeedBlockService::getFeedBlock: error get feed posts. Status:', response.status)
                return null
            }
        }
        catch (error) {
            console.error('FeedService::getBlockFeedPosts: exception raised. Msg: ', error)
            return null
        }
    }

    const getFeedPosts = async (slug) => {
        console.log('FeedService::getFeedPosts: start get feed posts for feed', slug)

        try {
            const params = {
                slug: slug
            }

            const response = await api.get(GET_FEED_POSTS_ACTION, { params: params })
            if (response.status === 200) {
                console.log('FeedService::getFeedPosts: successfully got feed posts')
                return response.data
            }
            else {
                console.error('FeedService::getFeedPosts: error get feed posts. Status:', response.status)
                return null
            }
        }
        catch (error) {
            console.error('FeedService::getFeedPosts: exception raised. Error=', error)
            return null
        }
    }

    const getWeeklyDigest = async () => {
        console.log('FeedService::getWeeklyDigest: start get weekly digest')

        try {
            const response = await api.get(GET_WEEKLY_DIGEST_ACTION)
            if (response.status === 200) {
                console.log('FeedBlockService::getFeedBlocks: successfully got weekly digest')
                return response.data
            }
            else {
                console.error('FeedBlockService::getFeedBlocks: error get weekly digest. Status:', response.status)
                return null
            }
        }
        catch (error) {
            console.error('FeedService::getWeeklyDigest: exception raised. Error=', error)
            return null
        }
    }

    return {
        getBlockFeedPosts, getFeedPosts,
        getWeeklyDigest
    }
}