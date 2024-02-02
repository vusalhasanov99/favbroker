namespace CybergAgency.Data.Models
{
    public class Log : BaseEntity
    {
        public string Ip { get; set; }
        public string UserAgent { get; set; }
        public WebSite WebSite { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public GClid GClid { get; set; }
        public bool IsBlack { get; set; }
        public MarketSubcatagory MarketSubcatagory { get; set; }
    }
}
