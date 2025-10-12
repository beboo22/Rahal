using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Dtos.Profile
{
    public class UpdateTourGuideProfileDto
    {
        public string Bio { get; set; }
        public string Ssn { get; set; }
        public List<Adress> Adresses { get; set; }
        public decimal SalaryPerDay { get; set; }
    }
    public class UpdateTravelerProfileDto
    {
        public string Bio { get; set; }
        public string Ssn { get; set; }
        public List<Adress> Adresses { get; set; }
    }
    public class UpdateTravelerCompanyProfileDto
    {
        public string Bio { get; set; }
        public string Ssn { get; set; }
        public List<Adress> Adresses { get; set; }
    }
}
