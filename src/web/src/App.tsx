import { useEffect } from 'react'
import { Routes, Route, Navigate } from 'react-router-dom'
import { useMsal } from '@azure/msal-react'
import { Container } from '@mui/material'
import { AuthGuard } from './components/auth/AuthGuard'
import { Layout } from './components/common/Layout'
import { PatientDashboard } from './pages/patient/Dashboard'
import { PatientRecords } from './pages/patient/Records'
import { PatientConsents } from './pages/patient/Consents'
import { HospitalDashboard } from './pages/hospital/Dashboard'
import { HospitalQrScreen } from './pages/hospital/QrScreen'
import { HospitalPatientSearch } from './pages/hospital/PatientSearch'
import { Login } from './pages/Login'
import { NotFound } from './pages/NotFound'

function App() {
  const { instance } = useMsal()

  useEffect(() => {
    // Initialize MSAL
    instance.initialize()
  }, [instance])

  return (
    <Container maxWidth={false} disableGutters>
      <Routes>
        <Route path="/login" element={<Login />} />
        
        {/* Patient Portal Routes */}
        <Route
          path="/patient"
          element={
            <AuthGuard allowedRoles={['Patient']}>
              <Layout portal="patient" />
            </AuthGuard>
          }
        >
          <Route index element={<Navigate to="dashboard" replace />} />
          <Route path="dashboard" element={<PatientDashboard />} />
          <Route path="records" element={<PatientRecords />} />
          <Route path="consents" element={<PatientConsents />} />
        </Route>

        {/* Hospital Portal Routes */}
        <Route
          path="/hospital"
          element={
            <AuthGuard allowedRoles={['Clinician', 'Registrar', 'HIM', 'OrgAdmin']}>
              <Layout portal="hospital" />
            </AuthGuard>
          }
        >
          <Route index element={<Navigate to="dashboard" replace />} />
          <Route path="dashboard" element={<HospitalDashboard />} />
          <Route path="qr-connect" element={<HospitalQrScreen />} />
          <Route path="patients" element={<HospitalPatientSearch />} />
        </Route>

        <Route path="/" element={<Navigate to="/patient" replace />} />
        <Route path="*" element={<NotFound />} />
      </Routes>
    </Container>
  )
}

export default App