using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rajaongkir.city.Models;

namespace rajaongkir.cost.Models
{
    public class RajaOngkirCost
    {
        [JsonProperty("rajaongkir")]
        public CostValue CostValueList { get; set; }

    }

    public class CostValue
    {
        [JsonProperty("query")]
        public QueryModel Query { get; set; }

        [JsonProperty("status")]
        public StatusModel Status { get; set; }

        [JsonProperty("origin_details")]
        public CityModel OriginDetails { get; set; }

        [JsonProperty("destination_details")]
        public CityModel DestinationDetails { get; set; }

        [JsonProperty("results")]
        public List<ResultCostModel> Results { get; set; }
    }
    public class QueryModel
    {
        [JsonProperty("origin")]
        public string Origin { get; set; }

        [JsonProperty("destination")]
        public string Destination { get; set; }

        [JsonProperty("weight")]
        public string Weight { get; set; }
        [JsonProperty("courier")]
        public string Courier { get; set; }
    }

    public class ResultCostModel
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("costs")]
        public List<Costs> Costs { get; set; }
    }
    public class Costs
    {
        [JsonProperty("service")]
        public string Service { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("cost")]
        public List<Cost> Cost { get; set; }

    }
    public class Cost
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("etd")]
        public string Etd { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

    }
}
