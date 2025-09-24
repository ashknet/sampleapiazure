import { DefaultTheme } from 'react-native-paper'

export const theme = {
  ...DefaultTheme,
  colors: {
    ...DefaultTheme.colors,
    primary: '#2196F3',
    accent: '#FF5722',
    background: '#f5f5f5',
    surface: '#ffffff',
    error: '#F44336',
    text: '#212121',
    onSurface: '#000000',
    disabled: '#9E9E9E',
    placeholder: '#757575',
    backdrop: 'rgba(0, 0, 0, 0.5)',
    notification: '#FF9800',
  },
  roundness: 8,
}