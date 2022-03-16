using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rajaongkir.province.Models
{

    public class RajaOngkirProvince
    {
        [JsonProperty("rajaongkir")]
        public ProvinceValueList ProvinceList { get; set; }
    }

    public class ProvinceValueList
    {
        [JsonProperty("query")]
        public List<QueryModel> Query       { get; set; }

        [JsonProperty("status")]
        public StatusModel Status           { get; set; }

        [JsonProperty("results")]
        public List<ProvinceModel> Result   { get; set; }
    }

    public class QueryModel
    {
        [JsonProperty("id")]
        public string Id            { get; set; }
    }

    public class StatusModel
    {
        [JsonProperty("code")]
        public string Code          { get; set; }

        [JsonProperty("description")]
        public string Description   { get; set; }
    }

    public class ProvinceModel
    {
        [JsonProperty("province_id")]
        public string ProvinceId    { get; set; }

        [JsonProperty("province")]
        public string Province      { get; set; }
    }
}
