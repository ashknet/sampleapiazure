import { Box, Grid, Card, CardContent, Typography, Paper } from '@mui/material'
import {
  People,
  Assignment,
  CalendarToday,
  QrCode2,
  TrendingUp,
  AccessTime,
} from '@mui/icons-material'
import { useQuery } from '@tanstack/react-query'
import { hospitalService } from '../../services/hospitalService'
import { useAuth } from '../../hooks/useAuth'
import { format } from 'date-fns'

export function HospitalDashboard() {
  const { user } = useAuth()

  const { data: stats } = useQuery({
    queryKey: ['hospitalStats'],
    queryFn: () => hospitalService.getDashboardStats(),
  })

  const statCards = [
    {
      title: 'Patients Today',
      value: stats?.patientsToday || 0,
      icon: <People />,
      color: 'primary.main',
      trend: '+12%',
    },
    {
      title: 'Appointments',
      value: stats?.appointmentsToday || 0,
      icon: <CalendarToday />,
      color: 'success.main',
      trend: '+5%',
    },
    {
      title: 'Documents Uploaded',
      value: stats?.documentsToday || 0,
      icon: <Assignment />,
      color: 'warning.main',
      trend: '+18%',
    },
    {
      title: 'QR Connections',
      value: stats?.qrConnectionsToday || 0,
      icon: <QrCode2 />,
      color: 'info.main',
      trend: '+25%',
    },
  ]

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Hospital Dashboard
      </Typography>
      <Typography variant="body1" color="text.secondary" gutterBottom>
        Welcome back, {user?.name}
      </Typography>

      <Grid container spacing={3} sx={{ mt: 1 }}>
        {statCards.map((stat) => (
          <Grid item xs={12} sm={6} md={3} key={stat.title}>
            <Card>
              <CardContent>
                <Box display="flex" justifyContent="space-between" alignItems="start">
                  <Box>
                    <Typography color="text.secondary" variant="body2" gutterBottom>
                      {stat.title}
                    </Typography>
                    <Typography variant="h4">{stat.value}</Typography>
                    <Box display="flex" alignItems="center" mt={1}>
                      <TrendingUp fontSize="small" color="success" />
                      <Typography variant="body2" color="success.main" sx={{ ml: 0.5 }}>
                        {stat.trend}
                      </Typography>
                    </Box>
                  </Box>
                  <Box
                    sx={{
                      backgroundColor: stat.color,
                      borderRadius: 2,
                      p: 1,
                      color: 'white',
                    }}
                  >
                    {stat.icon}
                  </Box>
                </Box>
              </CardContent>
            </Card>
          </Grid>
        ))}

        <Grid item xs={12} md={8}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Recent Activities
            </Typography>
            <Box sx={{ mt: 2 }}>
              <Typography variant="body2" color="text.secondary">
                Recent patient connections and document uploads will appear here
              </Typography>
            </Box>
          </Paper>
        </Grid>

        <Grid item xs={12} md={4}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Quick Actions
            </Typography>
            <Box sx={{ mt: 2 }}>
              <Typography variant="body2" color="text.secondary">
                Quick action buttons will appear here
              </Typography>
            </Box>
          </Paper>
        </Grid>
      </Grid>
    </Box>
  )
}