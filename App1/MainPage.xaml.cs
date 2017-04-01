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
    /// Histamine Control Panel version 0.1 ALPHA
    /// Program on startup retrieves necessary objectst to send a message to the histamine sensing apparatus.
    /// Bence Halpern
    /// 2017.04.01.
    /// </summary>
    /// 
   
    public sealed partial class MainPage : Page
    {

        public List<string> deviceList = new List<string>();
        public MainPage()
        {
            this.InitializeComponent();
            Debug.WriteLine("A");
            LoadedState();
            Debug.WriteLine("B");

        }
        private async void LoadedState()
        {
            var taskDevices = await getDevices();
        }
        public async Task<List<string>> getDevices()
        {
            Debug.WriteLine("C");
            foreach (DeviceInformation di in await DeviceInformation.FindAllAsync(GattDeviceService.GetDeviceSelectorFromUuid(new Guid("0000ffe0-0000-1000-8000-00805f9b34fb"))))
            {
                Debug.WriteLine("D");
                Debug.WriteLine(di.Name);
                Debug.WriteLine(di.Id);
                Debug.WriteLine(di.IsEnabled);
                GattDeviceService bleService = await GattDeviceService.FromIdAsync(di.Id);
                
                Debug.WriteLine("E");
                Debug.WriteLine(GattCharacteristic.ConvertShortIdToUuid(0xFFE1).ToString());
                var accConfig = bleService.GetCharacteristics(GattCharacteristic.ConvertShortIdToUuid(0xFFE1))[0];
                Debug.WriteLine("F");
                // The message to be sent
                string message = "Control Panel connected.";
                await accConfig.WriteValueAsync((Encoding.UTF8.GetBytes(message)).AsBuffer());
                Debug.WriteLine("G");
            }
            return deviceList;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
        }
        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
