using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.TravelerCompanyEntity
{
    public class TravelCompanyBusinessGalary
    {
        public int Id { get; set; }
        public int TravelCompanyId { get; set; }
        public TravelCompany TravelCompany { get; set; }
        public string PhotoUrl { get; set; }
        public DateOnly Date { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
    }
}
