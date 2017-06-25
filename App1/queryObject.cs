using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json; // one of the JSON things might be redundant
using Windows.Data.Json;
using System.IO;
using System.Net.Http;
using System.Diagnostics;



namespace App1
{
[DataContract]
    class queryObject
    {
       
        [DataMember(Name = "query")]
        public string query = "abc";

        [DataMember(Name = "params")]
        public Object parameters = new Object();

        public string jsonstring = "";
  

        public queryObject()
        {
        }
       public queryObject(string queryT)
        {
            // The constructor handles the JSON serialisation of the Cypher query
            this.query = queryT;
            MemoryStream stream1 = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(queryObject));
            ser.WriteObject(stream1, this);
            stream1.Position = 0;
            StreamReader sr = new StreamReader(stream1);
            string jsonString = sr.ReadToEnd();
            this.jsonstring = jsonString;
        }

       public async Task<string> cypherPOST(HttpClient client)
        {
            HttpRequestMessage request2 = new HttpRequestMessage(HttpMethod.Post, "db/data/cypher");
            request2.Content = new StringContent(this.jsonstring, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(request2);
            Stream receiveStream = await response.Content.ReadAsStreamAsync();
            StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);
            string output = readStream.ReadToEnd();
            return output;
        }
        public void parseFromData(string output, string[] fields, out string[] results)
        {
            bool allData = false;
            parseFromData(output, fields, out results, allData);
        }
        public void parseFromData(string output, string[] fields, out string[] results, bool allData)
        {
            results = new string[fields.Length];
            JsonObject value = JsonObject.Parse(output);
            IJsonValue j, k;
            int i = 0;
            value.TryGetValue("data", out j);
            JsonArray value2 = j.GetArray();
            // In case the search does not return any results, this array will be empty""
            if (value2.Count == 0) results[0] = "empty";
            else
            {
            
                if (!allData)
                {
                    // Otherwise go on and parse it
                    JsonObject object2 = value2[0].GetArray()[0].GetObject();
                    object2.TryGetValue("data", out k);

                    // Go through each string fields specified in the array
                    JsonObject object3 = k.GetObject();
                    foreach (string field in fields)
                    {
                        IJsonValue l;
                        object3.TryGetValue(field, out l);
                        results[i++] = l.GetString();
                    }
                }
                else
                {
                    value2 = value2[0].GetArray();
                    results = new string[value2.Count()];
                    for (int m = 0; m < value2.Count(); m++)
                    {
                        // TODO: Type-checking needed here
                        results[m] = Convert.ToString(value2[m].GetNumber());
                    } 
                }
            }
        }
    }
}
