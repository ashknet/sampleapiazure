import axios from 'axios'
import * as SecureStore from 'expo-secure-store'

const API_URL = process.env.API_URL || 'https://localhost:5001'

const apiClient = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
})

// Request interceptor to add auth token
apiClient.interceptors.request.use(
  async (config) => {
    try {
      const token = await SecureStore.getItemAsync('accessToken')
      if (token) {
        config.headers.Authorization = `Bearer ${token}`
      }
    } catch (error) {
      console.error('Error getting access token:', error)
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
        const refreshToken = await SecureStore.getItemAsync('refreshToken')
        if (refreshToken) {
          // Import authService dynamically to avoid circular dependency
          const { authService } = await import('./authService')
          const tokenResponse = await authService.refreshAccessToken(refreshToken)
          
          await SecureStore.setItemAsync('accessToken', tokenResponse.access_token)
          if (tokenResponse.refresh_token) {
            await SecureStore.setItemAsync('refreshToken', tokenResponse.refresh_token)
          }
          
          // Retry the original request
          error.config.headers.Authorization = `Bearer ${tokenResponse.access_token}`
          return apiClient.request(error.config)
        }
      } catch (refreshError) {
        console.error('Token refresh failed:', refreshError)
        // Redirect to login
      }
    }
    return Promise.reject(error)
  }
)

export { apiClient }