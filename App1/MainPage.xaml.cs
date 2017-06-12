using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.Media;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using Windows.Data.Json;
using Windows.Security.Cryptography;
using System.Globalization;


namespace App1
{
    /// <summary>
    /// Histamine Control Panel version 0.8 Alpha
    /// 
    /// 
    /// NEW 0.8 - Patient measurement plot implemented, adding measurements implemented
    /// 0.7.1 - Login functionality extended with patient selector screen. Added capability to access data from webserver via SSL.
    /// 0.6 - Extended login functionality implemented with Neo4j, more functionalities to come
    /// 0.5 - Basic login functionality implemented, interface created for different voltammetries, though the latter is not working fully.
    /// 0.4 - Two-way Bluetooth communication interface with a TI MSP430 via an RFDuino. Able to receive a plot shorter time series
    /// data, sending is sometimes unstable.
    /// 0.3 - Contains the OxyPlot module to visualise voltammetry. 
    /// 
    /// Bence Halpern
    /// 2017.05.30.
    /// </summary>
    /// 

    public sealed partial class MainPage : Page
    {

        // Properties related to Bluetooth connection
        public List<string> deviceList = new List<string>();
        GattCharacteristic accData;
        GattCharacteristic accConfig;
        bool firstUpdate = true;
        double[] x_v = { -0.4, -0.38994, -0.37987, -0.36981, -0.35975, -0.34969, -0.33962, -0.32956, -0.3195, -0.30943, -0.29937, -0.28931, -0.27925, -0.26918, -0.25912, -0.24906, -0.23899, -0.22893, -0.21887, -0.20881, -0.19874, -0.18868, -0.17862, -0.16855, -0.15849, -0.14843, -0.13836, -0.1283, -0.11824, -0.10818, -0.098113, -0.08805, -0.077987, -0.067925, -0.057862, -0.047799, -0.037736, -0.027673, -0.01761, -0.0075472, 0.0025157, 0.012579, 0.022642, 0.032704, 0.042767, 0.05283, 0.062893, 0.072956, 0.083019, 0.093082, 0.10314, 0.11321, 0.12327, 0.13333, 0.1434, 0.15346, 0.16352, 0.17358, 0.18365, 0.19371, 0.20377, 0.21384, 0.2239, 0.23396, 0.24403, 0.25409, 0.26415, 0.27421, 0.28428, 0.29434, 0.3044, 0.31447, 0.32453, 0.33459, 0.34465, 0.35472, 0.36478, 0.37484, 0.38491, 0.39497, 0.39497, 0.38491, 0.37484, 0.36478, 0.35472, 0.34465, 0.33459, 0.32453, 0.31447, 0.3044, 0.29434, 0.28428, 0.27421, 0.26415, 0.25409, 0.24403, 0.23396, 0.2239, 0.21384, 0.20377, 0.19371, 0.18365, 0.17358, 0.16352, 0.15346, 0.1434, 0.13333, 0.12327, 0.11321, 0.10314, 0.093082, 0.083019, 0.072956, 0.062893, 0.05283, 0.042767, 0.032704, 0.022642, 0.012579, 0.0025157, -0.0075472, -0.01761, -0.027673, -0.037736, -0.047799, -0.057862, -0.067925, -0.077987, -0.08805, -0.098113, -0.10818, -0.11824, -0.1283, -0.13836, -0.14843, -0.15849, -0.16855, -0.17862, -0.18868, -0.19874, -0.20881, -0.21887, -0.22893, -0.23899, -0.24906, -0.25912, -0.26918, -0.27925, -0.28931, -0.29937, -0.30943, -0.3195, -0.32956, -0.33962, -0.34969, -0.35975, -0.36981, -0.37987, -0.38994, -0.4 };
        double[] y_v = { 0, -1.9614e-06, -2.6117e-06, -3.754e-06, -5.4536e-06, -8.0848e-06, -1.1938e-05, -1.7816e-05, -2.6083e-05, -3.8579e-05, -5.7197e-05, -8.4343e-05, -0.00012531, -0.00018488, -0.00027339, -0.00040375, -0.00059661, -0.00088658, -0.0013121, -0.0019412, -0.0028707, -0.0042436, -0.0063095, -0.0093189, -0.013744, -0.020292, -0.029929, -0.044308, -0.064985, -0.095589, -0.14016, -0.20409, -0.29582, -0.42305, -0.59942, -0.8317, -1.112, -1.4613, -1.8329, -2.1866, -2.4811, -2.6732, -2.7582, -2.7386, -2.648, -2.5156, -2.3637, -2.2117, -2.068, -1.9387, -1.8233, -1.7222, -1.634, -1.5569, -1.4894, -1.4299, -1.3772, -1.3303, -1.2881, -1.25, -1.2154, -1.1837, -1.1546, -1.1277, -1.1027, -1.0794, -1.0577, -1.0373, -1.0181, -0.99997, -0.98284, -0.9666, -0.95117, -0.93648, -0.92249, -0.90912, -0.89633, -0.88409, -0.87235, -0.86107, -0.85023, -0.83981, -0.82976, -0.82007, -0.81073, -0.8017, -0.79297, -0.78452, -0.77635, -0.76842, -0.76074, -0.75328, -0.74604, -0.73899, -0.73214, -0.72547, -0.71896, -0.71259, -0.70636, -0.70023, -0.69418, -0.68816, -0.68212, -0.67597, -0.66961, -0.66287, -0.6555, -0.64718, -0.63741, -0.62552, -0.61057, -0.59123, -0.56582, -0.53227, -0.48847, -0.43269, -0.36356, -0.28365, -0.19744, -0.1118, -0.035265, 0.024542, 0.065369, 0.089052, 0.09746, 0.096255, 0.089028, 0.078947, 0.068194, 0.05752, 0.047926, 0.039494, 0.032328, 0.026298, 0.021323, 0.01721, 0.013859, 0.011119, 0.0088865, 0.0070705, 0.0055894, 0.0043815, 0.0033935, 0.002585, 0.0019211, 0.0013756, 0.00092616, 0.00055545, 0.00024917, -4.0847e-06, -0.00021347, -0.00038661, -0.00052967, -0.0006477, -0.00074484, -0.00082449, -0.0008895, -0.00094219, -0.00098451, -0.0010181};
        // Properties related to users
        string userBeingTreated = "";
        string doctor = "";
        HttpClient sharedClient;
        // Properties related to plotting
        public PlotModel MyModel { get; private set; }
        public PlotModel MeasurementModel { get; private set; }

        public MainPage()
        {
            this.InitializeComponent();
            MyModel = new PlotModel { Title = "Loading voltamogramm..." };
            MeasurementModel = new PlotModel { Title = "Loading patient data" };
            TryCrypto();
            LoginLoad();
            Debug.WriteLine(MyPlotView.Background);
            MyPlotView.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(this.BaseUri, "Assets/SplashScreen.scale-200.png"))};
            MyPlotView.Model = MyModel;
            MeasurementPlot.Model = MeasurementModel;
            Debug.WriteLine("A");
            LoadedState();
            Debug.WriteLine("B");
        }
        public void TryCrypto()
        {
            securityAlgorithms sec = new securityAlgorithms();
           // Debug.WriteLine(sec.keyGet);
        }
        private async void LoginLoad()
        {
            // Login screen
            ContentDialog1 signInDialog = new ContentDialog1();
            await signInDialog.ShowAsync();
            // Because it is statically allocated all the instances of content dialog share it, thus it only makes sense to access the property from the type itself
            var client = ContentDialog1.client;
            sharedClient = client;
            // Patient list request
            //Debug.WriteLine(signInDialog.username);
            string preQuery = String.Format("MATCH (a:doctor)-[: treat]->(b:patient) WHERE a.username = \"{0}\"  RETURN b", signInDialog.username);
            // Preparing a JSON query
            queryObject query1 = new queryObject(preQuery);
            string output = await query1.cypherPOST(client);
            // Deserialization of data
            JsonObject value = JsonObject.Parse(output);
            IJsonValue j, k, m;
            value.TryGetValue("data", out j);
            JsonArray value2 = j.GetArray();
            // In case the search does not return any results, this array will be empty
            List<string> patientList = new List<string> { }; // Meaning it will be empty if zero found
            if (value2.Count != 0) // If there are patients
            {
                for (int i=0; i < value2.Count; i++)
                {
                    JsonArray value3 = value2[i].GetArray();
                    JsonObject object2 = value3[0].GetObject();
                    object2.TryGetValue("data", out k);
                    JsonObject object3 = k.GetObject();
                    object3.TryGetValue("username", out m);
                    string patient = m.GetString();
                    patientList.Add(patient);
                }
            }
            ContentDialog2 patientDialog = new ContentDialog2(signInDialog.username);
            patientDialog.patientList = patientList; // You could make patientlist private if you do an initaliser for it
            await patientDialog.ShowAsync();
            textBlock2_Copy.Text = String.Format("You are currently treating {0}.", patientDialog.patientToTreat);
            this.userBeingTreated = patientDialog.patientToTreat;
            this.doctor = signInDialog.username;
            // When everything is done, load the patient measurement data into the plot
            List<MeasurementPoint> mes = await fetchMeasurements();
            if (mes.Count != 0) updatePatientPlot(mes);
        }
        private async void LoadedState()
        {
           var taskDevices = await getDevices();
           updatePlot(x_v, y_v);
           List<MeasurementPoint> mes = await fetchMeasurements();
           if(mes.Count != 0) updatePatientPlot(mes);
        }

        public async Task<List<string>> getDevices()
        {
            Debug.WriteLine("C");
            foreach (DeviceInformation di in await DeviceInformation.FindAllAsync(GattDeviceService.GetDeviceSelectorFromUuid(new Guid("00002220-0000-1000-8000-00805f9b34fb"))))
            {
                Debug.WriteLine("D");
                Debug.WriteLine(di.Name);
                GattDeviceService bleService = await GattDeviceService.FromIdAsync(di.Id);
                Debug.WriteLine(bleService.Device.ConnectionStatus.ToString());
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

        async void updatePlot(double[] x_values, double[] y_values)
        {
            // TODO: generalise it, so this along with updatePatientPlot() is more abstract
            MyModel = new PlotModel { Title = "Cyclic Voltammetry" };
            var LineSeries = new LineSeries { };
            for (int i = 0; i < x_values.Length; i++)
            {
                LineSeries.Points.Add(new DataPoint(x_values[i], y_values[i]));
            }
            MyModel.Series.Add(LineSeries);
            // Dispatcher call to reach UI thread
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                MyPlotView.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                MyPlotView.Model = MyModel;
         });

        }
        async void updatePatientPlot(List<MeasurementPoint> mesList)
        {
            // TODO: generalise it, so this along with updatePatientPlot() is more abstract
            MeasurementModel = new PlotModel { Title = "Patient histamine data" };
            //var LineSeries = new LineSeries { };
            var StemSeries = new StemSeries
            {
                MarkerStroke = OxyColors.Red,
                MarkerType = MarkerType.Circle
            };
            DateTime startDate = mesList.First().date;
            DateTime endDate = mesList.Last().date;
            var minValue = DateTimeAxis.ToDouble(startDate);
            //Debug.WriteLine(minValue); Debug.WriteLine(maxValue);
            var maxValue = DateTimeAxis.ToDouble(endDate);
            MeasurementModel.Axes.Add(new DateTimeAxis{ Position = AxisPosition.Bottom, Minimum = minValue, Maximum = maxValue, StringFormat = "dd/MM/yyyy \n HH:mm:ss"});
            for (int i = 0; i < mesList.Count(); i++)
            {
                DateTime i2date = mesList.ElementAt(i).date;
                StemSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(i2date), mesList.ElementAt(i).value));
            }
            MeasurementModel.Series.Add(StemSeries);
            // Dispatcher call to reach UI thread
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                MeasurementPlot.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                MeasurementPlot.Model = MeasurementModel;
            });

        }

        async Task<List<MeasurementPoint>> fetchMeasurements()
        {
            string preQuery = String.Format("MATCH(a:patient) -[ :belongs]->(b:measurement) WHERE a.username = \"{0}\" RETURN b", userBeingTreated);
            queryObject uploadQuery = new queryObject(preQuery);
            string output = await uploadQuery.cypherPOST(sharedClient);
            Debug.WriteLine(output);
            // Parsing
            JsonObject value = JsonObject.Parse(output);
            IJsonValue j, k, m,l;
            value.TryGetValue("data", out j);
            JsonArray value2 = j.GetArray();
            // In case the search does not return any results, this array will be empty
            List<MeasurementPoint> measurementList = new List<MeasurementPoint> { };
            if (value2.Count != 0) // Given there are results
            {
                for (int i = 0; i < value2.Count; i++)
                {
                    JsonArray value3 = value2[i].GetArray();
                    JsonObject object2 = value3[0].GetObject();
                    object2.TryGetValue("data", out k);
                    JsonObject object3 = k.GetObject();
                    object3.TryGetValue("date", out m);
                    string measurementDate = m.GetString();
                    string fmt = "dd/MM/yyyy HH:mm:ss"; // Format of the date
                    DateTime mesDateTime = DateTime.ParseExact(measurementDate, fmt, null);
                    object3.TryGetValue("value", out l);
                    string measurementValue = l.GetString();
                    MeasurementPoint currentMeasurement = new MeasurementPoint { date = mesDateTime, value = Convert.ToDouble(measurementValue) };
                    measurementList.Add(currentMeasurement);
                }
            }
            // Sort
            measurementList.OrderBy(mes => mes.date);
            return measurementList;
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
                messageInterpreter(newData);
            });
        }

        void messageInterpreter(string newData)
        {
            if (newData[0].Equals('v'))
            {
                // Voltammetry data incoming
                string[] dataSplit = newData.Split();
              
              
                Debug.WriteLine(dataSplit.Length);
                int startX = 0;
                int endX = 0;
                int startY = 0;
                int endY = dataSplit.Length - 2; // last is new line
                // Identify start and end
                for (int i = 0; i < dataSplit.Length;  i++)
                {
                    Debug.WriteLine(dataSplit[i]);
                    if (dataSplit[i].Equals("x"))
                    {
                        startX = i + 1;
                        //Debug.WriteLine("updatedX");
                    }
                    if (dataSplit[i].Equals("y"))
                    {
                        startY = i + 1;
                        endX = i - 1;
                    }
                    
                }
                // Create array for the plot
                double[] x_plot = new double[endX-startX+1];  
                for (int i = startX; i <= endX; i++)
                {
                    x_plot[i - startX] = Convert.ToDouble(dataSplit[i]);
                }
                double[] y_plot = new double[endY - startY + 1];
                for (int i = startY; i <= endY; i++)
                {
                    Debug.WriteLine(dataSplit[i]);
                    y_plot[i - startY] = Convert.ToDouble(dataSplit[i]);
                }
                // UpdatePlot
                updatePlot(x_plot, y_plot);
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

        private void textBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            // Button programmed to initiate cv measurement
            sendMessage("cv");
        }

        private void textBlock1_Copy2_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
        private void textBlock1_Copy_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void textBlock2_Copy_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void textBlock3_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private async void button4_Click(object sender, RoutedEventArgs e)
        {
            // Upload voltammetry data which is in textbox
            Debug.WriteLine(textBox4.Text);
            // textBlock2_Copy.Text = String.Format("You are currently treating {0}.", patientDialog.patientToTreat)
            // Commented out for not uploading
            DateTime localDate = DateTime.Now;
            string dateNow = localDate.ToString(new CultureInfo("en-GB")); // display date in DATE-TIME Format
            Debug.WriteLine(dateNow);
                
                string preQuery = "MATCH (b:patient) WHERE b.username = \"";
                preQuery += userBeingTreated; // replace with username
                preQuery += "\" CREATE(b)-[: belongs]->(a: measurement{ type:\"histamine\", value: \"";
                preQuery += textBox4.Text;
            preQuery += "\", date: \"";
            preQuery += dateNow;
            preQuery += "\"";
                preQuery += "}) RETURN b";
                queryObject uploadQuery = new queryObject(preQuery);
                
                string output = await uploadQuery.cypherPOST(sharedClient); 
            }

        class MeasurementPoint
        {
            public DateTime date { get; set; }
            public double value { get; set; }
        }
    }
}
