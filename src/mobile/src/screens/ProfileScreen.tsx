import React from 'react'
import { View, StyleSheet } from 'react-native'
import { Title, Paragraph, Card, Button } from 'react-native-paper'
import { SafeAreaView } from 'react-native-safe-area-context'
import { useAuth } from '../contexts/AuthContext'

export function ProfileScreen() {
  const { user, logout } = useAuth()

  return (
    <SafeAreaView style={styles.container}>
      <Card style={styles.card}>
        <Card.Content>
          <Title>Profile</Title>
          <Paragraph>Name: {user?.name}</Paragraph>
          <Paragraph>Email: {user?.email}</Paragraph>
          <Button mode="contained" onPress={logout} style={styles.logoutButton}>
            Logout
          </Button>
        </Card.Content>
      </Card>
    </SafeAreaView>
  )
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f5f5',
  },
  card: {
    margin: 20,
  },
  logoutButton: {
    marginTop: 20,
  },
})