namespace TenderOps_API.Models
{
    public class TenderCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Tender> Tenders { get; set; } = new List<Tender>();
    }
}
