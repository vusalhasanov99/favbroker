using CybergAgency.Data;
using CybergAgency.Data.Models;
using Dapper;
using MaxMind.GeoIP2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Net;
using System.Text.Json;

namespace CybergAgency.Services
{
    public class HelperService : IHelperService
    {
        private const string IpStackApiKey = "b10c26b12d706ffac2dd71c351e996c20e28e3566cb248a0ae668b7f";
        private const string IpStackBaseUrl = "https://api.ipdata.co/";
        private const string IpStackOwnService = "http://3.85.129.75/geolookup/";
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;




        public HelperService(AppDBContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<CountryInfo> CheckIpAddressForWebsiteAsync(string ipAddress, string webSiteCountryCode)
        {
            CountryInfo countryInfo = new CountryInfo();
            DateTime today = DateTime.UtcNow.AddHours(3);

            var query = "SELECT COUNT(*) FROM Logs WHERE Ip = @IpAddress AND CONVERT(date, CreateDate) = @TodayDate";
            var parameters = new { IpAddress = ipAddress, TodayDate = today.Date };
            using IDbConnection db = new SqlConnection("Data Source=cybergagencysql.cyu5xtobvdgy.us-east-1.rds.amazonaws.com;Initial Catalog=CybergAgencyDB;User ID=admin;Password=cybergagency2023");
            var ipCount = await db.QuerySingleAsync<int>(query, parameters);

            try
            {
                var embedServiceStatus = _context.StaticDatas.Where(s => s.Key == "OwnEmbed").FirstOrDefault();
                var ipDataStatus = _context.StaticDatas.Where(s => s.Key == "IpData").FirstOrDefault();
                var ownApiStatus = _context.StaticDatas.Where(s => s.Key == "OwnApi").FirstOrDefault();

                if (embedServiceStatus.Value.Equals("true"))
                {
                    countryInfo = await IpEmbedService(ipAddress, webSiteCountryCode);
                }

                if (countryInfo.CountryCode is "Anonym" && ipDataStatus.Value.Equals("true"))
                {
                    countryInfo = await IpDataService(ipAddress, webSiteCountryCode);
                }

                if (countryInfo.CountryCode is "Anonym" && ownApiStatus.Value.Equals("true"))
                {
                    countryInfo = await IpOwnService(ipAddress, webSiteCountryCode);
                }

                return countryInfo;

            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return countryInfo;
            }
        }


        public async Task<GClid> CheckGoogleClickIdAsync(string gClid, WebSite webSite)
        {
            var gClidEntity = await _context.GClids.Where(gc => gc.GoogleClickID == gClid).FirstOrDefaultAsync();

            if (gClidEntity is null)
            {
                gClidEntity = new GClid()
                {
                    WebSite = webSite,
                    GoogleClickID = gClid,
                    MarketSubcatagory = webSite.MarketSubcatagory
                };

                await _context.GClids.AddAsync(gClidEntity);
                await _context.SaveChangesAsync();
            }
            return gClidEntity;
        }

        public bool CheckUserAgent(string userAgent)
        {
            List<string> userAgents = new List<string>
            {
                "Googlebot",
                "Baiduspider",
                "ia_archiver",
                "R6_FeedFetcher",
                "NetcraftSurveyAgent",
                "Sogou web spider",
                "bingbot",
                "Yahoo! Slurp",
                "facebookexternalhit",
                "PrintfulBot",
                "msnbot",
                "Twitterbot",
                "UnwindFetchor",
                "urlresolver"
            };

            return userAgents.Any(agent => userAgent.IndexOf(agent, StringComparison.OrdinalIgnoreCase) >= 0);
        }


        private async Task<CountryInfo> IpDataService(string ipAddress, string webSiteCountryCode)
        {
            CountryInfo countryInfo = new CountryInfo();
            string apiUrl = $"{IpStackBaseUrl}{ipAddress}?api-key={IpStackApiKey}";

            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                try
                {
                    Root root = JsonSerializer.Deserialize<Root>(responseBody);
                    countryInfo.CountryName = root.country_name + " | IpData";
                    countryInfo.CountryCode = root.country_code;
                    countryInfo.Status = countryInfo.CountryCode.Trim().ToLower() == webSiteCountryCode.Trim().ToLower();

                    return countryInfo;
                }
                catch (Exception jsonException)
                {
                    await Console.Out.WriteLineAsync(jsonException.Message);
                }
            }

            return countryInfo;
        }

        private async Task<CountryInfo> IpOwnService(string ipAddress, string webSiteCountryCode)
        {
            CountryInfo countryInfo = new CountryInfo();


            var apiUrlForOwnService = $"{IpStackOwnService}{ipAddress}";
            HttpResponseMessage responseForOwnService = await _httpClient.GetAsync(apiUrlForOwnService);
            if (responseForOwnService.IsSuccessStatusCode)
            {
                string jsonString = await responseForOwnService.Content.ReadAsStringAsync();
                JArray jsonArray = JArray.Parse(jsonString);

                try
                {
                    // Attempt to deserialize JSON response
                    countryInfo.CountryName = jsonArray[0]["country_info"]["Country"].ToString() + " | Own";
                    countryInfo.CountryCode = jsonArray[0]["country_info"]["Alpha-2 code"].ToString();
                    countryInfo.Status = countryInfo.CountryCode.Trim().ToLower() == webSiteCountryCode.Trim().ToLower();
                }
                catch (Exception jsonException)
                {
                    await Console.Out.WriteLineAsync(jsonException.Message);
                }
            }

            return countryInfo;
        }


        private async Task<CountryInfo> IpEmbedService(string ipAddress, string webSiteCountryCode)
        {
            CountryInfo countryInfo = new CountryInfo();


            try
            {
                using (var reader = new DatabaseReader(GetFile("Country.mmdb")))
                {
                    var response = reader.Country(ipAddress);

                    countryInfo.CountryName = response.Country.Name + " | Embed";
                    countryInfo.CountryCode = response.Country.IsoCode;
                    countryInfo.Status = countryInfo.CountryCode.Trim().ToLower() == webSiteCountryCode.Trim().ToLower();
                }
            }
            catch (Exception ex)
            {

                await Console.Out.WriteLineAsync(ex.Message);
            }

            return countryInfo;
        }

        private string GetFile(string fileName)
        {
            var webRootPath = _webHostEnvironment.WebRootPath;
            var filePath = Path.Combine(webRootPath, fileName);

            if (System.IO.File.Exists(filePath))
            {
                // Do something with the file path, e.g., return it in the response
                return filePath;
            }
            else
            {
                return string.Empty;
            }
        }


    }
}