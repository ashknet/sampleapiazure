import { apiClient } from './apiClient'

export const patientService = {
  getSummary: async () => {
    const response = await apiClient.get('/api/v1/patient/summary')
    return response.data
  },

  getRecentActivity: async () => {
    const response = await apiClient.get('/api/v1/patient/activity')
    return response.data
  },

  getRecords: async (category?: string) => {
    const response = await apiClient.get('/api/v1/patient/records', {
      params: { category },
    })
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
}