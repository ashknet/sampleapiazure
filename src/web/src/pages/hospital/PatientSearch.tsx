import { useState } from 'react'
import {
  Box,
  TextField,
  InputAdornment,
  Card,
  CardContent,
  Typography,
  List,
  ListItem,
  ListItemText,
  ListItemAvatar,
  Avatar,
  Chip,
  CircularProgress,
  Alert,
  IconButton,
} from '@mui/material'
import { Search, Person, Visibility } from '@mui/icons-material'
import { useQuery } from '@tanstack/react-query'
import { hospitalService } from '../../services/hospitalService'
import { useDebounce } from '../../hooks/useDebounce'
import { format } from 'date-fns'

export function HospitalPatientSearch() {
  const [searchQuery, setSearchQuery] = useState('')
  const debouncedQuery = useDebounce(searchQuery, 500)

  const { data: searchResults, isLoading, error } = useQuery({
    queryKey: ['patientSearch', debouncedQuery],
    queryFn: () => hospitalService.searchPatients(debouncedQuery),
    enabled: debouncedQuery.length >= 3,
  })

  const handleViewPatient = (patientId: string) => {
    // Navigate to patient details or open modal
    console.log('View patient:', patientId)
  }

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Patient Search
      </Typography>
      <Typography variant="body1" color="text.secondary" gutterBottom>
        Search for patients by name, MRN, or date of birth
      </Typography>

      <Card sx={{ mt: 3 }}>
        <CardContent>
          <TextField
            fullWidth
            placeholder="Search patients..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <Search />
                </InputAdornment>
              ),
            }}
            helperText="Enter at least 3 characters to search"
          />

          {isLoading && (
            <Box display="flex" justifyContent="center" mt={4}>
              <CircularProgress />
            </Box>
          )}

          {error && (
            <Alert severity="error" sx={{ mt: 2 }}>
              An error occurred while searching. Please try again.
            </Alert>
          )}

          {searchResults && searchResults.length === 0 && (
            <Alert severity="info" sx={{ mt: 2 }}>
              No patients found matching your search criteria.
            </Alert>
          )}

          {searchResults && searchResults.length > 0 && (
            <List sx={{ mt: 2 }}>
              {searchResults.map((patient: any) => (
                <ListItem
                  key={patient.id}
                  divider
                  secondaryAction={
                    <IconButton onClick={() => handleViewPatient(patient.id)}>
                      <Visibility />
                    </IconButton>
                  }
                >
                  <ListItemAvatar>
                    <Avatar>
                      <Person />
                    </Avatar>
                  </ListItemAvatar>
                  <ListItemText
                    primary={
                      <Box display="flex" alignItems="center" gap={1}>
                        <Typography variant="subtitle1">
                          {patient.firstName} {patient.lastName}
                        </Typography>
                        {patient.hasActiveConsent && (
                          <Chip
                            label="Consent Active"
                            size="small"
                            color="success"
                          />
                        )}
                      </Box>
                    }
                    secondary={
                      <Box>
                        <Typography variant="body2" color="text.secondary">
                          MRN: {patient.medicalRecordNumber} | DOB:{' '}
                          {format(new Date(patient.dateOfBirth), 'MM/dd/yyyy')}
                        </Typography>
                        {patient.lastVisit && (
                          <Typography variant="body2" color="text.secondary">
                            Last visit:{' '}
                            {format(new Date(patient.lastVisit), 'MMM d, yyyy')}
                          </Typography>
                        )}
                      </Box>
                    }
                  />
                </ListItem>
              ))}
            </List>
          )}
        </CardContent>
      </Card>

      <Alert severity="warning" sx={{ mt: 2 }}>
        You can only access patient records if you have active consent or if the patient
        is currently under your care.
      </Alert>
    </Box>
  )
}