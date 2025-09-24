import { useState } from 'react'
import {
  Box,
  Card,
  CardContent,
  Typography,
  List,
  ListItem,
  ListItemText,
  ListItemSecondaryAction,
  IconButton,
  Chip,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  DialogContentText,
  Alert,
  Paper,
  Divider,
} from '@mui/material'
import {
  Delete,
  Visibility,
  Business,
  AccessTime,
  CheckCircle,
  Cancel,
} from '@mui/icons-material'
import { format } from 'date-fns'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { patientService } from '../../services/patientService'

export function PatientConsents() {
  const queryClient = useQueryClient()
  const [selectedConsent, setSelectedConsent] = useState<any>(null)
  const [viewDialogOpen, setViewDialogOpen] = useState(false)
  const [revokeDialogOpen, setRevokeDialogOpen] = useState(false)

  const { data: consents, isLoading } = useQuery({
    queryKey: ['patientConsents'],
    queryFn: () => patientService.getConsents(),
  })

  const revokeMutation = useMutation({
    mutationFn: (consentId: string) => patientService.revokeConsent(consentId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['patientConsents'] })
      setRevokeDialogOpen(false)
      setSelectedConsent(null)
    },
  })

  const handleViewConsent = (consent: any) => {
    setSelectedConsent(consent)
    setViewDialogOpen(true)
  }

  const handleRevokeClick = (consent: any) => {
    setSelectedConsent(consent)
    setRevokeDialogOpen(true)
  }

  const handleRevokeConfirm = () => {
    if (selectedConsent) {
      revokeMutation.mutate(selectedConsent.id)
    }
  }

  const getStatusColor = (status: string) => {
    switch (status.toLowerCase()) {
      case 'active':
        return 'success'
      case 'revoked':
        return 'error'
      case 'expired':
        return 'default'
      default:
        return 'default'
    }
  }

  const getStatusIcon = (status: string) => {
    switch (status.toLowerCase()) {
      case 'active':
        return <CheckCircle fontSize="small" />
      case 'revoked':
      case 'expired':
        return <Cancel fontSize="small" />
      default:
        return null
    }
  }

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Consent Management
      </Typography>
      <Typography variant="body1" color="text.secondary" gutterBottom>
        Manage who has access to your medical records and for how long
      </Typography>

      <Alert severity="info" sx={{ mt: 2, mb: 3 }}>
        You can revoke consent at any time. Organizations will immediately lose access to your records.
      </Alert>

      <Card>
        <CardContent>
          <Typography variant="h6" gutterBottom>
            Active Consents
          </Typography>
          <List>
            {consents
              ?.filter((c: any) => c.status === 'Active')
              .map((consent: any) => (
                <ListItem key={consent.id} divider>
                  <ListItemText
                    primary={
                      <Box display="flex" alignItems="center" gap={1}>
                        <Business fontSize="small" />
                        <Typography variant="subtitle1">
                          {consent.organizationName}
                        </Typography>
                        <Chip
                          label={consent.status}
                          size="small"
                          color={getStatusColor(consent.status) as any}
                          icon={getStatusIcon(consent.status) as any}
                        />
                      </Box>
                    }
                    secondary={
                      <Box mt={1}>
                        <Typography variant="body2" color="text.secondary">
                          <AccessTime fontSize="small" sx={{ mr: 0.5, verticalAlign: 'middle' }} />
                          Granted: {format(new Date(consent.consentDate), 'MMM d, yyyy h:mm a')}
                        </Typography>
                        {consent.expirationDate && (
                          <Typography variant="body2" color="text.secondary">
                            Expires: {format(new Date(consent.expirationDate), 'MMM d, yyyy')}
                          </Typography>
                        )}
                        <Box display="flex" flexWrap="wrap" gap={0.5} mt={1}>
                          {consent.scopes?.map((scope: string) => (
                            <Chip key={scope} label={scope} size="small" variant="outlined" />
                          ))}
                        </Box>
                      </Box>
                    }
                  />
                  <ListItemSecondaryAction>
                    <IconButton onClick={() => handleViewConsent(consent)}>
                      <Visibility />
                    </IconButton>
                    <IconButton
                      onClick={() => handleRevokeClick(consent)}
                      color="error"
                    >
                      <Delete />
                    </IconButton>
                  </ListItemSecondaryAction>
                </ListItem>
              ))}
          </List>

          {consents?.filter((c: any) => c.status === 'Active').length === 0 && (
            <Typography variant="body2" color="text.secondary" sx={{ py: 2, textAlign: 'center' }}>
              No active consents
            </Typography>
          )}
        </CardContent>
      </Card>

      <Card sx={{ mt: 3 }}>
        <CardContent>
          <Typography variant="h6" gutterBottom>
            Consent History
          </Typography>
          <List>
            {consents
              ?.filter((c: any) => c.status !== 'Active')
              .map((consent: any) => (
                <ListItem key={consent.id} divider>
                  <ListItemText
                    primary={
                      <Box display="flex" alignItems="center" gap={1}>
                        <Typography variant="subtitle1">
                          {consent.organizationName}
                        </Typography>
                        <Chip
                          label={consent.status}
                          size="small"
                          color={getStatusColor(consent.status) as any}
                        />
                      </Box>
                    }
                    secondary={
                      <Typography variant="body2" color="text.secondary">
                        {consent.status === 'Revoked'
                          ? `Revoked on ${format(new Date(consent.revokedDate), 'MMM d, yyyy')}`
                          : `Expired on ${format(new Date(consent.expirationDate), 'MMM d, yyyy')}`}
                      </Typography>
                    }
                  />
                  <ListItemSecondaryAction>
                    <IconButton onClick={() => handleViewConsent(consent)}>
                      <Visibility />
                    </IconButton>
                  </ListItemSecondaryAction>
                </ListItem>
              ))}
          </List>
        </CardContent>
      </Card>

      {/* View Consent Dialog */}
      <Dialog
        open={viewDialogOpen}
        onClose={() => setViewDialogOpen(false)}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>Consent Details</DialogTitle>
        <DialogContent dividers>
          {selectedConsent && (
            <Box>
              <Paper sx={{ p: 2, mb: 2, bgcolor: 'grey.50' }}>
                <Typography variant="subtitle2" gutterBottom>
                  Organization
                </Typography>
                <Typography variant="body1">
                  {selectedConsent.organizationName}
                </Typography>
              </Paper>

              <Paper sx={{ p: 2, mb: 2, bgcolor: 'grey.50' }}>
                <Typography variant="subtitle2" gutterBottom>
                  Status
                </Typography>
                <Chip
                  label={selectedConsent.status}
                  color={getStatusColor(selectedConsent.status) as any}
                  icon={getStatusIcon(selectedConsent.status) as any}
                />
              </Paper>

              <Paper sx={{ p: 2, mb: 2, bgcolor: 'grey.50' }}>
                <Typography variant="subtitle2" gutterBottom>
                  Timeline
                </Typography>
                <Typography variant="body2">
                  Granted: {format(new Date(selectedConsent.consentDate), 'MMM d, yyyy h:mm a')}
                </Typography>
                {selectedConsent.expirationDate && (
                  <Typography variant="body2">
                    Expires: {format(new Date(selectedConsent.expirationDate), 'MMM d, yyyy')}
                  </Typography>
                )}
                {selectedConsent.revokedDate && (
                  <Typography variant="body2">
                    Revoked: {format(new Date(selectedConsent.revokedDate), 'MMM d, yyyy')}
                  </Typography>
                )}
              </Paper>

              <Paper sx={{ p: 2, bgcolor: 'grey.50' }}>
                <Typography variant="subtitle2" gutterBottom>
                  Authorized Access
                </Typography>
                <Box display="flex" flexWrap="wrap" gap={0.5}>
                  {selectedConsent.scopes?.map((scope: string) => (
                    <Chip key={scope} label={scope} size="small" />
                  ))}
                </Box>
              </Paper>
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setViewDialogOpen(false)}>Close</Button>
        </DialogActions>
      </Dialog>

      {/* Revoke Consent Dialog */}
      <Dialog
        open={revokeDialogOpen}
        onClose={() => setRevokeDialogOpen(false)}
      >
        <DialogTitle>Revoke Consent?</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Are you sure you want to revoke consent for{' '}
            <strong>{selectedConsent?.organizationName}</strong>? They will
            immediately lose access to your medical records.
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setRevokeDialogOpen(false)}>Cancel</Button>
          <Button
            onClick={handleRevokeConfirm}
            color="error"
            variant="contained"
            disabled={revokeMutation.isPending}
          >
            Revoke Consent
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}