import axios from 'axios'

const B2C_AUTHORITY = process.env.B2C_AUTHORITY || ''
const CLIENT_ID = process.env.B2C_CLIENT_ID || ''

export const authService = {
  exchangeCodeForToken: async (code: string, codeVerifier: string) => {
    const response = await axios.post(
      `${B2C_AUTHORITY}/oauth2/v2.0/token`,
      new URLSearchParams({
        grant_type: 'authorization_code',
        client_id: CLIENT_ID,
        code,
        code_verifier: codeVerifier,
      }),
      {
        headers: {
          'Content-Type': 'application/x-www-form-urlencoded',
        },
      }
    )
    return response.data
  },

  refreshAccessToken: async (refreshToken: string) => {
    const response = await axios.post(
      `${B2C_AUTHORITY}/oauth2/v2.0/token`,
      new URLSearchParams({
        grant_type: 'refresh_token',
        client_id: CLIENT_ID,
        refresh_token: refreshToken,
        scope: 'openid profile email offline_access',
      }),
      {
        headers: {
          'Content-Type': 'application/x-www-form-urlencoded',
        },
      }
    )
    return response.data
  },

  getUserInfo: async (accessToken: string) => {
    // Parse JWT token to get user info
    const base64Url = accessToken.split('.')[1]
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/')
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    )

    const tokenData = JSON.parse(jsonPayload)
    
    return {
      id: tokenData.sub,
      email: tokenData.emails?.[0] || tokenData.email,
      name: tokenData.name || `${tokenData.given_name} ${tokenData.family_name}`,
      roles: tokenData.roles || ['Patient'],
    }
  },
}