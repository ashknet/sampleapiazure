import React, { useState, useEffect } from 'react'
import { View, StyleSheet, Alert } from 'react-native'
import { BarCodeScanner } from 'expo-barcode-scanner'
import { Button, Card, Title, Paragraph, Portal, Dialog } from 'react-native-paper'
import { SafeAreaView } from 'react-native-safe-area-context'
import { useNavigation } from '@react-navigation/native'
import { consentService } from '../services/consentService'
import { LoadingScreen } from './LoadingScreen'

export function QrScannerScreen() {
  const navigation = useNavigation()
  const [hasPermission, setHasPermission] = useState<boolean | null>(null)
  const [scanned, setScanned] = useState(false)
  const [scanData, setScanData] = useState<any>(null)
  const [dialogVisible, setDialogVisible] = useState(false)
  const [isProcessing, setIsProcessing] = useState(false)

  useEffect(() => {
    (async () => {
      const { status } = await BarCodeScanner.requestPermissionsAsync()
      setHasPermission(status === 'granted')
    })()
  }, [])

  const handleBarCodeScanned = ({ type, data }: { type: string; data: string }) => {
    setScanned(true)
    
    // Parse QR code data
    if (data.startsWith('emr://connect/')) {
      const code = data.replace('emr://connect/', '')
      fetchConsentDetails(code)
    } else {
      Alert.alert('Invalid QR Code', 'This QR code is not from an EMR system.')
      setScanned(false)
    }
  }

  const fetchConsentDetails = async (code: string) => {
    try {
      const details = await consentService.getQrCodeDetails(code)
      setScanData({ code, ...details })
      setDialogVisible(true)
    } catch (error) {
      Alert.alert('Error', 'Unable to fetch consent details. Please try again.')
      setScanned(false)
    }
  }

  const handleAuthorize = async () => {
    if (!scanData) return
    
    setIsProcessing(true)
    try {
      await consentService.authorizeQrCode(scanData.code, {
        authorizedScopes: scanData.requestedScopes,
        consentDurationHours: 24,
      })
      
      Alert.alert(
        'Success',
        'You have successfully authorized access to your records.',
        [{ text: 'OK', onPress: () => navigation.goBack() }]
      )
    } catch (error) {
      Alert.alert('Error', 'Unable to authorize access. Please try again.')
    } finally {
      setIsProcessing(false)
      setDialogVisible(false)
      setScanned(false)
    }
  }

  const handleDeny = () => {
    setDialogVisible(false)
    setScanned(false)
    setScanData(null)
  }

  if (hasPermission === null) {
    return <LoadingScreen />
  }

  if (hasPermission === false) {
    return (
      <SafeAreaView style={styles.container}>
        <Card style={styles.card}>
          <Card.Content>
            <Title>Camera Permission Required</Title>
            <Paragraph>
              Please grant camera permission to scan QR codes for connecting with healthcare providers.
            </Paragraph>
          </Card.Content>
        </Card>
      </SafeAreaView>
    )
  }

  return (
    <View style={styles.container}>
      <BarCodeScanner
        onBarCodeScanned={scanned ? undefined : handleBarCodeScanned}
        style={StyleSheet.absoluteFillObject}
      />
      
      <View style={styles.overlay}>
        <View style={styles.scanArea} />
        <Paragraph style={styles.instructionText}>
          Point your camera at the QR code displayed at the healthcare facility
        </Paragraph>
      </View>

      {scanned && (
        <View style={styles.rescanContainer}>
          <Button
            mode="contained"
            onPress={() => setScanned(false)}
            style={styles.rescanButton}
          >
            Tap to Scan Again
          </Button>
        </View>
      )}

      <Portal>
        <Dialog visible={dialogVisible} onDismiss={handleDeny}>
          <Dialog.Title>Authorization Request</Dialog.Title>
          <Dialog.Content>
            <Paragraph style={styles.dialogText}>
              <Title style={styles.organizationName}>
                {scanData?.organizationName}
              </Title>
              {'\n\n'}
              is requesting access to the following information:
            </Paragraph>
            
            {scanData?.requestedScopes.map((scope: string, index: number) => (
              <Paragraph key={index} style={styles.scopeItem}>
                • {scope.replace(/:/g, ' ').replace(/_/g, ' ')}
              </Paragraph>
            ))}
            
            <Paragraph style={styles.expiryText}>
              Access will expire in 24 hours
            </Paragraph>
          </Dialog.Content>
          <Dialog.Actions>
            <Button onPress={handleDeny} disabled={isProcessing}>
              Deny
            </Button>
            <Button
              onPress={handleAuthorize}
              loading={isProcessing}
              disabled={isProcessing}
            >
              Authorize
            </Button>
          </Dialog.Actions>
        </Dialog>
      </Portal>
    </View>
  )
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  card: {
    margin: 20,
  },
  overlay: {
    flex: 1,
    backgroundColor: 'rgba(0,0,0,0.5)',
    justifyContent: 'center',
    alignItems: 'center',
  },
  scanArea: {
    width: 250,
    height: 250,
    borderWidth: 2,
    borderColor: 'white',
    borderRadius: 10,
  },
  instructionText: {
    color: 'white',
    marginTop: 20,
    fontSize: 16,
    textAlign: 'center',
    paddingHorizontal: 40,
  },
  rescanContainer: {
    position: 'absolute',
    bottom: 50,
    left: 0,
    right: 0,
    alignItems: 'center',
  },
  rescanButton: {
    paddingHorizontal: 30,
  },
  dialogText: {
    fontSize: 16,
    lineHeight: 24,
  },
  organizationName: {
    fontSize: 18,
    fontWeight: 'bold',
  },
  scopeItem: {
    fontSize: 14,
    marginLeft: 20,
    marginTop: 8,
  },
  expiryText: {
    marginTop: 16,
    fontStyle: 'italic',
    color: '#666',
  },
})