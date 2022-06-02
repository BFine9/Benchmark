using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmark.Model
{
    public class CityModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("city_ascii")]
        public string CityAscii { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("population")]
        public string Population { get; set; }
        [JsonProperty("lat")]
        public double Lat { get; set; }
        [JsonProperty("lng")]
        public double Lng { get; set; }

    }
    public class RootCityJson
    {
        public List<CityModel> Cities { get; set; }
    }
    public class CityArrango : CityModel
    {
        [JsonProperty("_key")]
        public string _key { get; set; }
        [JsonProperty("_id")]
        public string _id { get; set; }
        [JsonProperty("_rev")]
        public string _rev { get; set; }
    }
   
    public class CityDgraph: CityModel
    {
        public int Distance;
        public CityModel ToCity = new CityModel();
    }

    public class CityDgraphDistance : CityModel
    {
        public int Distance { get; set; }
    }

    public class Edge
    {
        public string _key { get; set; }
        public string _id { get; set; }
        public string _from { get; set; }
        public string _to { get; set; }
        public string _rev { get; set; }
        public int dystans { get; set; }
    }

    public class Vertex
    {
        public string _key { get; set; }
        public string _id { get; set; }
        public string _rev { get; set; }
        public int id { get; set; }
        public string city_ascii { get; set; }
        public string country { get; set; }
        public string population { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class MyArray
    {
        public List<Edge> edges { get; set; }
        public List<Vertex> vertices { get; set; }
    }

    public class CityArangoList
    {
        [JsonProperty("MyArray")]
        public List<MyArray> Cities { get; set; }
    }


}