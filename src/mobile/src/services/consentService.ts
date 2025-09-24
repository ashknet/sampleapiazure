import { apiClient } from './apiClient'

interface AuthorizeQrCodeRequest {
  authorizedScopes: string[]
  consentDurationHours: number
}

export const consentService = {
  getQrCodeDetails: async (code: string) => {
    const response = await apiClient.get(`/api/v1/connect/qr/${code}/details`)
    return response.data
  },

  authorizeQrCode: async (code: string, request: AuthorizeQrCodeRequest) => {
    const response = await apiClient.post(`/api/v1/connect/qr/${code}/authorize`, request)
    return response.data
  },

  getConsents: async () => {
    const response = await apiClient.get('/api/v1/patient/consents')
    return response.data
  },

  revokeConsent: async (consentId: string) => {
    const response = await apiClient.post(`/api/v1/patient/consents/${consentId}/revoke`)
    return response.data
  },

  getConsentDetails: async (consentId: string) => {
    const response = await apiClient.get(`/api/v1/patient/consents/${consentId}`)
    return response.data
  },
}