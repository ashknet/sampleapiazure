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

  getRecordDetails: async (recordId: string) => {
    const response = await apiClient.get(`/api/v1/patient/records/${recordId}`)
    return response.data
  },

  downloadDocument: async (documentId: string) => {
    const response = await apiClient.get(`/api/v1/patient/documents/${documentId}/download`)
    return response.data
  },
}