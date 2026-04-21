using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.Hotel_flights
{
    public enum TravelClass
    {
        Economy = 1,
        PremiumEconomy = 2,
        Business = 3,
        First = 4
    }
    public enum TripType
    {
        RoundTrip = 1,
        OneWay = 2,
        MultiCity = 3
    }
    public enum HotelSortOption
    {
        Relevance = 3,
        LowestPrice = 8,
        HighestRating = 13,
        MostReviewed = 14
    }
}
