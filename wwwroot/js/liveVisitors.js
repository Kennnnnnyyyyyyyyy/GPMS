/**
 * Live Visitors Counter - Real-time visitor count updates using SignalR with polling fallback
 */

class LiveVisitorsCounter {
    constructor(options = {}) {
        this.countElementId = options.countElementId || 'liveVisitorsCount';
        this.statusElementId = options.statusElementId || 'connectionStatus';
        this.pollingInterval = options.pollingInterval || 5000; // 5 seconds
        this.apiEndpoint = options.apiEndpoint || '/api/v1/dashboards/visitors-live';
        this.hubUrl = options.hubUrl || '/hubs/visitors';
        
        this.connection = null;
        this.pollingTimer = null;
        this.isSignalRActive = false;
        this.reconnectAttempts = 0;
        this.maxReconnectAttempts = 10;
        
        this.init();
    }

    /**
     * Initialize the live counter
     */
    async init() {
        console.log('LiveVisitorsCounter: Initializing...');
        
        // Try SignalR first
        await this.initSignalR();
        
        // Load initial data
        await this.loadInitialData();
        
        // Start fallback polling if SignalR failed
        if (!this.isSignalRActive) {
            this.startPolling();
        }
    }

    /**
     * Initialize SignalR connection
     */
    async initSignalR() {
        try {
            // Check if SignalR is available
            if (typeof signalR === 'undefined') {
                console.warn('LiveVisitorsCounter: SignalR not loaded, falling back to polling');
                return;
            }

            this.connection = new signalR.HubConnectionBuilder()
                .withUrl(this.hubUrl)
                .withAutomaticReconnect({
                    nextRetryDelayInMilliseconds: retryContext => {
                        if (retryContext.previousRetryCount < 5) {
                            return Math.pow(2, retryContext.previousRetryCount) * 1000; // Exponential backoff
                        }
                        return 30000; // Max 30 seconds
                    }
                })
                .configureLogging(signalR.LogLevel.Information)
                .build();

            // Set up event handlers
            this.setupSignalREvents();

            // Start connection
            await this.connection.start();
            this.isSignalRActive = true;
            this.reconnectAttempts = 0;
            this.updateConnectionStatus('LIVE');
            
            console.log('LiveVisitorsCounter: SignalR connected successfully');

        } catch (error) {
            console.error('LiveVisitorsCounter: SignalR connection failed:', error);
            this.isSignalRActive = false;
            this.updateConnectionStatus('OFFLINE');
        }
    }

    /**
     * Set up SignalR event handlers
     */
    setupSignalREvents() {
        if (!this.connection) return;

        // Live count updates
        this.connection.on('liveCountChanged', (data) => {
            console.log('LiveVisitorsCounter: Received count update:', data);
            this.updateCount(data.totalInside);
        });

        // Visitor check-in events
        this.connection.on('visitorCheckedIn', (data) => {
            console.log('LiveVisitorsCounter: Visitor checked in:', data);
            this.onVisitorCheckedIn(data);
        });

        // Visitor check-out events
        this.connection.on('visitorCheckedOut', (data) => {
            console.log('LiveVisitorsCounter: Visitor checked out:', data);
            this.onVisitorCheckedOut(data);
        });

        // Connection state changes
        this.connection.onreconnecting(() => {
            console.log('LiveVisitorsCounter: Reconnecting...');
            this.updateConnectionStatus('RECONNECTING...');
            this.startPolling(); // Use polling during reconnection
        });

        this.connection.onreconnected(() => {
            console.log('LiveVisitorsCounter: Reconnected');
            this.updateConnectionStatus('LIVE');
            this.stopPolling(); // Stop polling when SignalR is back
            this.isSignalRActive = true;
            this.reconnectAttempts = 0;
            // Refresh data after reconnection
            this.loadInitialData();
        });

        this.connection.onclose(() => {
            console.log('LiveVisitorsCounter: Connection closed');
            this.updateConnectionStatus('OFFLINE');
            this.isSignalRActive = false;
            this.startPolling(); // Fallback to polling
        });
    }

    /**
     * Load initial data from API
     */
    async loadInitialData() {
        try {
            const response = await fetch(this.apiEndpoint, {
                credentials: 'include',
                headers: {
                    'Accept': 'application/json'
                }
            });

            if (response.ok) {
                const data = await response.json();
                this.updateCount(data.totalInside);
                console.log('LiveVisitorsCounter: Initial data loaded:', data);
            } else {
                console.error('LiveVisitorsCounter: Failed to load initial data:', response.status);
            }
        } catch (error) {
            console.error('LiveVisitorsCounter: Error loading initial data:', error);
        }
    }

    /**
     * Start polling fallback
     */
    startPolling() {
        if (this.pollingTimer) return; // Already polling

        console.log('LiveVisitorsCounter: Starting polling fallback');
        this.pollingTimer = setInterval(async () => {
            if (!this.isSignalRActive) {
                await this.loadInitialData();
            }
        }, this.pollingInterval);
    }

    /**
     * Stop polling
     */
    stopPolling() {
        if (this.pollingTimer) {
            console.log('LiveVisitorsCounter: Stopping polling');
            clearInterval(this.pollingTimer);
            this.pollingTimer = null;
        }
    }

    /**
     * Update the count display
     */
    updateCount(count) {
        const element = document.getElementById(this.countElementId);
        if (element) {
            element.textContent = count || '0';
            element.setAttribute('data-last-updated', new Date().toISOString());
        }

        // Trigger custom event for other components
        window.dispatchEvent(new CustomEvent('liveVisitorCountChanged', { 
            detail: { count: count || 0 }
        }));
    }

    /**
     * Update connection status indicator
     */
    updateConnectionStatus(status) {
        const element = document.getElementById(this.statusElementId);
        if (element) {
            element.textContent = status;
            element.className = `connection-status ${status.toLowerCase().replace('...', '')}`;
        }
    }

    /**
     * Handle visitor check-in event
     */
    onVisitorCheckedIn(data) {
        // Trigger custom event for other components to handle
        window.dispatchEvent(new CustomEvent('visitorCheckedIn', { detail: data }));
        
        // Optionally show notification
        this.showNotification(`${data.visitorName} checked in`, 'success');
    }

    /**
     * Handle visitor check-out event
     */
    onVisitorCheckedOut(data) {
        // Trigger custom event for other components to handle
        window.dispatchEvent(new CustomEvent('visitorCheckedOut', { detail: data }));
        
        // Optionally show notification
        this.showNotification(`${data.visitorName} checked out`, 'info');
    }

    /**
     * Show notification (if notification system is available)
     */
    showNotification(message, type = 'info') {
        // Check if there's a global notification function
        if (typeof showNotification === 'function') {
            showNotification(message, type);
        } else {
            console.log(`LiveVisitorsCounter: ${message}`);
        }
    }

    /**
     * Cleanup resources
     */
    destroy() {
        this.stopPolling();
        
        if (this.connection) {
            this.connection.stop();
            this.connection = null;
        }
        
        this.isSignalRActive = false;
        console.log('LiveVisitorsCounter: Destroyed');
    }

    /**
     * Manually refresh count
     */
    async refresh() {
        await this.loadInitialData();
    }
}

// Auto-initialize if DOM is ready and required elements exist
document.addEventListener('DOMContentLoaded', function() {
    const countElement = document.getElementById('liveVisitorsCount');
    if (countElement) {
        window.liveVisitorsCounter = new LiveVisitorsCounter();
    }
});

// Make class available globally
window.LiveVisitorsCounter = LiveVisitorsCounter;
