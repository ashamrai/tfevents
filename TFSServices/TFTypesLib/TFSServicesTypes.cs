using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TFHelper
{
    public class TFSServicesTypes
    {
        [DataContract]
        public class Rule
        {
            [DataMember]
            public int id;
            [DataMember]
            public int rev;
            [DataMember]
            public int period;
            [DataMember]
            public int step;
        }
    }
}
