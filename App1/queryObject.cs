using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace App1
{
[DataContract]
    class queryObject
    {
       
        [DataMember(Name = "query")]
        public string query = "abc";

        [DataMember(Name = "params")]
        public Object parameters = new Object();

    }
}
