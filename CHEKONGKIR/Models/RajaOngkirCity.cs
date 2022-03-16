using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rajaongkir.city.Models
{
    public class RajaOngkirCity
    {
        [JsonProperty("rajaongkir")]
        public CityValue CityList { get; set; }
    }
    public class CityValue
    {
        [JsonProperty("query")]
        public List<QueryModel> Query { get; set; }

        [JsonProperty("status")]
        public StatusModel Status { get; set; }

        [JsonProperty("results")]
        public List<CityModel> Result { get; set; }
    }

    public class QueryModel
    {
        [JsonProperty("province")]
        public string Province { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class StatusModel
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public class CityModel
    {
        [JsonProperty("city_id")]
        public string CityId            { get; set; }

        [JsonProperty("province_id")]
        public string ProvinceId        { get; set; }

        [JsonProperty("province")]
        public string Province          { get; set; }

        [JsonProperty("type")]
        public string Type              { get; set; }

        [JsonProperty("city_name")]
        public string CityName          { get; set; }

        [JsonProperty("postal_code")]
        public string PostalCode        { get; set; }
    }
}
