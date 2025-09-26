import React from 'react'
import { View, ScrollView, StyleSheet } from 'react-native'
import { Card, Title, Paragraph, List, Avatar, Chip, useTheme } from 'react-native-paper'
import { SafeAreaView } from 'react-native-safe-area-context'
import { useQuery } from '@tanstack/react-query'
import { format } from 'date-fns'
import { useAuth } from '../contexts/AuthContext'
import { patientService } from '../services/patientService'

export function DashboardScreen() {
  const theme = useTheme()
  const { user } = useAuth()

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
      icon: 'pill',
      color: theme.colors.primary,
    },
    {
      title: 'Upcoming Appointments',
      value: summary?.upcomingAppointments || 0,
      icon: 'calendar',
      color: theme.colors.accent,
    },
    {
      title: 'Active Conditions',
      value: summary?.activeConditions || 0,
      icon: 'hospital',
      color: '#FF9800',
    },
    {
      title: 'Shared Records',
      value: summary?.sharedRecords || 0,
      icon: 'folder-shared',
      color: '#4CAF50',
    },
  ]

  return (
    <SafeAreaView style={styles.container}>
      <ScrollView showsVerticalScrollIndicator={false}>
        <View style={styles.header}>
          <Title style={styles.greeting}>Welcome back, {user?.name?.split(' ')[0]}</Title>
          <Paragraph style={styles.date}>
            {format(new Date(), 'EEEE, MMMM d, yyyy')}
          </Paragraph>
        </View>

        <View style={styles.statsGrid}>
          {statCards.map((stat, index) => (
            <Card key={index} style={styles.statCard}>
              <Card.Content style={styles.statContent}>
                <Avatar.Icon
                  size={40}
                  icon={stat.icon}
                  style={{ backgroundColor: stat.color }}
                />
                <Title style={styles.statValue}>{stat.value}</Title>
                <Paragraph style={styles.statLabel}>{stat.title}</Paragraph>
              </Card.Content>
            </Card>
          ))}
        </View>

        <Card style={styles.activityCard}>
          <Card.Title title="Recent Activity" />
          <Card.Content>
            {recentActivity?.length === 0 && (
              <Paragraph style={styles.emptyText}>No recent activity</Paragraph>
            )}
            {recentActivity?.map((activity: any) => (
              <List.Item
                key={activity.id}
                title={activity.title}
                description={format(new Date(activity.date), 'MMM d, yyyy h:mm a')}
                left={(props) => <List.Icon {...props} icon={getActivityIcon(activity.type)} />}
                right={() => (
                  <Chip mode="outlined" textStyle={{ fontSize: 12 }}>
                    {activity.status}
                  </Chip>
                )}
              />
            ))}
          </Card.Content>
        </Card>
      </ScrollView>
    </SafeAreaView>
  )
}

function getActivityIcon(type: string) {
  switch (type) {
    case 'appointment':
      return 'calendar'
    case 'document':
      return 'file-document'
    case 'medication':
      return 'pill'
    default:
      return 'information'
  }
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f5f5',
  },
  header: {
    padding: 20,
  },
  greeting: {
    fontSize: 24,
    fontWeight: 'bold',
  },
  date: {
    fontSize: 16,
    color: '#666',
  },
  statsGrid: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    paddingHorizontal: 10,
  },
  statCard: {
    width: '47%',
    margin: '1.5%',
    elevation: 2,
  },
  statContent: {
    alignItems: 'center',
    paddingVertical: 20,
  },
  statValue: {
    fontSize: 28,
    fontWeight: 'bold',
    marginTop: 10,
  },
  statLabel: {
    fontSize: 14,
    color: '#666',
    textAlign: 'center',
  },
  activityCard: {
    margin: 20,
    marginTop: 10,
    elevation: 2,
  },
  emptyText: {
    textAlign: 'center',
    color: '#999',
    paddingVertical: 20,
  },
})