import { useState, useEffect } from 'react'
import { useMsal } from '@azure/msal-react'
import { AccountInfo } from '@azure/msal-browser'

interface AuthState {
  isAuthenticated: boolean
  user: AccountInfo | null
  roles: string[]
  loading: boolean
}

export function useAuth(): AuthState {
  const { instance, accounts } = useMsal()
  const [authState, setAuthState] = useState<AuthState>({
    isAuthenticated: false,
    user: null,
    roles: [],
    loading: true,
  })

  useEffect(() => {
    const checkAuth = async () => {
      try {
        if (accounts.length > 0) {
          const account = accounts[0]
          const idTokenClaims = account.idTokenClaims as any
          
          setAuthState({
            isAuthenticated: true,
            user: account,
            roles: idTokenClaims?.roles || [],
            loading: false,
          })
        } else {
          setAuthState({
            isAuthenticated: false,
            user: null,
            roles: [],
            loading: false,
          })
        }
      } catch (error) {
        console.error('Auth check error:', error)
        setAuthState({
          isAuthenticated: false,
          user: null,
          roles: [],
          loading: false,
        })
      }
    }

    checkAuth()
  }, [accounts, instance])

  return authState
}