using System.ComponentModel.DataAnnotations;

namespace FullStackAPi.Models
{
    public class ShoppingCard
    {
        [Key]
        public Guid Id { get; set; }
        public int Pnr { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
    }
}
