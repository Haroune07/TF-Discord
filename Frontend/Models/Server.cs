using Frontend.ViewModels;

namespace Frontend.Models
{
    public class Server : ServerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string ImageUrl { get; set; }
    }
}
