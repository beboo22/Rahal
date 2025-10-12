namespace Domain.Entity.TripEntity
{
    [Flags]
    public enum Package : byte
    {
        None = 0,
        Food = 1,    // 0000 0001
        Accommodation = 2,    // 0000 0010
        Transportation = 4,    // 0000 0100
        TourGuide = 8,    // 0000 1000
        Activities = 16,   // 0001 0000
        Insurance = 32,   // 0010 0000
        Visa = 64,   // 0100 0000
        Other = 128   // 1000 0000
    }

}