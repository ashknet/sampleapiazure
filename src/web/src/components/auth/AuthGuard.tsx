import { ReactNode, useEffect } from 'react'
import { Navigate, useLocation } from 'react-router-dom'
import { useMsal } from '@azure/msal-react'
import { CircularProgress, Box } from '@mui/material'
import { InteractionStatus } from '@azure/msal-browser'
import { useAuth } from '../../hooks/useAuth'

interface AuthGuardProps {
  children: ReactNode
  allowedRoles?: string[]
}

export function AuthGuard({ children, allowedRoles }: AuthGuardProps) {
  const location = useLocation()
  const { inProgress } = useMsal()
  const { isAuthenticated, roles, loading } = useAuth()

  useEffect(() => {
    // Handle redirect after login
    if (inProgress === InteractionStatus.None && !isAuthenticated && !loading) {
      // Store intended destination
      sessionStorage.setItem('redirectAfterLogin', location.pathname)
    }
  }, [inProgress, isAuthenticated, loading, location])

  if (loading || inProgress !== InteractionStatus.None) {
    return (
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        minHeight="100vh"
      >
        <CircularProgress />
      </Box>
    )
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />
  }

  if (allowedRoles && allowedRoles.length > 0) {
    const hasRequiredRole = allowedRoles.some(role => roles.includes(role))
    if (!hasRequiredRole) {
      return <Navigate to="/unauthorized" replace />
    }
  }

  return <>{children}</>
}