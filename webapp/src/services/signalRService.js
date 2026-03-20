import * as signalR from '@microsoft/signalr'

class SignalRService {
    constructor() {
        this.connection = null
        this.isConnected = false
        this.listeners = new Map()
    }

    /**
     * Start the SignalR connection
     * @returns {Promise<void>}
     */
    async start() {
        // If already connected, return
        if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
            console.log('SignalRService::start: already connected')
            this.isConnected = true
            return
        }

        // If connection exists but is not connected, wait for it to connect or reconnect
        if (this.connection && this.connection.state === signalR.HubConnectionState.Connecting) {
            console.log('SignalRService::start: connection in progress, waiting...')
            // Wait for connection to complete
            while (this.connection.state === signalR.HubConnectionState.Connecting) {
                await new Promise(resolve => setTimeout(resolve, 100))
            }
            if (this.connection.state === signalR.HubConnectionState.Connected) {
                this.isConnected = true
                return
            }
        }

        try {
            // Create connection to the feed update hub if it doesn't exist
            if (!this.connection) {
                this.connection = new signalR.HubConnectionBuilder()
                    .withUrl('/hubs/feedupdate')
                    .withAutomaticReconnect({
                        nextRetryDelayInMilliseconds: retryContext => {
                            if (retryContext.elapsedMilliseconds < 60000) {
                                // Retry every 2 seconds for the first minute
                                return 2000
                            } else {
                                // Retry every 30 seconds after the first minute
                                return 30000
                            }
                        }
                    })
                    .configureLogging(signalR.LogLevel.Information)
                    .build()

                // Handle connection state changes
                this.connection.onreconnecting(() => {
                    console.log('SignalRService: connection reconnecting...')
                    this.isConnected = false
                })

                this.connection.onreconnected(() => {
                    console.log('SignalRService: connection reconnected')
                    this.isConnected = true
                })

                this.connection.onclose(() => {
                    console.log('SignalRService: connection closed')
                    this.isConnected = false
                })

                // Register feed update handler
                this.connection.on('FeedUpdated', (data) => {
                    console.log('SignalRService: received FeedUpdated event', data)
                    this.notifyListeners('FeedUpdated', data)
                })
            }

            // Start the connection if not already connected
            if (this.connection.state !== signalR.HubConnectionState.Connected) {
                await this.connection.start()
                this.isConnected = true
                console.log('SignalRService: connection started successfully')
            }
        } catch (error) {
            console.error('SignalRService::start: error starting connection', error)
            this.isConnected = false
            throw error
        }
    }

    /**
     * Stop the SignalR connection
     * @returns {Promise<void>}
     */
    async stop() {
        if (this.connection) {
            try {
                await this.connection.stop()
                console.log('SignalRService: connection stopped')
            } catch (error) {
                console.error('SignalRService::stop: error stopping connection', error)
            } finally {
                this.isConnected = false
                this.connection = null
            }
        }
    }

    /**
     * Add a listener for a specific event
     * @param {string} eventName - The event name to listen for
     * @param {Function} callback - The callback function to call when the event is received
     * @returns {Function} - A function to remove the listener
     */
    on(eventName, callback) {
        if (!this.listeners.has(eventName)) {
            this.listeners.set(eventName, [])
        }
        this.listeners.get(eventName).push(callback)

        // Return a function to remove the listener
        return () => {
            const callbacks = this.listeners.get(eventName)
            if (callbacks) {
                const index = callbacks.indexOf(callback)
                if (index > -1) {
                    callbacks.splice(index, 1)
                }
            }
        }
    }

    /**
     * Remove a listener for a specific event
     * @param {string} eventName - The event name
     * @param {Function} callback - The callback function to remove
     */
    off(eventName, callback) {
        const callbacks = this.listeners.get(eventName)
        if (callbacks) {
            const index = callbacks.indexOf(callback)
            if (index > -1) {
                callbacks.splice(index, 1)
            }
        }
    }

    /**
     * Notify all listeners of an event
     * @private
     * @param {string} eventName - The event name
     * @param {*} data - The event data
     */
    notifyListeners(eventName, data) {
        const callbacks = this.listeners.get(eventName)
        if (callbacks) {
            callbacks.forEach(callback => {
                try {
                    callback(data)
                } catch (error) {
                    console.error(`SignalRService: error in listener for ${eventName}`, error)
                }
            })
        }
    }

    /**
     * Get the current connection state
     * @returns {boolean}
     */
    getConnectionState() {
        return this.isConnected && this.connection?.state === signalR.HubConnectionState.Connected
    }
}

// Export a singleton instance
const signalRService = new SignalRService()

export default signalRService
