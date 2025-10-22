using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Dtos.Profile
{
    public class CreateTourGuideProfileDto
    {
        public string Photo { get; set; }

        public decimal SalaryPerDay { get; set; }

        #region verification
        // Incoming files from client
        //public IFormFile FrontIdentityPhoto { get; set; } = null!;
        //public IFormFile BackIdentityPhoto { get; set; } = null!;

        //// URLs after upload (you'll fill these later in your service)
        //public string FrontIdentityPhotoUrl { get; internal set; }
        //public string BackIdentityPhotoUrl { get; internal set; }

        public string Bio { get; set; }
        public string Ssn { get; set; }
        public List<Adress> Adresses { get; set; }
        public BusinessGalary BusinessGalaries { get; set; }
        #endregion
    }

    public class CreateTravelerProfileDto
    {
        public string Photo { get; set; }

        #region verification
        //// Incoming files from client
        //public IFormFile FrontIdentityPhoto { get; set; } = null!;
        //public IFormFile BackIdentityPhoto { get; set; } = null!;

        //// URLs after upload (you'll fill these later in your service)
        //public string FrontIdentityPhotoUrl { get; internal set; }
        //public string BackIdentityPhotoUrl { get; internal set; }

        public string Bio { get; set; }
        public string Ssn { get; set; }
        public List<Adress> Adresses { get; set; }
        #endregion
    }
    public class CreateTravelerCompanyProfileDto
    {
        public string Photo { get; set; }

        #region verification
        //// Incoming files from client
        //public IFormFile FrontIdentityPhoto { get; set; } = null!;
        //public IFormFile BackIdentityPhoto { get; set; } = null!;

        //// URLs after upload (you'll fill these later in your service)
        //public string FrontIdentityPhotoUrl { get; internal set; }
        //public string BackIdentityPhotoUrl { get; internal set; }

        public string Bio { get; set; }
        public string Ssn { get; set; }
        public List<Adress> Adresses { get; set; }
        public BusinessGalary BusinessGalaries { get; set; }
        #endregion
    }
    
}
