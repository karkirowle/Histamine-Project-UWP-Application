using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Net.Http;
using Windows.Security.Cryptography.Certificates;
using System.Net.Http.Headers; // ez minek
using System.Net;
//using Windows.Web.Http;
using System.Runtime.Serialization.Json; // one of the JSON things might be redundant
using Windows.Data.Json;
using System.Threading.Tasks;
// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

    /* Log in class logic and view */

namespace App1
{
    public sealed partial class ContentDialog1 : ContentDialog
    {
        public bool loggedIn = false;
        public bool failedLog = false;
        private bool authorised = false;
        public string username = "";
        public string password = "";
        private static HttpClientHandler myClientHandler = new HttpClientHandler();
        public static readonly HttpClient client = new HttpClient(myClientHandler); // ezt kell public staticcá tenni és át kell menteni a mainpage egyik propertyjébe

        public ContentDialog1()
        {
            
            this.InitializeComponent();
            //if (failedLog) textBlock1.Text = "Password or username is wrong. Please try again.";
            //secureAuthoriseConnect();
            Debug.WriteLine("elindul ez is");
            authoriseConnect();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void textBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

    /*    private void button_Click(object sender, RoutedEventArgs e)
        {
            if ( textBox.Text.Equals("admin") && passwordBox.Password.Equals("admin"))
            {
                loggedIn = true;
                
            }
            this.Hide();
        } */

        private void textBlock1_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private async void button_Click_1(object sender, RoutedEventArgs e)
        {
            loginring.IsActive = true;
            textBlock1.Text = "";
            if (!authorised) authoriseConnect(); // if didn't get authorisation on startup handle it here
            //string pass = await getPassword(textBox.Text);
            string[] loginfo = await saltHashPrivate(textBox.Text);
            // TODO: get all relevant info
            // TODO: verify user in IF using keyGenerator
            Debug.WriteLine(loginfo[0]);
            Debug.WriteLine(loginfo[1]);
            if (keyGenerator.verifyUser(textBox.Text, passwordBox.Password, loginfo[1], loginfo[0]))
            {
                loggedIn = true;
                loginring.IsActive = false;
                username = textBox.Text;
                password = passwordBox.Password;
                this.Hide();
            }
            else
            {
                textBlock1.Text = "Wrong credentials. Please try again.";
                loginring.IsActive = false;
            }
            
            
        }
        
        async private void authoriseConnect()
        {
            // Does the authorisation request and creates a shared HttpClient class which is accessible from outside
            // TODO: More elaborate exception handling for cases like: not connected to db
            // Get authorisation first
            //DbContextOptionsBuilder
           // var apiKey = ConfigurationManager.AppSettings.Get("apiKey");
            string uriString = "https://9875.k.time4vps.cloud:7473/";
            client.BaseAddress = new Uri(uriString);
           myClientHandler.ClientCertificateOptions = ClientCertificateOption.Automatic;
           // client = new HttpClient(myClientHandler);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            string authorisationString = "Basic ";
            // TODO: Basically anyone can enter this with these credentials, try to come up for something with that
            var toTranslate = System.Text.Encoding.UTF8.GetBytes("neo4j:admin");
            authorisationString += System.Convert.ToBase64String(toTranslate);
            client.DefaultRequestHeaders.Add("Authorization", authorisationString);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "user/neo4j");
            var result = await client.SendAsync(request);
            //var result = await client.SendRequestAsync(request);
            Debug.WriteLine(result);
            // Assume it's succesful - TODO: think of a success measure based on what it returns (200 is success)
            authorised = true;
        }

        private async Task<string[]> saltHashPrivate(string user)
        {
            String s = String.Format("MATCH(a:doctor) WHERE a.username = \"{0}\" RETURN a", user);
            queryObject query1 = new queryObject(s);
            string[] results = new string[10];
            if (authorised)
            {
                string output = await query1.cypherPOST(client);
                query1.parseFromData(output, new string[] { "salt", "password", "privateKey" }, out results);
                Debug.WriteLine(output);
                return results;
               
            }
            return results;
        }
        /*
        async private Task<string> getPassword(string user)
        {
            // Concatenation of string query to request typed in username
            string preQuery = "MATCH (a:doctor) WHERE a.username = \"";
            preQuery += user;
            preQuery += "\" RETURN a";
            // Preparing a JSON query
            queryObject query1 = new queryObject(preQuery);
            // HTTP post request
            string output = await query1.cypherPOST(client);
            Debug.WriteLine(output);
            // Deserialization of data
            JsonObject value = JsonObject.Parse(output);
            IJsonValue j, k, l, m;
            value.TryGetValue("data", out j);
            JsonArray value2 = j.GetArray();
            // In case the search does not return any results, this array will be empty
            if (value2.Count == 0) return "s";
            // Otherwise go on and parse it
            JsonArray value3 = value2[0].GetArray();
            JsonObject object2 = value3[0].GetObject();
            object2.TryGetValue("data", out k);
            JsonObject object3 = k.GetObject();
            object3.TryGetValue("password", out l);
            object3.TryGetValue("username", out m);
            string password = l.GetString();
            string username = l.GetString();
            Debug.WriteLine(password);
            Debug.WriteLine(username);
            return password;
        } */
    }
}
