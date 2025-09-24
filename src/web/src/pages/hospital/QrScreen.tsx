import { useState } from 'react'
import {
  Box,
  Button,
  Card,
  CardContent,
  Grid,
  Typography,
  Paper,
  Stepper,
  Step,
  StepLabel,
  Alert,
  CircularProgress,
  Chip,
  List,
  ListItem,
  ListItemText,
  ListItemIcon,
  Checkbox,
} from '@mui/material'
import { QrCode2, Refresh, CheckCircle, Person } from '@mui/icons-material'
import { useMutation, useQuery } from '@tanstack/react-query'
import { hospitalService } from '../../services/hospitalService'
import { format } from 'date-fns'

const steps = ['Generate QR Code', 'Patient Scans & Authorizes', 'Access Granted']

const requestableScopes = [
  { id: 'read:demographics', label: 'Demographics', description: 'Name, DOB, contact info' },
  { id: 'read:insurance', label: 'Insurance', description: 'Coverage and eligibility' },
  { id: 'read:medications', label: 'Medications', description: 'Current medication list' },
  { id: 'read:allergies', label: 'Allergies', description: 'Known allergies' },
  { id: 'read:conditions', label: 'Conditions', description: 'Medical conditions' },
  { id: 'write:documents', label: 'Upload Documents', description: 'Add clinical documents' },
]

export function HospitalQrScreen() {
  const [activeStep, setActiveStep] = useState(0)
  const [selectedScopes, setSelectedScopes] = useState<string[]>([
    'read:demographics',
    'read:insurance',
  ])
  const [qrCode, setQrCode] = useState<any>(null)
  const [exchangeResult, setExchangeResult] = useState<any>(null)

  const generateQrMutation = useMutation({
    mutationFn: () =>
      hospitalService.generateQrCode({
        requestedScopes: selectedScopes,
        expirationMinutes: 15,
      }),
    onSuccess: (data) => {
      setQrCode(data)
      setActiveStep(1)
      // Start polling for authorization
      startPolling(data.code)
    },
  })

  const exchangeMutation = useMutation({
    mutationFn: (code: string) => hospitalService.exchangeQrCode(code),
    onSuccess: (data) => {
      setExchangeResult(data)
      setActiveStep(2)
    },
  })

  const startPolling = (code: string) => {
    const pollInterval = setInterval(async () => {
      try {
        const result = await hospitalService.checkQrStatus(code)
        if (result.status === 'authorized') {
          clearInterval(pollInterval)
          exchangeMutation.mutate(code)
        } else if (result.status === 'expired') {
          clearInterval(pollInterval)
          setActiveStep(0)
          setQrCode(null)
        }
      } catch (error) {
        console.error('Polling error:', error)
      }
    }, 2000) // Poll every 2 seconds

    // Clean up on unmount
    return () => clearInterval(pollInterval)
  }

  const handleScopeToggle = (scopeId: string) => {
    setSelectedScopes((prev) =>
      prev.includes(scopeId)
        ? prev.filter((id) => id !== scopeId)
        : [...prev, scopeId]
    )
  }

  const handleGenerateQr = () => {
    setExchangeResult(null)
    generateQrMutation.mutate()
  }

  const handleReset = () => {
    setActiveStep(0)
    setQrCode(null)
    setExchangeResult(null)
  }

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Patient QR Connection
      </Typography>
      <Typography variant="body1" color="text.secondary" gutterBottom>
        Generate a QR code for patients to scan and authorize access to their records
      </Typography>

      <Box sx={{ mt: 3 }}>
        <Stepper activeStep={activeStep} sx={{ mb: 4 }}>
          {steps.map((label) => (
            <Step key={label}>
              <StepLabel>{label}</StepLabel>
            </Step>
          ))}
        </Stepper>

        <Grid container spacing={3}>
          <Grid item xs={12} md={6}>
            {activeStep === 0 && (
              <Card>
                <CardContent>
                  <Typography variant="h6" gutterBottom>
                    Select Information to Request
                  </Typography>
                  <List>
                    {requestableScopes.map((scope) => (
                      <ListItem key={scope.id} dense>
                        <ListItemIcon>
                          <Checkbox
                            edge="start"
                            checked={selectedScopes.includes(scope.id)}
                            onChange={() => handleScopeToggle(scope.id)}
                          />
                        </ListItemIcon>
                        <ListItemText
                          primary={scope.label}
                          secondary={scope.description}
                        />
                      </ListItem>
                    ))}
                  </List>
                  <Box mt={3}>
                    <Button
                      variant="contained"
                      size="large"
                      fullWidth
                      startIcon={<QrCode2 />}
                      onClick={handleGenerateQr}
                      disabled={
                        selectedScopes.length === 0 || generateQrMutation.isPending
                      }
                    >
                      {generateQrMutation.isPending ? (
                        <CircularProgress size={24} />
                      ) : (
                        'Generate QR Code'
                      )}
                    </Button>
                  </Box>
                </CardContent>
              </Card>
            )}

            {activeStep === 1 && qrCode && (
              <Card>
                <CardContent sx={{ textAlign: 'center' }}>
                  <Typography variant="h6" gutterBottom>
                    QR Code Generated
                  </Typography>
                  <Box
                    component="img"
                    src={qrCode.qrCodeUrl}
                    alt="QR Code"
                    sx={{ width: 300, height: 300, my: 2 }}
                  />
                  <Typography variant="body2" color="text.secondary" gutterBottom>
                    Code: {qrCode.code}
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    Expires: {format(new Date(qrCode.expiresAt), 'h:mm:ss a')}
                  </Typography>
                  <Alert severity="info" sx={{ mt: 2 }}>
                    Waiting for patient to scan and authorize...
                  </Alert>
                </CardContent>
              </Card>
            )}

            {activeStep === 2 && exchangeResult && (
              <Card>
                <CardContent>
                  <Box sx={{ textAlign: 'center', mb: 3 }}>
                    <CheckCircle color="success" sx={{ fontSize: 60 }} />
                    <Typography variant="h6" sx={{ mt: 2 }}>
                      Connection Established
                    </Typography>
                  </Box>
                  <Paper sx={{ p: 2, bgcolor: 'grey.100' }}>
                    <Typography variant="subtitle2" gutterBottom>
                      Patient Information
                    </Typography>
                    <Box sx={{ mt: 1 }}>
                      <Typography variant="body2">
                        <strong>Name:</strong> {exchangeResult.patient.firstName}{' '}
                        {exchangeResult.patient.lastName}
                      </Typography>
                      <Typography variant="body2">
                        <strong>DOB:</strong>{' '}
                        {format(
                          new Date(exchangeResult.patient.dateOfBirth),
                          'MM/dd/yyyy'
                        )}
                      </Typography>
                      <Typography variant="body2">
                        <strong>MRN:</strong>{' '}
                        {exchangeResult.patient.medicalRecordNumber}
                      </Typography>
                    </Box>
                  </Paper>
                  <Box sx={{ mt: 2 }}>
                    <Typography variant="subtitle2" gutterBottom>
                      Authorized Scopes
                    </Typography>
                    <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                      {exchangeResult.scopes.map((scope: string) => (
                        <Chip key={scope} label={scope} size="small" />
                      ))}
                    </Box>
                  </Box>
                  <Button
                    variant="outlined"
                    fullWidth
                    sx={{ mt: 3 }}
                    startIcon={<Refresh />}
                    onClick={handleReset}
                  >
                    Generate New QR Code
                  </Button>
                </CardContent>
              </Card>
            )}
          </Grid>

          <Grid item xs={12} md={6}>
            <Paper sx={{ p: 3 }}>
              <Typography variant="h6" gutterBottom>
                How It Works
              </Typography>
              <List>
                <ListItem>
                  <ListItemIcon>
                    <Chip label="1" color="primary" size="small" />
                  </ListItemIcon>
                  <ListItemText
                    primary="Generate QR Code"
                    secondary="Select the information you need access to and generate a unique QR code"
                  />
                </ListItem>
                <ListItem>
                  <ListItemIcon>
                    <Chip label="2" color="primary" size="small" />
                  </ListItemIcon>
                  <ListItemText
                    primary="Patient Scans"
                    secondary="Patient uses their mobile app to scan the QR code"
                  />
                </ListItem>
                <ListItem>
                  <ListItemIcon>
                    <Chip label="3" color="primary" size="small" />
                  </ListItemIcon>
                  <ListItemText
                    primary="Patient Authorizes"
                    secondary="Patient reviews and approves the requested information"
                  />
                </ListItem>
                <ListItem>
                  <ListItemIcon>
                    <Chip label="4" color="primary" size="small" />
                  </ListItemIcon>
                  <ListItemText
                    primary="Access Granted"
                    secondary="You receive temporary access to the authorized information"
                  />
                </ListItem>
              </List>
              <Alert severity="warning" sx={{ mt: 2 }}>
                Access is temporary and will expire after the consent duration set by
                the patient
              </Alert>
            </Paper>
          </Grid>
        </Grid>
      </Box>
    </Box>
  )
}