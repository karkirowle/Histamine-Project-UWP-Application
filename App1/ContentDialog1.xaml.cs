﻿using System;
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
using System.Runtime.Serialization.Json;
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
        public static readonly HttpClient client = new HttpClient(); // ezt kell public staticcá tenni és át kell menteni a mainpage egyik propertyjébe

        public ContentDialog1()
        {
            
            this.InitializeComponent();
            //if (failedLog) textBlock1.Text = "Password or username is wrong. Please try again.";
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
            string pass = await getPassword(textBox.Text);
            if (passwordBox.Password.Equals(pass))
            {
                loggedIn = true;
                loginring.IsActive = false;
                username = textBox.Text;
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
            //string uriString = "https://9875.k.time4vps.cloud:7473/browser/";
            string uriString2 = "http://localhost:7474/";
            client.BaseAddress = new Uri(uriString2);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            string authorisationString = "Basic ";
            var toTranslate = System.Text.Encoding.UTF8.GetBytes("neo4j:admin");
            authorisationString += System.Convert.ToBase64String(toTranslate);
            client.DefaultRequestHeaders.Add("Authorization", authorisationString);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "user/neo4j");
            //request.Content = new StringContent("", System.Text.Encoding.UTF8, "application/json");
            var result = await client.SendAsync(request);
            Debug.WriteLine(result);
            // Assume it's succesful - TODO: think of a success measure based on what it returns (200 is success)
            authorised = true;
        }

        async private Task<string> getPassword(string user)
        {
           
            // Preparing a JSON query
            queryObject query1 = new queryObject();
            // Concatenation of string query to request typed in username
            string preQuery = "MATCH (a:doctor) WHERE a.username = \"";
            preQuery += user;
            preQuery += "\" RETURN a";
            //query1.query = "MATCH (a:doctor) WHERE a.username = \"admin\" RETURN a";
            query1.query = preQuery;
          // Writing out the JSON string for the authentication
            MemoryStream stream1 = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(queryObject));
            ser.WriteObject(stream1, query1);
            stream1.Position = 0;
            StreamReader sr = new StreamReader(stream1);
            string jsonString = sr.ReadToEnd();
            Debug.WriteLine(jsonString);
            // HTTP post request
            HttpRequestMessage request2 = new HttpRequestMessage(HttpMethod.Post, "db/data/cypher");
            request2.Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(request2);
            Stream receiveStream = await response.Content.ReadAsStreamAsync();
            StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);
            string output = readStream.ReadToEnd();
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
        }
    }
}
