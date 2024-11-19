using System.ComponentModel.DataAnnotations;

namespace EDI315Api.Models
{
    public class WatchlistModel
    {
        [Key]
        public int Id { get; set; }
        
        public string UserId { get; set; } 
        public string ContainerNumber { get; set; } 
    }
}
