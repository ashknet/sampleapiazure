import axios from 'axios'
import { msalInstance } from './auth'

const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'https://localhost:5001',
  headers: {
    'Content-Type': 'application/json',
  },
})

// Request interceptor to add auth token
apiClient.interceptors.request.use(
  async (config) => {
    const accounts = msalInstance.getAllAccounts()
    if (accounts.length > 0) {
      try {
        const response = await msalInstance.acquireTokenSilent({
          scopes: [`${import.meta.env.VITE_API_SCOPE}/.default`],
          account: accounts[0],
        })
        config.headers.Authorization = `Bearer ${response.accessToken}`
      } catch (error) {
        console.error('Failed to acquire token:', error)
      }
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// Response interceptor for error handling
apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401) {
      // Token might be expired, try to refresh
      try {
        const accounts = msalInstance.getAllAccounts()
        if (accounts.length > 0) {
          await msalInstance.acquireTokenSilent({
            scopes: [`${import.meta.env.VITE_API_SCOPE}/.default`],
            account: accounts[0],
            forceRefresh: true,
          })
          // Retry the original request
          return apiClient.request(error.config)
        }
      } catch (refreshError) {
        // Refresh failed, redirect to login
        msalInstance.loginRedirect()
      }
    }
    return Promise.reject(error)
  }
)

export { apiClient }