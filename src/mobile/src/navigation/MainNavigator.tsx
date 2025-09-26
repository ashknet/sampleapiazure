import React from 'react'
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs'
import { createStackNavigator } from '@react-navigation/stack'
import { Ionicons } from '@expo/vector-icons'
import { useTheme } from 'react-native-paper'

// Screens
import { DashboardScreen } from '../screens/DashboardScreen'
import { RecordsScreen } from '../screens/RecordsScreen'
import { QrScannerScreen } from '../screens/QrScannerScreen'
import { ConsentsScreen } from '../screens/ConsentsScreen'
import { ProfileScreen } from '../screens/ProfileScreen'
import { ConsentDetailsScreen } from '../screens/ConsentDetailsScreen'
import { RecordDetailsScreen } from '../screens/RecordDetailsScreen'

const Tab = createBottomTabNavigator()
const Stack = createStackNavigator()

function DashboardStack() {
  return (
    <Stack.Navigator>
      <Stack.Screen 
        name="DashboardHome" 
        component={DashboardScreen} 
        options={{ title: 'Dashboard' }}
      />
    </Stack.Navigator>
  )
}

function RecordsStack() {
  return (
    <Stack.Navigator>
      <Stack.Screen 
        name="RecordsList" 
        component={RecordsScreen} 
        options={{ title: 'My Records' }}
      />
      <Stack.Screen 
        name="RecordDetails" 
        component={RecordDetailsScreen} 
        options={{ title: 'Record Details' }}
      />
    </Stack.Navigator>
  )
}

function ConsentsStack() {
  return (
    <Stack.Navigator>
      <Stack.Screen 
        name="ConsentsList" 
        component={ConsentsScreen} 
        options={{ title: 'Consents' }}
      />
      <Stack.Screen 
        name="ConsentDetails" 
        component={ConsentDetailsScreen} 
        options={{ title: 'Consent Details' }}
      />
    </Stack.Navigator>
  )
}

export function MainNavigator() {
  const theme = useTheme()

  return (
    <Tab.Navigator
      screenOptions={({ route }) => ({
        tabBarIcon: ({ focused, color, size }) => {
          let iconName: keyof typeof Ionicons.glyphMap = 'home'

          switch (route.name) {
            case 'Dashboard':
              iconName = focused ? 'home' : 'home-outline'
              break
            case 'Records':
              iconName = focused ? 'document-text' : 'document-text-outline'
              break
            case 'QR':
              iconName = focused ? 'qr-code' : 'qr-code-outline'
              break
            case 'Consents':
              iconName = focused ? 'shield-checkmark' : 'shield-checkmark-outline'
              break
            case 'Profile':
              iconName = focused ? 'person' : 'person-outline'
              break
          }

          return <Ionicons name={iconName} size={size} color={color} />
        },
        tabBarActiveTintColor: theme.colors.primary,
        tabBarInactiveTintColor: 'gray',
        headerShown: false,
      })}
    >
      <Tab.Screen name="Dashboard" component={DashboardStack} />
      <Tab.Screen name="Records" component={RecordsStack} />
      <Tab.Screen 
        name="QR" 
        component={QrScannerScreen} 
        options={{ title: 'Scan QR' }}
      />
      <Tab.Screen name="Consents" component={ConsentsStack} />
      <Tab.Screen name="Profile" component={ProfileScreen} />
    </Tab.Navigator>
  )
}