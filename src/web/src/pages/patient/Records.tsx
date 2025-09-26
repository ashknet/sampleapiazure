import { useState } from 'react'
import {
  Box,
  Card,
  CardContent,
  Typography,
  Tabs,
  Tab,
  List,
  ListItem,
  ListItemText,
  ListItemIcon,
  Chip,
  Button,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
} from '@mui/material'
import {
  Medication,
  Warning,
  LocalHospital,
  Assignment,
  Science,
  Vaccines,
  Download,
  Visibility,
  Close,
} from '@mui/icons-material'
import { format } from 'date-fns'
import { useQuery } from '@tanstack/react-query'
import { patientService } from '../../services/patientService'

interface TabPanelProps {
  children?: React.ReactNode
  index: number
  value: number
}

function TabPanel(props: TabPanelProps) {
  const { children, value, index, ...other } = props

  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`records-tabpanel-${index}`}
      aria-labelledby={`records-tab-${index}`}
      {...other}
    >
      {value === index && <Box sx={{ py: 3 }}>{children}</Box>}
    </div>
  )
}

export function PatientRecords() {
  const [tabValue, setTabValue] = useState(0)
  const [selectedRecord, setSelectedRecord] = useState<any>(null)
  const [viewDialogOpen, setViewDialogOpen] = useState(false)

  const { data: records, isLoading } = useQuery({
    queryKey: ['patientRecords', tabValue],
    queryFn: () => {
      const categories = ['medications', 'conditions', 'allergies', 'labs', 'immunizations', 'documents']
      return patientService.getRecords(categories[tabValue])
    },
  })

  const handleTabChange = (_: React.SyntheticEvent, newValue: number) => {
    setTabValue(newValue)
  }

  const handleViewRecord = (record: any) => {
    setSelectedRecord(record)
    setViewDialogOpen(true)
  }

  const getRecordIcon = (type: string) => {
    switch (type) {
      case 'medication':
        return <Medication />
      case 'condition':
        return <LocalHospital />
      case 'allergy':
        return <Warning />
      case 'lab':
        return <Science />
      case 'immunization':
        return <Vaccines />
      case 'document':
        return <Assignment />
      default:
        return <Assignment />
    }
  }

  const getStatusColor = (status: string) => {
    switch (status.toLowerCase()) {
      case 'active':
        return 'success'
      case 'inactive':
        return 'default'
      case 'resolved':
        return 'info'
      case 'severe':
        return 'error'
      default:
        return 'default'
    }
  }

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        My Medical Records
      </Typography>
      <Typography variant="body1" color="text.secondary" gutterBottom>
        View your complete medical history organized by category
      </Typography>

      <Card sx={{ mt: 3 }}>
        <Tabs
          value={tabValue}
          onChange={handleTabChange}
          variant="scrollable"
          scrollButtons="auto"
          sx={{ borderBottom: 1, borderColor: 'divider' }}
        >
          <Tab label="Medications" icon={<Medication />} iconPosition="start" />
          <Tab label="Conditions" icon={<LocalHospital />} iconPosition="start" />
          <Tab label="Allergies" icon={<Warning />} iconPosition="start" />
          <Tab label="Lab Results" icon={<Science />} iconPosition="start" />
          <Tab label="Immunizations" icon={<Vaccines />} iconPosition="start" />
          <Tab label="Documents" icon={<Assignment />} iconPosition="start" />
        </Tabs>

        <CardContent>
          <TabPanel value={tabValue} index={0}>
            <List>
              {records?.map((med: any) => (
                <ListItem
                  key={med.id}
                  secondaryAction={
                    <IconButton onClick={() => handleViewRecord(med)}>
                      <Visibility />
                    </IconButton>
                  }
                >
                  <ListItemIcon>{getRecordIcon('medication')}</ListItemIcon>
                  <ListItemText
                    primary={med.name}
                    secondary={`${med.dosage} - ${med.frequency}`}
                  />
                  <Chip
                    label={med.status}
                    size="small"
                    color={getStatusColor(med.status) as any}
                    sx={{ mr: 2 }}
                  />
                </ListItem>
              ))}
            </List>
          </TabPanel>

          <TabPanel value={tabValue} index={1}>
            <List>
              {records?.map((condition: any) => (
                <ListItem
                  key={condition.id}
                  secondaryAction={
                    <IconButton onClick={() => handleViewRecord(condition)}>
                      <Visibility />
                    </IconButton>
                  }
                >
                  <ListItemIcon>{getRecordIcon('condition')}</ListItemIcon>
                  <ListItemText
                    primary={condition.name}
                    secondary={`Diagnosed: ${format(new Date(condition.dateRecorded), 'MMM d, yyyy')}`}
                  />
                  <Chip
                    label={condition.status}
                    size="small"
                    color={getStatusColor(condition.status) as any}
                    sx={{ mr: 2 }}
                  />
                </ListItem>
              ))}
            </List>
          </TabPanel>

          <TabPanel value={tabValue} index={2}>
            <List>
              {records?.map((allergy: any) => (
                <ListItem
                  key={allergy.id}
                  secondaryAction={
                    <IconButton onClick={() => handleViewRecord(allergy)}>
                      <Visibility />
                    </IconButton>
                  }
                >
                  <ListItemIcon>{getRecordIcon('allergy')}</ListItemIcon>
                  <ListItemText
                    primary={allergy.substance}
                    secondary={`Reaction: ${allergy.reaction}`}
                  />
                  <Chip
                    label={allergy.severity}
                    size="small"
                    color={getStatusColor(allergy.severity) as any}
                    sx={{ mr: 2 }}
                  />
                </ListItem>
              ))}
            </List>
          </TabPanel>

          <TabPanel value={tabValue} index={3}>
            <List>
              {records?.map((lab: any) => (
                <ListItem
                  key={lab.id}
                  secondaryAction={
                    <IconButton onClick={() => handleViewRecord(lab)}>
                      <Visibility />
                    </IconButton>
                  }
                >
                  <ListItemIcon>{getRecordIcon('lab')}</ListItemIcon>
                  <ListItemText
                    primary={lab.testName}
                    secondary={`Performed: ${format(new Date(lab.date), 'MMM d, yyyy')}`}
                  />
                  {lab.abnormal && (
                    <Chip
                      label="Abnormal"
                      size="small"
                      color="warning"
                      sx={{ mr: 2 }}
                    />
                  )}
                </ListItem>
              ))}
            </List>
          </TabPanel>

          <TabPanel value={tabValue} index={4}>
            <List>
              {records?.map((immunization: any) => (
                <ListItem
                  key={immunization.id}
                  secondaryAction={
                    <IconButton onClick={() => handleViewRecord(immunization)}>
                      <Visibility />
                    </IconButton>
                  }
                >
                  <ListItemIcon>{getRecordIcon('immunization')}</ListItemIcon>
                  <ListItemText
                    primary={immunization.vaccine}
                    secondary={`Given: ${format(new Date(immunization.date), 'MMM d, yyyy')}`}
                  />
                  <Chip
                    label="Completed"
                    size="small"
                    color="success"
                    sx={{ mr: 2 }}
                  />
                </ListItem>
              ))}
            </List>
          </TabPanel>

          <TabPanel value={tabValue} index={5}>
            <List>
              {records?.map((doc: any) => (
                <ListItem
                  key={doc.id}
                  secondaryAction={
                    <Box>
                      <IconButton onClick={() => handleViewRecord(doc)}>
                        <Visibility />
                      </IconButton>
                      <IconButton>
                        <Download />
                      </IconButton>
                    </Box>
                  }
                >
                  <ListItemIcon>{getRecordIcon('document')}</ListItemIcon>
                  <ListItemText
                    primary={doc.title}
                    secondary={`${doc.type} - ${format(new Date(doc.date), 'MMM d, yyyy')}`}
                  />
                </ListItem>
              ))}
            </List>
          </TabPanel>
        </CardContent>
      </Card>

      {/* View Dialog */}
      <Dialog
        open={viewDialogOpen}
        onClose={() => setViewDialogOpen(false)}
        maxWidth="md"
        fullWidth
      >
        <DialogTitle>
          <Box display="flex" alignItems="center" justifyContent="space-between">
            <Typography variant="h6">Record Details</Typography>
            <IconButton onClick={() => setViewDialogOpen(false)}>
              <Close />
            </IconButton>
          </Box>
        </DialogTitle>
        <DialogContent dividers>
          {selectedRecord && (
            <Box>
              <Typography variant="subtitle1" gutterBottom>
                {selectedRecord.name || selectedRecord.title}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                {/* Add more detailed view based on record type */}
                Details would be shown here...
              </Typography>
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setViewDialogOpen(false)}>Close</Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}