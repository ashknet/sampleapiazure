import { apiClient } from './apiClient'

interface GenerateQrCodeRequest {
  requestedScopes: string[]
  expirationMinutes?: number
}

export const hospitalService = {
  generateQrCode: async (request: GenerateQrCodeRequest) => {
    const response = await apiClient.post('/api/v1/connect/qr', {
      organizationId: localStorage.getItem('organizationId'), // This would come from user context
      requestedScopes: request.requestedScopes,
      expirationMinutes: request.expirationMinutes || 15,
    })
    return response.data
  },

  checkQrStatus: async (code: string) => {
    const response = await apiClient.get(`/api/v1/connect/qr/${code}/status`)
    return response.data
  },

  exchangeQrCode: async (code: string) => {
    const response = await apiClient.get(`/api/v1/connect/qr/${code}/exchange`)
    return response.data
  },

  searchPatients: async (query: string) => {
    const response = await apiClient.get('/api/v1/hospital/patients/search', {
      params: { q: query },
    })
    return response.data
  },

  getPatientDetails: async (patientId: string) => {
    const response = await apiClient.get(`/api/v1/hospital/patients/${patientId}`)
    return response.data
  },

  uploadDocument: async (patientId: string, file: File, metadata: any) => {
    const formData = new FormData()
    formData.append('file', file)
    formData.append('metadata', JSON.stringify(metadata))

    const response = await apiClient.post(
      `/api/v1/hospital/patients/${patientId}/documents`,
      formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      }
    )
    return response.data
  },

  getDashboardStats: async () => {
    const response = await apiClient.get('/api/v1/hospital/dashboard/stats')
    return response.data
  },
}