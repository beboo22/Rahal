using Domain.Entity.TripEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.TripService.Query.Response
{
    public class TemplateTrip
    {
        public int Id { get;  set; }
        public string Title { get;  set; }
        public string From { get;  set; }
        public string Destination { get;  set; }
        public int Duration { get;  set; }
        public decimal Price { get;  set; }
        public TripCategory TripCategory { get;  set; }
        public Package? IncludedPackages { get;  set; }
        public int? NumberOfMember { get;  set; }
        public DateTime StartDate { get;  set; }
        public List<TemplateActivity> Activities { get;  set; }
    }
}
