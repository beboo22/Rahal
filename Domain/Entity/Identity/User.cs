using Domain.Entity.PostEntity;
using Domain.Entity.TourGuidEntity;
using Domain.Entity.TravelerCompanyEntity;
using Domain.Entity.TravelerEntity;
using Domain.Entity.TripEntity;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entity.Identity
{
    public class User : BaseEntity
    {
        [Required]
        public string FName { get; set; } = null!;

        [Required]
        public string LName { get; set; } = null!;

        [Required]
        public int Age { get; set; }

        #region Blocked

        public int? BlockedCounter { get; set; } = 0;
        public bool? IsBlocked { get; set; } = false;
        public DateTime? BlockedEndDate { get; set; }
        public DateTime? BlockedStartDate { get; set; }
        #endregion

        public bool Isverified { get; set; } = false;

        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public bool IsActive { get; set; } = true;

        public decimal? FinancialBalance { get; set; }

        public DateTime? LastLogoutTime { get; set; }

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }

        public virtual ICollection<Languages> Languages { get; set; }

        public virtual ICollection<PhoneNumber> phoneNumbers { get; set; }

        public ICollection<UserRole> Roles { get; set; } = new List<UserRole>();

        public ICollection<BookingPublicTrip> BookingPublicTrips { get; set; }
        public ICollection<BookingPrivateTrip> BookingPrivateTrips { get; set; }

        public ICollection<Trip> CreatedTrips { get; set; }

        public ICollection<Review> ReviewsWritten { get; set; }

        public ICollection<Review> ReviewsReceived { get; set; }

        public ICollection<ExperiencePost> Posts { get; set; }

        public Traveler? TravelerProfile { get; set; }
        public TourGuide? TourGuideProfile { get; set; }
        public TravelCompany? TravelerCompanyProfile { get; set; }
        public Admin? AdminProfile { get; set; }


        // OTP Fields
        public string? OtpCode { get; set; }

        public DateTime? OtpExpiry { get; set; }


        // Progressive Delay Fields
        public int OtpRequestCount { get; set; } = 0;

        public DateTime? NextOtpAllowedAt { get; set; }

        public DateTime? LastOtpRequestTime { get; set; }


        // Generate OTP
        public void GenerateOtp(int length = 6, int expiryMinutes = 3)
        {
            var random = Random.Shared;

            OtpCode = string.Concat(
                Enumerable.Range(0, length)
                .Select(_ => random.Next(0, 10).ToString())
            );

            OtpExpiry = DateTime.UtcNow.AddMinutes(expiryMinutes);
        }


        // Check OTP Request Permission
        public bool CanRequestOtp(out string message, out double remaining)
        {
            var now = DateTime.UtcNow;
            message = "";
            remaining = 0;

            // Reset counter after 1 hour
            if (LastOtpRequestTime.HasValue &&
                LastOtpRequestTime.Value.AddHours(1) < now)
            {
                OtpRequestCount = 0;
            }

            // Check cooldown
            if (NextOtpAllowedAt.HasValue &&
                NextOtpAllowedAt > now)
            {
                remaining =
                    (NextOtpAllowedAt.Value - now)
                    .TotalMinutes;

                message =
                    $"Try again after {Math.Ceiling(remaining)} minutes";

                return false;
            }

            return true;
        }


        // Register OTP Request
        public DateTime RegisterOtpRequest()
        {
            OtpRequestCount++;

            int delayMinutes = OtpRequestCount switch
            {
                1 => 0,
                2 => 3,
                3 => 5,
                4 => 8,
                5 => 12,
                _ => 15
            };

            NextOtpAllowedAt =
                DateTime.UtcNow.AddMinutes(delayMinutes);

            LastOtpRequestTime =
                DateTime.UtcNow;
            return NextOtpAllowedAt.Value;
        }


        public bool ValidateOtp(string otp)
        {
            return OtpCode == otp &&
                   OtpExpiry.HasValue &&
                   OtpExpiry > DateTime.UtcNow;
        }


        public void ClearOtp()
        {
            OtpCode = null;
            OtpExpiry = null;
        }
    }
}