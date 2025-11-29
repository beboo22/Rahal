using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.Discount.Query.Responce
{
    public class GenericDiscountTemplate
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal DiscountValue { get; set; }
        public bool IsPercentage { get; set; }
        public int MaxUsage { get; set; }
        public int CurrentUsage { get; set; }
        public int CreatorId { get; set; }
    }
    public class SpecificDiscountTemplate
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public string Code { get; set; }
        public decimal DiscountValue { get; set; }
        public bool IsPercentage { get; set; }
        public int MaxUsage { get; set; }
        public int CurrentUsage { get; set; }
        public int CreatorId { get; set; }
    }

}
