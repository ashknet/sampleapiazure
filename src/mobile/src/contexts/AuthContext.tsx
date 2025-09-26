import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react'
import * as SecureStore from 'expo-secure-store'
import * as AuthSession from 'expo-auth-session'
import * as WebBrowser from 'expo-web-browser'
import { authService } from '../services/authService'

WebBrowser.maybeCompleteAuthSession()

interface User {
  id: string
  email: string
  name: string
  roles: string[]
}

interface AuthContextType {
  user: User | null
  isLoading: boolean
  isAuthenticated: boolean
  login: () => Promise<void>
  logout: () => Promise<void>
  refreshToken: () => Promise<void>
}

const AuthContext = createContext<AuthContextType | undefined>(undefined)

const discovery = {
  authorizationEndpoint: `${process.env.B2C_AUTHORITY}/oauth2/v2.0/authorize`,
  tokenEndpoint: `${process.env.B2C_AUTHORITY}/oauth2/v2.0/token`,
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null)
  const [isLoading, setIsLoading] = useState(true)

  const [request, response, promptAsync] = AuthSession.useAuthRequest(
    {
      clientId: process.env.B2C_CLIENT_ID || '',
      scopes: ['openid', 'profile', 'email', 'offline_access'],
      responseType: AuthSession.ResponseType.Code,
      redirectUri: AuthSession.makeRedirectUri({
        scheme: 'emrmobile',
      }),
      codeChallenge: AuthSession.AuthRequest.PKCE.S256,
    },
    discovery
  )

  useEffect(() => {
    // Check for existing session
    checkAuthState()
  }, [])

  useEffect(() => {
    if (response?.type === 'success') {
      const { code } = response.params
      handleAuthCode(code)
    }
  }, [response])

  const checkAuthState = async () => {
    try {
      const token = await SecureStore.getItemAsync('accessToken')
      const userDataStr = await SecureStore.getItemAsync('userData')
      
      if (token && userDataStr) {
        const userData = JSON.parse(userDataStr)
        setUser(userData)
      }
    } catch (error) {
      console.error('Error checking auth state:', error)
    } finally {
      setIsLoading(false)
    }
  }

  const handleAuthCode = async (code: string) => {
    try {
      const tokenResponse = await authService.exchangeCodeForToken(code, request?.codeVerifier!)
      
      await SecureStore.setItemAsync('accessToken', tokenResponse.access_token)
      await SecureStore.setItemAsync('refreshToken', tokenResponse.refresh_token)
      
      const userData = await authService.getUserInfo(tokenResponse.access_token)
      await SecureStore.setItemAsync('userData', JSON.stringify(userData))
      
      setUser(userData)
    } catch (error) {
      console.error('Error exchanging code for token:', error)
    }
  }

  const login = async () => {
    await promptAsync()
  }

  const logout = async () => {
    try {
      await SecureStore.deleteItemAsync('accessToken')
      await SecureStore.deleteItemAsync('refreshToken')
      await SecureStore.deleteItemAsync('userData')
      setUser(null)
      
      // Open logout URL in browser
      const logoutUrl = `${process.env.B2C_AUTHORITY}/oauth2/v2.0/logout`
      await WebBrowser.openBrowserAsync(logoutUrl)
    } catch (error) {
      console.error('Error logging out:', error)
    }
  }

  const refreshToken = async () => {
    try {
      const refreshTokenValue = await SecureStore.getItemAsync('refreshToken')
      if (!refreshTokenValue) {
        throw new Error('No refresh token available')
      }
      
      const tokenResponse = await authService.refreshAccessToken(refreshTokenValue)
      
      await SecureStore.setItemAsync('accessToken', tokenResponse.access_token)
      if (tokenResponse.refresh_token) {
        await SecureStore.setItemAsync('refreshToken', tokenResponse.refresh_token)
      }
    } catch (error) {
      console.error('Error refreshing token:', error)
      // If refresh fails, logout user
      await logout()
    }
  }

  const value: AuthContextType = {
    user,
    isLoading,
    isAuthenticated: !!user,
    login,
    logout,
    refreshToken,
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider')
  }
  return context
}