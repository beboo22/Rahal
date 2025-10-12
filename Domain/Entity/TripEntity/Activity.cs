using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.TripEntity
{
    public class ActivityPublicTrip : BaseEntity
    {
        public string Destination { get; set; } = null!;
        public string Title { get; set; } = null!;
        public decimal FullPrice { get; set; }
        public int SelectedDay { get; set; }
        public string Image { get; set; }
        public TripCategory TripCategory { get; set; }
        public TimeOnly StartAt { get; set; }
        public TimeOnly EndAt { get; set; }


        public int PublicTripId { get; set; }
        [ForeignKey(nameof(PublicTripId))]
        public PublicTrip PublicTrip { get; set; } = null!;

    }
    public class ActivityPrivateTrip : BaseEntity
    {
        public string Destination { get; set; } = null!;
        public string Title { get; set; } = null!;
        public decimal FullPrice { get; set; }
        public int SelectedDay { get; set; }
        public string Image { get; set; }
        public TripCategory TripCategory { get; set; }
        public TimeOnly StartAt { get; set; }
        public TimeOnly EndAt { get; set; }


        public int PrivateTripId { get; set; }
        [ForeignKey("PrivateTripId")]
        public PrivateTrip PrivateTrip { get; set; }

    }
}
