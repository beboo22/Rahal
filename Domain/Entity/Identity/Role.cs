namespace Domain.Entity.Identity
{
        public enum RoleEnum
        {
            Admin,
            Traveler,
            TravelCompany,
            TourGuide
        }

    public class Role:BaseEntity
    {
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public string RoleName { get; set; }
    }

}