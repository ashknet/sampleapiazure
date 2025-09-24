import React, { useEffect } from 'react'
import { StatusBar } from 'expo-status-bar'
import { NavigationContainer } from '@react-navigation/native'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { Provider as PaperProvider } from 'react-native-paper'
import * as Notifications from 'expo-notifications'
import * as Linking from 'expo-linking'
import { AuthProvider } from './src/contexts/AuthContext'
import { RootNavigator } from './src/navigation/RootNavigator'
import { theme } from './src/utils/theme'

// Configure notifications
Notifications.setNotificationHandler({
  handleNotification: async () => ({
    shouldShowAlert: true,
    shouldPlaySound: true,
    shouldSetBadge: true,
  }),
})

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 5 * 60 * 1000, // 5 minutes
      retry: 1,
    },
  },
})

const linking = {
  prefixes: [Linking.createURL('/'), 'emrmobile://'],
  config: {
    screens: {
      Auth: {
        screens: {
          QrScanner: 'connect/:code',
        },
      },
    },
  },
}

export default function App() {
  useEffect(() => {
    // Request notification permissions
    const requestPermissions = async () => {
      const { status } = await Notifications.requestPermissionsAsync()
      if (status !== 'granted') {
        console.log('Notification permissions not granted')
      }
    }

    requestPermissions()

    // Handle deep links
    const handleDeepLink = (url: string) => {
      console.log('Deep link received:', url)
    }

    const subscription = Linking.addEventListener('url', ({ url }) => handleDeepLink(url))

    return () => {
      subscription.remove()
    }
  }, [])

  return (
    <QueryClientProvider client={queryClient}>
      <PaperProvider theme={theme}>
        <AuthProvider>
          <NavigationContainer linking={linking}>
            <StatusBar style="auto" />
            <RootNavigator />
          </NavigationContainer>
        </AuthProvider>
      </PaperProvider>
    </QueryClientProvider>
  )
}