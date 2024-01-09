using UnityEngine;
using System;
using System.Collections;

public class BluetoothDeviceScanner : MonoBehaviour
{
    private bool _scanning = false;
    private bool _connected = false;
    private string targetDeviceName = "PCA004";
    private string serviceUUIDName = "8aad8cd4-3830-45ee-a13e-74f0b01013ce"; // This should be the correct service UUID for your device
    private string characteristicUUIDName = "8aad46ab-3830-45ee-a13e-74f0b01013ce"; // This should be the correct characteristic UUID for your device

    void Start()
    {
        BluetoothLEHardwareInterface.Initialize(true, false, () =>
        {
            Debug.Log("BluetoothLE Initialized.");
            StartScan();
        }, (error) =>
        {
            Debug.LogError($"BluetoothLE Initialization Error: {error}");
        });
    }

    void Update()
    {
        if (!_connected && !_scanning)
        {
            StartScan();
        }
    }

    void StartScan()
    {
        _scanning = true;
        Debug.Log("Starting Scan...");

        BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(null, (address, name) =>
        {
            if (name == targetDeviceName)
            {
                _scanning = false;
                BluetoothLEHardwareInterface.StopScan();
                Debug.Log("Found target device.");

                BluetoothLEHardwareInterface.ConnectToPeripheral(address, (address) => {
                    // Connected
                }, (serviceUUID, characteristicUUID) => {
                    // Service discovered
                }, (address, serviceUUID, characteristicUUID) => {
                    _connected = true;
                    Debug.Log("Connected to target device.");
                    Debug.Log($"Device Address: {address}");
                    Debug.Log($"Service UUID: {serviceUUID}");
                    Debug.Log($"Characteristic UUID: {characteristicUUID}");

                    if (serviceUUID == serviceUUIDName && characteristicUUID == characteristicUUIDName)
                    {
                        Debug.Log("Correct Service and Characteristic UUIDs found.");
                        // Once connected, you might want to start a coroutine to read periodically
                        StartCoroutine(ReadCharacteristicRoutine(address, serviceUUID, characteristicUUID));
                    }
                }, (disconnectAddress) => {
                    // Disconnected
                    _connected = false;
                    Debug.Log($"Device disconnected: {disconnectAddress}");
                });
            }
        }, (error) => {
            // Handle error
            Debug.LogError($"Scan error: {error}");
        });
    }

    private IEnumerator ReadCharacteristicRoutine(string address, string serviceUUID, string characteristicUUID)
    {
        while (_connected)
        {
            // BluetoothLEHardwareInterface.ReadCharacteristic(address, serviceUUID, characteristicUUID, (characteristicUUID, data) =>
            // {
            //     Debug.Log("Read characteristic data.");
            //     Debug.Log($"Data: {BitConverter.ToString(data)}");
            // });

            //trying a fix for this error that i see

BluetoothLEHardwareInterface.ReadCharacteristic(address, serviceUUID, characteristicUUID, (characteristicUUID, data, dataLength, rawData) =>
{
    Debug.Log("Read characteristic data.");
    Debug.Log($"Data: {BitConverter.ToString(rawData)}");
});



            yield return new WaitForSeconds(30); // Wait for 30 seconds before reading again
        }
    }
}
