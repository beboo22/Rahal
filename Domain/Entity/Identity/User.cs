using Domain.Entity.PostEntity;
using Domain.Entity.TourGuidEntity;
using Domain.Entity.TravelerCompanyEntity;
using Domain.Entity.TravelerEntity;
using Domain.Entity.TripEntity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.Identity
{
    public class User:BaseEntity
    {
        [Required]
        public string FName { get; set; } = null!;
        [Required]
        public string LName { get; set; } = null!;
        [Required]
        public int Age { get; set; }

        #region Blocked
        public bool? IsBlocked { get; set; }=false;
        public DateTime? BlockedEndDate { get; set; }
        public DateTime? BlockedStartDate { get; set; }
        #endregion

        public bool Isverified { get; set; } = false;


        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string PasswordHash { get; set; } = null!;
        [Required]
        public bool IsActive { get; set; } = true;

        public decimal? FinancialBalance { get; set; }

        public DateTime? LastPasswordResetTime { get; set; }
        public DateTime? LastLogoutTime { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
        public virtual ICollection<PasswordResetToken> PasswordResetTokens { get; set; }

        public virtual ICollection<Languages> Languages { get; set; }
        public virtual ICollection<PhoneNumber> phoneNumbers { get; set; }
        public ICollection<UserRole> Roles { get; set; } = new List<UserRole>();
        public ICollection<BookingTrip> BookingTrips { get; set; }
        public ICollection<Trip> CreatedTrips { get; set; }
        public ICollection<Review> ReviewsWritten { get; set; }
        public ICollection<Review> ReviewsReceived { get; set; }
        public ICollection<ExperiencePost> Posts { get; set; }



        // navigation to role-specific extensions
        public Traveler? TravelerProfile { get; set; }
        public TourGuide? TourGuideProfile { get; set; }
        public TravelCompany? TravelerCompanyProfile { get; set; }
        public Admin? AdminProfile { get; set; }


    }
}
