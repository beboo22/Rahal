using Domain.Entity.Identity;
using Domain.Entity.TripEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.TripService.Query.Response
{
    public class TemplateTrip
    {

        public int CreatedById { get; set; }
        public List<int>? IncludedPackages { get;  set; }
        public int Id { get;  set; }
        public string Title { get;  set; }
        public string From { get;  set; }
        public string Destination { get;  set; }
        public int Duration { get;  set; }
        public decimal Price { get;  set; }
        public TripCategory TripCategory { get;  set; }

        public int? NumberOfMember { get;  set; }
        public DateTime? StartDate { get;  set; }
        public List<TemplateActivity>? Activities { get;  set; }
        public decimal TravelerFee { get; internal set; }
    }
    public class PrivateTemplateTrip
    {

        public int CreatedById { get; set; }
        public int Id { get;  set; }
        public string Title { get;  set; }
        public string From { get;  set; }
        public string Destination { get;  set; }
        public int Duration { get;  set; }
        public decimal Price { get;  set; }
        public TripCategory TripCategory { get;  set; }
        public DateTime? StartDate { get; set; }

        //public ICollection<Review> Reviews { get; set; }
        public int? TourGuideId { get; set; }
        public decimal? CustomizationFee { get; set; }
        public List<TemplateActivity> Activities { get;  set; }
    }


}
