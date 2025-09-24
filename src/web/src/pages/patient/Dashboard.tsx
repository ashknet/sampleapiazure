import { useState, useEffect } from 'react'
import {
  Box,
  Card,
  CardContent,
  Grid,
  Typography,
  Paper,
  List,
  ListItem,
  ListItemText,
  ListItemIcon,
  Chip,
  Skeleton,
} from '@mui/material'
import {
  LocalHospital,
  Assignment,
  CalendarToday,
  Medication,
  Warning,
  FolderShared,
} from '@mui/icons-material'
import { format } from 'date-fns'
import { useQuery } from '@tanstack/react-query'
import { useAuth } from '../../hooks/useAuth'
import { patientService } from '../../services/patientService'

export function PatientDashboard() {
  const { user } = useAuth()
  const [currentTime, setCurrentTime] = useState(new Date())

  useEffect(() => {
    const timer = setInterval(() => setCurrentTime(new Date()), 60000)
    return () => clearInterval(timer)
  }, [])

  const { data: summary, isLoading } = useQuery({
    queryKey: ['patientSummary'],
    queryFn: () => patientService.getSummary(),
  })

  const { data: recentActivity } = useQuery({
    queryKey: ['recentActivity'],
    queryFn: () => patientService.getRecentActivity(),
  })

  const statCards = [
    {
      title: 'Active Medications',
      value: summary?.activeMedications || 0,
      icon: <Medication />,
      color: 'primary.main',
    },
    {
      title: 'Upcoming Appointments',
      value: summary?.upcomingAppointments || 0,
      icon: <CalendarToday />,
      color: 'success.main',
    },
    {
      title: 'Active Conditions',
      value: summary?.activeConditions || 0,
      icon: <LocalHospital />,
      color: 'warning.main',
    },
    {
      title: 'Shared Records',
      value: summary?.sharedRecords || 0,
      icon: <FolderShared />,
      color: 'info.main',
    },
  ]

  return (
    <Box>
      <Box mb={3}>
        <Typography variant="h4" gutterBottom>
          Welcome back, {user?.name?.split(' ')[0]}
        </Typography>
        <Typography variant="body1" color="text.secondary">
          {format(currentTime, 'EEEE, MMMM d, yyyy')}
        </Typography>
      </Box>

      <Grid container spacing={3}>
        {/* Stats Cards */}
        {statCards.map((stat) => (
          <Grid item xs={12} sm={6} md={3} key={stat.title}>
            <Card>
              <CardContent>
                <Box display="flex" alignItems="center" mb={2}>
                  <Box
                    sx={{
                      backgroundColor: stat.color,
                      borderRadius: 2,
                      p: 1,
                      mr: 2,
                      color: 'white',
                    }}
                  >
                    {stat.icon}
                  </Box>
                  <Box>
                    <Typography color="text.secondary" variant="body2">
                      {stat.title}
                    </Typography>
                    <Typography variant="h5">
                      {isLoading ? <Skeleton width={40} /> : stat.value}
                    </Typography>
                  </Box>
                </Box>
              </CardContent>
            </Card>
          </Grid>
        ))}

        {/* Recent Activity */}
        <Grid item xs={12} md={8}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Recent Activity
            </Typography>
            <List>
              {isLoading ? (
                Array.from({ length: 5 }).map((_, i) => (
                  <ListItem key={i}>
                    <Skeleton variant="rectangular" width="100%" height={60} />
                  </ListItem>
                ))
              ) : (
                recentActivity?.map((activity: any) => (
                  <ListItem key={activity.id} divider>
                    <ListItemIcon>
                      {activity.type === 'appointment' && <CalendarToday />}
                      {activity.type === 'document' && <Assignment />}
                      {activity.type === 'medication' && <Medication />}
                    </ListItemIcon>
                    <ListItemText
                      primary={activity.title}
                      secondary={format(new Date(activity.date), 'MMM d, yyyy h:mm a')}
                    />
                    <Chip
                      label={activity.status}
                      size="small"
                      color={activity.status === 'New' ? 'primary' : 'default'}
                    />
                  </ListItem>
                ))
              )}
            </List>
          </Paper>
        </Grid>

        {/* Alerts */}
        <Grid item xs={12} md={4}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Alerts & Reminders
            </Typography>
            <List>
              <ListItem>
                <ListItemIcon>
                  <Warning color="warning" />
                </ListItemIcon>
                <ListItemText
                  primary="Medication Refill Needed"
                  secondary="Lisinopril - Refill by Dec 15"
                />
              </ListItem>
              <ListItem>
                <ListItemIcon>
                  <CalendarToday color="info" />
                </ListItemIcon>
                <ListItemText
                  primary="Upcoming Appointment"
                  secondary="Dr. Smith - Dec 20 at 2:00 PM"
                />
              </ListItem>
            </List>
          </Paper>
        </Grid>
      </Grid>
    </Box>
  )
}