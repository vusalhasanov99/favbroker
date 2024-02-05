using CybergAgency.Data;
using CybergAgency.Data.Models;
using CybergAgency.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CybergAgency.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDBContext _context;
        private readonly IHelperService _helperService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(ILogger<HomeController> logger, AppDBContext context, IHelperService helperService, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _context = context;
            _helperService = helperService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }


        [Route("{country=uae}")]
        public async Task<IActionResult> Index(string secretKey, string gclid)
        {
            var pathWithQuery = _httpContextAccessor.HttpContext.Request.PathBase + _httpContextAccessor.HttpContext.Request.Path + _httpContextAccessor.HttpContext.Request.QueryString;

            


            ViewBag.Query = pathWithQuery.ToUpper();

            try
            {
                string remoteIPAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                string userAgent = Request.Headers["User-Agent"].ToString();
                var webSiteName = _configuration["WebSite"];

                var webSite = await _context.WebSites
                    .Where(w => w.Name.Trim().ToLower() == webSiteName.Trim().ToLower())
                    .Include(w => w.MarketSubcatagory)
                    .ThenInclude(msc => msc.Market)
                    .Include(w => w.MarketSubcatagory)
                    .ThenInclude(msc => msc.Brands.Where(b => b.IsActive).OrderBy(b => b.Place))
                    .ThenInclude(b => b.PaymentTypes)
                    .FirstOrDefaultAsync();


                var countryInfo = await _helperService.CheckIpAddressForWebsiteAsync(remoteIPAddress, webSite.MarketSubcatagory.Market.Code);
                Log log = new Log();
                log.IsBlack = true;
                log.Country = countryInfo.CountryName;
                log.CountryCode = countryInfo.CountryCode;
                log.Ip = remoteIPAddress;
                log.UserAgent = userAgent;
                log.WebSite = webSite;
                log.MarketSubcatagory = webSite.MarketSubcatagory;

                if (gclid is not null)
                {
                    var googleClickId = await _helperService.CheckGoogleClickIdAsync(gclid, webSite);
                    log.GClid = googleClickId;
                    ViewBag.VisitId = googleClickId.Id;
                }
                else
                {
                    var googleClickId = await _helperService.CheckGoogleClickIdAsync("Anonym", webSite);
                    log.GClid = googleClickId;
                    ViewBag.VisitId = googleClickId.Id;
                }

                await _context.Logs.AddAsync(log);
                await _context.SaveChangesAsync();
                return View(webSite);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return View();
            }
        }

        [Route("/countries")]
        public IActionResult Countries()
        {
            return View();
        }


        public IActionResult PrivacyPolicy()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Advertising()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
    }
}