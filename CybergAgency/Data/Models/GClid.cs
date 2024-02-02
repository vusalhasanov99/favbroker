namespace CybergAgency.Data.Models
{
    public class GClid : BaseEntity
    {
        public string GoogleClickID { get; set; }
        public WebSite WebSite { get; set; }
        public MarketSubcatagory MarketSubcatagory { get; set; }
        public List<PostBack> PostBacks { get; set; }
    }
}
