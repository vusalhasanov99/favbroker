using CybergAgency.Data;
using CybergAgency.Data.Models;

namespace CybergAgency.Services
{
    public interface IHelperService
    {
        Task<CountryInfo> CheckIpAddressForWebsiteAsync(string ipAddress, string webSiteCountryCode);
        Task<GClid> CheckGoogleClickIdAsync(string gClid, WebSite webSite);
        
        bool CheckUserAgent(string userAgent);
    }
}
