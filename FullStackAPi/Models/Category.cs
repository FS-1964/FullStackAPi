using System.ComponentModel.DataAnnotations;

namespace FullStackAPi.Models
{
    public class Category
    {
        [Key]
        public string? CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }
}
