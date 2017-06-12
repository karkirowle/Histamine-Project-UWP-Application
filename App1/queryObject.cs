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
    }
}
