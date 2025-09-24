import React from 'react'
import { View, StyleSheet, Image } from 'react-native'
import { Button, Title, Paragraph, Card } from 'react-native-paper'
import { SafeAreaView } from 'react-native-safe-area-context'
import { useAuth } from '../contexts/AuthContext'

export function LoginScreen() {
  const { login } = useAuth()

  return (
    <SafeAreaView style={styles.container}>
      <View style={styles.content}>
        <View style={styles.logoContainer}>
          <Image
            source={require('../../assets/icon.png')}
            style={styles.logo}
            resizeMode="contain"
          />
        </View>
        
        <Card style={styles.card}>
          <Card.Content>
            <Title style={styles.title}>EMR Mobile</Title>
            <Paragraph style={styles.subtitle}>
              Your medical records, anywhere, anytime
            </Paragraph>
            
            <Button
              mode="contained"
              onPress={login}
              style={styles.loginButton}
              contentStyle={styles.loginButtonContent}
            >
              Sign In with Azure AD B2C
            </Button>
            
            <Paragraph style={styles.securityText}>
              Secure authentication powered by Microsoft
            </Paragraph>
          </Card.Content>
        </Card>
      </View>
    </SafeAreaView>
  )
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f5f5',
  },
  content: {
    flex: 1,
    justifyContent: 'center',
    padding: 20,
  },
  logoContainer: {
    alignItems: 'center',
    marginBottom: 40,
  },
  logo: {
    width: 120,
    height: 120,
  },
  card: {
    elevation: 4,
  },
  title: {
    fontSize: 28,
    textAlign: 'center',
    marginBottom: 10,
  },
  subtitle: {
    fontSize: 16,
    textAlign: 'center',
    marginBottom: 30,
    color: '#666',
  },
  loginButton: {
    marginVertical: 20,
  },
  loginButtonContent: {
    paddingVertical: 8,
  },
  securityText: {
    textAlign: 'center',
    fontSize: 12,
    color: '#999',
  },
})