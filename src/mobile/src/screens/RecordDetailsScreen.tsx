import React from 'react'
import { View, StyleSheet } from 'react-native'
import { Title, Paragraph, Card } from 'react-native-paper'
import { SafeAreaView } from 'react-native-safe-area-context'

export function RecordDetailsScreen() {
  return (
    <SafeAreaView style={styles.container}>
      <Card style={styles.card}>
        <Card.Content>
          <Title>Record Details</Title>
          <Paragraph>Detailed record information will be shown here</Paragraph>
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
})