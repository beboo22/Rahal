namespace Domain.Abstraction
{
    public class GetDashbourdDataDto
    {
        public int TotalUsers { get; set; }
        public int TotalTravelcompany { get; set; }
        public int TotalUnverifiedTravelcompany { get; set; }
        public int TotalTourgiude { get; set; }
        public int TotalUnverifiedTourgiude { get; set; }
        public List<MonthlyUserCreation> ToTalUserCreatedInEveryMonth { get; set; }
        public int TotalUnValidPost { get; set; }
        public double PercentageUvalidPost { get; set; }
        public List<PercentageUvalidPostEverymonth> PercentageUvalidPostEverymonth { get; set; }
        public List<TopDestinationInTrips> TopDestinationInTrips { get; set; }
        public int TotalPost { get; set; }
    }
    public class TopDestinationInTrips
    {
        public string Destination { get; set; }
        public int Count { get; set; }
        public double PercentageDestination { get; set; }
    }
    public class PercentageUvalidPostEverymonth
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public double PercentageUnvalidPosts { get; set; }
    }
    public class MonthlyUserCreation
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int UserCount { get; set; }
    }
}