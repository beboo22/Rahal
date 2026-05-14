using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Dtos.Profile
{
    public class UpdateTourGuideProfileDto
    {
        public IFormFile? photo { get; set; }
        public string? Bio { get; set; }
        public string? Ssn { get; set; }
        public decimal? SalaryPerDay { get; set; }

        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? BuildingNumber { get; set; }
        //public List<Adress>? Adresses { get; set; }
        //public decimal SalaryPerDay { get; set; }
    }
    public class UpdateTravelerProfileDto
    {
        public IFormFile? photo { get; set; }
        public string? Bio { get; set; }
        public string? Ssn { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? BuildingNumber { get; set; }
        //public List<Adress>? Adresses { get; set; }
    }
    public class UpdateTravelerCompanyProfileDto
    {
        public IFormFile? photo { get; set; }
        public string? Bio { get; set; }
        public string? Ssn { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? BuildingNumber { get; set; }
        //public List<Adress>? Adresses { get; set; }
    }
}
