using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Dtos.Trip
{
    public class SearchForTripDto
    {
        public string? Destination { get; set; }
        public string? Title { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
