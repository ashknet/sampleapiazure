import { PublicClientApplication, Configuration } from '@azure/msal-browser'

const msalConfig: Configuration = {
  auth: {
    clientId: import.meta.env.VITE_B2C_CLIENT_ID || '',
    authority: import.meta.env.VITE_B2C_AUTHORITY || '',
    knownAuthorities: [import.meta.env.VITE_B2C_KNOWN_AUTHORITY || ''],
    redirectUri: window.location.origin,
    postLogoutRedirectUri: window.location.origin,
  },
  cache: {
    cacheLocation: 'localStorage',
    storeAuthStateInCookie: false,
  },
}

export const msalInstance = new PublicClientApplication(msalConfig)

export const loginRequest = {
  scopes: ['openid', 'profile', 'email'],
}

export const apiScopes = {
  patient: [`${import.meta.env.VITE_API_SCOPE}/patient.read`],
  hospital: [`${import.meta.env.VITE_API_SCOPE}/hospital.read`],
}