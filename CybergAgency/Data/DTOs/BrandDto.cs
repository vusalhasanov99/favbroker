namespace CybergAgency.Data.DTOs
{
    public class BrandDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? URL { get; set; }
        public string? Logo { get; set; }
        public string? Option1 { get; set; }
        public string? Option2 { get; set; }
        public string? Option3 { get; set; }
        public double Bonus { get; set; }
        public string? BonusDesciription { get; set; }
        public bool TopSlot { get; set; }
        public int Place { get; set; }
        public int SlotPlace { get; set; }
    }
}
