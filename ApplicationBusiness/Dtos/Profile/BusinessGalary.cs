using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Dtos.Profile
{
    public class BusinessGalary
    {
        [FromForm]
        public IFormFile Photo { get; set; }
        public DateOnly Date { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
    }
    public class BusinessGalaryDto
    {
        public string PhotoUrl { get; internal set; }
        public DateOnly Date { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
    }
}
