using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Threading.Tasks;


namespace App1
{
    /// <summary>
    /// Histamine Control Pane version 0.1 ALPHA
    /// Program on startup retrieves necessary objectst to send a message to the histamine sensing apparatus.
    /// Bence Halpern
    /// 2017.04.01.
    /// </summary>
    /// 
   
    public sealed partial class MainPage : Page
    {

        public List<string> deviceList = new List<string>();
        GattDeviceService bleService;
        GattCharacteristic accData;
        GattCharacteristic accConfig;
        DevicePairingResult dpr;
        bool paired = false;
        bool firstUpdate = true;


        public MainPage()
        {
            this.InitializeComponent();
            Debug.WriteLine("A");
            LoadedState();
            Debug.WriteLine("B");
        }
        private async void LoadedState()
        {

          /*  while (await pairHandler() == "Disconnected")
            {
                Debug.WriteLine("Connection failed... Trying again.");
            } */
           //enumerateSnapshot();
            var taskDevices = await getDevices();
            Debug.WriteLine("G");
           sendMessage("Hello World");
        }
        public async Task<List<string>> getDevices()
        {
            Debug.WriteLine("C");
            /* foreach (DeviceInformation di in await DeviceInformation.FindAllAsync(GattDeviceService.GetDeviceSelectorFromUuid(new Guid("0000ffe0-0000-1000-8000-00805f9b34fb"))))
             {
                 Debug.WriteLine("D");
                 Debug.WriteLine(di.Name);
                 Debug.WriteLine(di.Id);
                 Debug.WriteLine(di.IsEnabled);
                 GattDeviceService bleService = await GattDeviceService.FromIdAsync(di.Id);
                 Debug.WriteLine(bleService.Device.ConnectionStatus.ToString());
                 Debug.WriteLine("E");
                 Debug.WriteLine(GattCharacteristic.ConvertShortIdToUuid(0xFFE1).ToString());
                 // TODO: Throws exception if not paired before startup because pairing info is not stored on device
                 accConfig = bleService.GetCharacteristics(GattCharacteristic.ConvertShortIdToUuid(0xFFE1))[0];
                 Debug.WriteLine("F");
             } */
            foreach (DeviceInformation di in await DeviceInformation.FindAllAsync(GattDeviceService.GetDeviceSelectorFromUuid(new Guid("00002220-0000-1000-8000-00805f9b34fb"))))
            {
                Debug.WriteLine("D");
                Debug.WriteLine(di.Name);
                GattDeviceService bleService = await GattDeviceService.FromIdAsync(di.Id);
                Debug.WriteLine(bleService.Device.ConnectionStatus.ToString());
                //Debug.WriteLine(GattCharacteristic.ConvertShortIdToUuid(0x2222).ToString());
                accConfig = bleService.GetCharacteristics(GattCharacteristic.ConvertShortIdToUuid(0x2222))[0];
                Debug.WriteLine("E:Write_char");
                accData = bleService.GetCharacteristics(GattCharacteristic.ConvertShortIdToUuid(0x2221))[0];
                accData.ValueChanged += accData_ValueChanged;
                // I have no idea what the next line does
                await accData.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                Debug.WriteLine("F:Read_char");

            }
                return deviceList;
        }


        async void accData_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var allData = (await sender.ReadValueAsync()).Value.ToArray();
            string newData = System.Text.Encoding.UTF8.GetString(allData);
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                // firstUpdate clears default text
                if (firstUpdate)
                {
                    textBox.Text = newData;
                    firstUpdate = false;
                }
                else
                {
                    textBox.Text += newData;
                }
                textBox.Text += "\n";
            });
        }
        async Task<string> pairHandler()
        {
            DeviceInformationCollection collection = await DeviceInformation.FindAllAsync(BluetoothLEDevice.GetDeviceSelectorFromDeviceName("HIST"));
            DeviceInformation foundDevice = collection[0];
            BluetoothLEDevice egydevice = await BluetoothLEDevice.FromIdAsync(foundDevice.Id);
            if (egydevice.ConnectionStatus.ToString() == "Disconnected")
            {
                dpr = await foundDevice.Pairing.PairAsync(DevicePairingProtectionLevel.None);
                return egydevice.ConnectionStatus.ToString();
            }
            return egydevice.ConnectionStatus.ToString();
        }
        async void enumerateSnapshot()
        {
            // Finding HIST Device
   
            DeviceInformationCollection collection = await DeviceInformation.FindAllAsync(BluetoothLEDevice.GetDeviceSelectorFromPairingState(true));
            Debug.WriteLine(collection.Count());
            if (collection.Count() != 1)
            {
                Debug.WriteLine("HIST sensor unpaired - pairing process started");
                collection = await DeviceInformation.FindAllAsync(BluetoothLEDevice.GetDeviceSelectorFromDeviceName("HIST"));
                for (int i = 0; i < collection.Count(); i++)
                {
                    if (collection[i].Name.ToString() == "HIST")
                    {
                        Debug.WriteLine("Found HIST sensor");
                        DeviceInformation foundDevice = collection[i];
                        dpr = await foundDevice.Pairing.PairAsync(DevicePairingProtectionLevel.None); // Pair
                        Debug.WriteLine(foundDevice.Id);
                        BluetoothLEDevice egydevice = await BluetoothLEDevice.FromIdAsync(foundDevice.Id);
                        GattDeviceService egyservice = egydevice.GetGattService(new Guid("0000ffe0-0000-1000-8000-00805f9b34fb"));
                        
                        Debug.WriteLine(GattCharacteristic.ConvertShortIdToUuid(0xFFE1).ToString());
                        Debug.WriteLine("success with dpr");
                    
                        Debug.WriteLine("Sucess with GATT");

                    }
                }
            }
            else
            {
                Debug.WriteLine("HIST already paired");
                DeviceInformation foundDevice = collection[0];
               // bleService = await BluetoothLEDevice.FromIdAsync(foundDevice.Id);
                Debug.WriteLine(bleService.DeviceId);
              //  Debug.WriteLine(bleService.ConnectionStatus.ToString());
              //  Debug.WriteLine(bleService.GattServices);
               // GattDeviceService gattService = bleService.GetGattService(GattCharacteristic.ConvertShortIdToUuid(0xFFE1));
                Debug.WriteLine("Service done");
            }
            
          }
        public async void sendMessage(string messageToSend)
        {
            // Worldlimit to send is 20 - longer messages need to be partitioned into 20
            // or smaller chunks depending on chunk size
      
            var ChunksToSend = ChunksUpto(messageToSend, 20);
            foreach (string Chunks in ChunksToSend)
            {
                //await accConfig.WriteValueAsync((Encoding.UTF8.GetBytes(messageToSend)).AsBuffer());
                await accConfig.WriteValueAsync((Encoding.UTF8.GetBytes(Chunks)).AsBuffer());
            }
            Debug.WriteLine("H:Value_writed");
        }

        static IEnumerable<string> ChunksUpto(string str, int maxChunkSize)
        {
            // String partitioner function
            for (int i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
        }
        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            // This button is for sending the message in the textbox
            sendMessage(textBox1.Text);
        }
    }
}
