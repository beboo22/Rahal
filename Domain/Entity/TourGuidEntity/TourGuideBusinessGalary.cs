using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.TourGuidEntity
{
    public class TourGuideBusinessGalary
    {
        public int Id { get; set; }


        public int TourGuidId { get; set; }
        public TourGuide TourGuid{ get; set; }

        public string? PhotoUrl { get; set; }
        public DateOnly Date { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }



    }
}
