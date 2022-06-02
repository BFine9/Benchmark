using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmark.Model
{
    public class OrientResponse
    {
        //[JsonProperty("@ORID")]
        // public ORID ORID { get; set; }
        [JsonProperty("@OVersion")]
        public int OVersion { get; set; }
        [JsonProperty("@OType")]
        public int OType { get; set; }
        [JsonProperty("@OClassId")]
        public int OClassId { get; set; }
        public int documentReads { get; set; }
        public Current current { get; set; }
        public int documentAnalyzedCompatibleClass { get; set; }
        public int recordReads { get; set; }
        public int fetchingFromTargetElapsed { get; set; }
        public int evaluated { get; set; }
        public double elapsed { get; set; }
        public string resultType { get; set; }
        public int resultSize { get; set; }
    }
    
  /*  public class Orid
        {
            public int ClusterId { get; set; }
            public int ClusterPosition { get; set; }
            public string RID { get; set; }
        }*/

        public class Current
        {
            public int ClusterId { get; set; }
            public int ClusterPosition { get; set; }
            public string RID { get; set; }
        }

}

