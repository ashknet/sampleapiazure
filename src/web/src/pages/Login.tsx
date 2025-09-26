import { useEffect } from 'react'
import { useNavigate, useLocation } from 'react-router-dom'
import { useMsal } from '@azure/msal-react'
import {
  Box,
  Button,
  Card,
  CardContent,
  Container,
  Typography,
  CircularProgress,
} from '@mui/material'
import { LocalHospital } from '@mui/icons-material'
import { loginRequest } from '../services/auth'

export function Login() {
  const navigate = useNavigate()
  const location = useLocation()
  const { instance, accounts, inProgress } = useMsal()

  const from = location.state?.from?.pathname || '/'

  useEffect(() => {
    if (accounts.length > 0) {
      // User is already logged in, redirect
      const redirectPath = sessionStorage.getItem('redirectAfterLogin') || from
      sessionStorage.removeItem('redirectAfterLogin')
      navigate(redirectPath, { replace: true })
    }
  }, [accounts, navigate, from])

  const handleLogin = async () => {
    try {
      await instance.loginRedirect({
        ...loginRequest,
        redirectStartPage: from,
      })
    } catch (error) {
      console.error('Login error:', error)
    }
  }

  if (inProgress === 'login') {
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

  return (
    <Container component="main" maxWidth="xs">
      <Box
        sx={{
          marginTop: 8,
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
        }}
      >
        <Card sx={{ width: '100%', mt: 3 }}>
          <CardContent sx={{ textAlign: 'center', py: 5 }}>
            <LocalHospital sx={{ fontSize: 60, color: 'primary.main', mb: 2 }} />
            <Typography component="h1" variant="h4" gutterBottom>
              EMR System
            </Typography>
            <Typography variant="body1" color="text.secondary" sx={{ mb: 4 }}>
              Electronic Medical Records Portal
            </Typography>
            <Button
              fullWidth
              variant="contained"
              size="large"
              onClick={handleLogin}
              sx={{ mb: 2 }}
            >
              Sign In with Azure AD B2C
            </Button>
            <Typography variant="body2" color="text.secondary">
              Secure authentication powered by Microsoft
            </Typography>
          </CardContent>
        </Card>
      </Box>
    </Container>
  )
}