namespace WebApplication1.Models
{
    public class URL
    {
        public int Id { get; set; }
        public string LongUrl { get; set; }
        public string ShortUrl { get; set; }        
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
