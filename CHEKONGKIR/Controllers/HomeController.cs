using CHEKONGKIR.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using rajaongkir.province.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using rajaongkir.city.Models;
using rajaongkir.cost.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;
using selecttwo.Models;

namespace cekongkir.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly string myKeys = "fbdec8900f53cab26f7acec9ec89fbe3";

        private readonly IMemoryCache _memoryCache;
        public HomeController(ILogger<HomeController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }


        [AllowAnonymous,HttpGet("/")]
        public async Task<IActionResult> Index()
        {
            try
            {   
                //run method GetAllCity for get cache city
                List<CityModel> cities = await GetAllCity();
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
            }
           
            return View();
        }

        #region province
        public static List<SelectListItem> CreateSelectBoxProvince(List<ProvinceModel> provinces)
        {
            List<SelectListItem> selectLists = new();
            foreach(ProvinceModel pm in provinces)
            {
                SelectListItem selectList = new();
                selectList.Text = pm.Province;
                selectList.Value = pm.ProvinceId;

                selectLists.Add(selectList);
            }

            return selectLists;
        }
        //method ini tidak digunakan, namun bisa di pakai untuk mencari data provinsi
        public async Task<List<ProvinceModel>> GetAllProvince()
        {
            var cacheProvince = "provincelist";

            //checks if cache entries exists
            if (!_memoryCache.TryGetValue(cacheProvince, out List<ProvinceModel> provinces))
            {
                //calling data from api raja ongkir
                var client = new RestClient("https://api.rajaongkir.com/starter/province");
                var request = new RestRequest("", Method.Get);
                request.AddHeader("key", myKeys);
                RestResponse response = await client.ExecuteAsync(request);

                if (response.ErrorMessage == null)
                {
                    //set data json to list
                    var getData = JsonConvert.DeserializeObject<RajaOngkirProvince>(response.Content).ProvinceList;
                    if (getData.Status.Code == "400")
                    {
                        throw new Exception(getData.Status.Description);
                    }

                    provinces = getData.Result;
                }
                else
                {
                    throw new Exception(response.ErrorMessage + ": Check your connection");
                }

                //setting up cache options
                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    //cache will expire absolute on the nex day
                    AbsoluteExpiration = DateTime.Now.AddDays(1),
                    Priority = CacheItemPriority.High,
                    //cache will expire if it is not used by anyone
                    //SlidingExpiration = TimeSpan.FromSeconds(20)
                };
                //setting cache entries
                _memoryCache.Set(cacheProvince, provinces, cacheExpiryOptions);
            }
          
            return provinces;
        }
        #endregion

        #region city
        public async Task<List<CityModel>> GetAllCity()
        {
            var cacheCity = "cityList";

            //checks if cache entries exists
            if (!_memoryCache.TryGetValue(cacheCity, out List<CityModel> citys))
            {
                //calling data from api raja ongkir
                var client = new RestClient("https://api.rajaongkir.com/starter/city");
                var request = new RestRequest("", Method.Get);
                request.AddHeader("key", myKeys);
                RestResponse response = await client.ExecuteAsync(request);

                if (response.ErrorMessage == null)
                {
                    //set data json to list
                    var getData = JsonConvert.DeserializeObject<RajaOngkirCity>(response.Content).CityList;
                    if (getData.Status.Code == "400")
                    {
                        throw new Exception(getData.Status.Description);
                    }

                    citys = getData.Result;
                }
                else
                {
                    throw new Exception(response.ErrorMessage + ": Check your connection");
                }

                //setting up cache options
                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    //cache will expire absolute on the nex day
                    AbsoluteExpiration = DateTime.Now.AddDays(1), 
                    Priority = CacheItemPriority.High,
                    //cache will expire if it is not used by anyone
                    //SlidingExpiration = TimeSpan.FromSeconds(20)
                };
                //setting cache entries
                _memoryCache.Set(cacheCity, citys, cacheExpiryOptions);
            }          
            
            return citys;
        }

        [HttpGet]
        public async Task<IActionResult> SearchCity(string search)
        {
            try
            {
                List<CityModel> citys = await GetAllCity();
                citys = citys.Where(a => a.CityName.Contains(search,StringComparison.CurrentCultureIgnoreCase)).ToList();

                List<SelectTwoModel> results = new();
                foreach(CityModel city in citys)
                {
                    SelectTwoModel res = new();
                    city.Type = city.Type.ToLower() == "kabupaten" ? "Kab " : city.Type + " ";

                    res.Id = city.CityId;                
                    res.Text = city.Type + city.CityName + "," + city.Province;

                    results.Add(res);
                }
                return Json(new { isuccess = true, items = results });
                
            }
            catch(Exception e)
            {
                return Json(new { isuccess = false, msgerror = e.Message});
            }
            
        }
        #endregion

        public string ValidationCost(string origin, string destination, string weight)
        {
            string msgError = null;

            if(string.IsNullOrEmpty(origin))
            {
                msgError = "Silahkan isi kota/kabupaten asal";
            }
            else if (string.IsNullOrEmpty(destination))
            {
                msgError = "Silahkan isi kota/kabupaten tujuan";
            }
            else if (string.IsNullOrEmpty(weight))
            {
                msgError = "Silahkan isi berat (kg)";
            }

            return msgError;
        }
        
        public async Task<IActionResult> SearchCost(string origin,string destination, string weight)
        {
            try
            {
                weight = (int.Parse(weight) * 1000).ToString();
                string[] courier = { "jne", "tiki", "pos" };

                string msgError = ValidationCost(origin, destination, weight);
                if (!string.IsNullOrEmpty(msgError))
                {
                    ViewBag.validErr = msgError;
                    return PartialView("~/Views/Home/_GridView.cshtml");
                }

                ViewBag.cityfrom = null;
                ViewBag.cityto = null;

                List<ResultCostModel> resultCosts = new();
                for (int i = 0; i < courier.Length; i++)
                {
                    var client = new RestClient("https://api.rajaongkir.com/starter/cost");
                    var request = new RestRequest("", Method.Post);
                    request.AddHeader("key", myKeys);
                    request.AddHeader("content-type", "application/x-www-form-urlencoded");
                    request.AddParameter("origin", origin);
                    request.AddParameter("destination", destination);
                    request.AddParameter("weight", weight);
                    request.AddParameter("courier", courier[i]);
                    RestResponse response = await client.ExecuteAsync(request);

                    if (response.ErrorMessage == null)
                    {
                        var data = JsonConvert.DeserializeObject<RajaOngkirCost>(response.Content).CostValueList;
                        if (data.Status.Code == "400")
                        {
                            throw new Exception(data.Status.Description);
                        }

                        ViewBag.cityfrom = data.OriginDetails.Type.ToLower().Replace("kabupaten","kab") +" "+ data.OriginDetails.CityName;
                        ViewBag.cityto = data.DestinationDetails.Type.ToLower().Replace("kabupaten", "kab") + " " + data.DestinationDetails.CityName; ;

                        if (data.Results.Count > 0)
                        {
                            resultCosts.AddRange(data.Results);
                        }
                    }
                    else
                    {
                        throw new Exception(response.ErrorMessage + ": Check your connection");
                    }                
                }

                
                ViewData["listdata"] = resultCosts;
                ViewBag.weight = (int.Parse(weight)/1000).ToString();              
            }
            catch(Exception e)
            {
                ViewBag.sysError = e.Message;
            }
            return PartialView("~/Views/Home/_GridView.cshtml");

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
