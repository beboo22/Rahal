using ApplicationBusiness.Dtos.Auth;
using ApplicationBusiness.Dtos.Profile;
using ApplicationBusiness.Fetures.BookingTripService.Query.Response;
using ApplicationBusiness.Fetures.PostService.Query.Response;
using ApplicationBusiness.Fetures.Profile.Command;
using ApplicationBusiness.Fetures.TripService.Query.Response;
using Domain.Entity.Identity;
using Domain.Entity.PostEntity;
using Domain.Entity.TripEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.Authentication.Query.Response
{
    public class TemplateStatusOfFollowing
    {
        public string UserName { get; set; }
        public string UserPhoto { get; set; }
        public List<StatusViewModel> UserStatus { get; set; }
    }

    public static class UserTemplateMapper
    {
        public static TemplateTourGuide MapTourGuide(
    User user)
        {
            if (user?.TourGuideProfile == null)
                return null;

            return new TemplateTourGuide
            {
                Id = user.Id,
                PhotoUrl = user.TourGuideProfile.PhotoUrl,
                Bio = user.TourGuideProfile.Bio,
                Ssn = user.TourGuideProfile.Ssn,

                //BusinessGalaries = user.TourGuideProfile
                //    .tourGuidBusinessGalaries?
                //    .Select(x => new BusinessGalaryDto
                //    {
                //        Date = x.Date,
                //        Description = x.Description,
                //        Location = x.Location,
                //        PhotoUrl = x.PhotoUrl
                //    }).ToList(),

                
                        City = user.TourGuideProfile.City,
                        Country = user.TourGuideProfile.Country,
                        BuildingNumber = user.TourGuideProfile.BuildingNumber,
                        Street = user.TourGuideProfile.Street,
                  

                ExperiencePostTemplates = user.Posts?
                    .OfType<ExperiencePost>()
                    .Select(MapExperiencePost)
                    .ToList(),

            };
        }
        public static TemplateTraveler MapTraveler(User user)
        {
            if (user?.TravelerProfile == null)
                return null;

            return new TemplateTraveler
            {
                Id = user.Id,
                PhotoUrl = user.TravelerProfile.PhotoUrl,
                Bio = user.TravelerProfile.Bio,
                Ssn = user.TravelerProfile.Ssn,
                    City = user.TravelerProfile.City,
                    Country = user.TravelerProfile.Country,
                    BuildingNumber = user.TravelerProfile.BuildingNumber,
                    Street = user.TravelerProfile.Street,

                ExperiencePostTemplates = user.Posts?
                    .OfType<ExperiencePost>()
                    .Select(MapExperiencePost)
                    .ToList(),

                PublicTrips = user.PublicTrips?
                    .Select(MapPrivateTrip)
                    .ToList(),

                BookedTrip = user.BookingPublicTrips?
                    .Select(MapBookingTrip)
                    .ToList()
            };
        }

        public static TemplateTokenTraveler MapTokenTraveler(
            User user,
            Token token)
        {
            if (user?.TravelerProfile == null)
                return null;

            return new TemplateTokenTraveler
            {
                profile = new TemplateTraveler
                {

                Id = user.Id,
                PhotoUrl = user.TravelerProfile.PhotoUrl,
                Bio = user.TravelerProfile.Bio,
                Ssn = user.TravelerProfile.Ssn,
                    City = user.TravelerCompanyProfile.City,
                    Country = user.TravelerCompanyProfile.Country,
                    BuildingNumber = user.TravelerCompanyProfile.BuildingNumber,
                    Street = user.TravelerCompanyProfile.Street,



                    ExperiencePostTemplates = user.Posts?
                    .OfType<ExperiencePost>()
                    .Select(MapExperiencePost)
                    .ToList(),
                }

                //PrivateTrips = user.CreatedTrips?
                //    .Select(MapPrivateTrip)
                //    .ToList(),

            };
        }

        public static TemplateTravelComapny MapTravelCompany(
            User user)
        {
            if (user?.TravelerCompanyProfile == null)
                return null;

            return new TemplateTravelComapny
            {
                Id = user.Id,
                PhotoUrl = user.TravelerCompanyProfile.PhotoUrl,
                Bio = user.TravelerCompanyProfile.Bio,
                Ssn = user.TravelerCompanyProfile.Ssn,

                //BusinessGalaries = user.TravelerCompanyProfile
                //    .travelCompanyBusinessGalaries?
                //    .Select(x => new BusinessGalaryDto
                //    {
                //        Date = x.Date,
                //        Description = x.Description,
                //        Location = x.Location,
                //        PhotoUrl = x.PhotoUrl
                //    }).ToList(),

                
                        City= user.TravelerCompanyProfile.City,
                        Country =user.TravelerCompanyProfile.Country,
                        BuildingNumber = user.TravelerCompanyProfile.BuildingNumber,
                        Street = user.TravelerCompanyProfile.Street,
                    

                ExperiencePostTemplates = user.Posts?
                    .OfType<ExperiencePost>()
                    .Select(MapExperiencePost)
                    .ToList(),

                //PrivateTrips = user.CreatedTrips?
                //    .Select(MapPrivateTrip)
                //    .ToList(),

            };
        }

        private static ExperiencePostTemplate MapExperiencePost(
            ExperiencePost post)
        {
            return new ExperiencePostTemplate
            {
                Id = post.Id,
                CreatedAt = post.CreatedAt,
                Country = post.Country,
                City = post.City,
                Title = post.Title,
                Description = post.Description,
                PhotoUrl = post.PhotoUrl,

                UserPost = new TemplateUserPost
                {
                    Id = post.CreatedBy?.Id ?? 0,
                    FullName =
                        $"{post.CreatedBy?.FName} {post.CreatedBy?.LName}",

                    PrifleUser =
                        post.CreatedBy?.TravelerProfile != null
                            ? post.CreatedBy.TravelerProfile.PhotoUrl
                            : post.CreatedBy?.TourGuideProfile != null
                                ? post.CreatedBy.TourGuideProfile.PhotoUrl
                                : post.CreatedBy?.TravelerCompanyProfile != null
                                    ? post.CreatedBy.TravelerCompanyProfile.PhotoUrl
                                    : null
                },

                Comments = post.Comments?
                    .Select(MapComment)
                    .ToList(),

                Likes = post.Likes?.Select(x => new likesSerive.Query.TemplateuserLikePost
                {
                    LikeType = x.LikeType,
                    UserLike = new TemplateUserPost
                    {
                        Id = x.UserId,
                        // Use ?. to prevent crash if x.User is null
                        FullName = x.User != null ? x.User.FName + " " + x.User.LName : "Unknown User",

                        PrifleUser = x.User?.TravelerProfile != null
                     ? x.User.TravelerProfile.PhotoUrl
                     : x.User?.TourGuideProfile != null
                         ? x.User.TourGuideProfile.PhotoUrl
                         : null
                    }
                }).ToList() ?? new List<likesSerive.Query.TemplateuserLikePost>(),

                numLikes = post.Likes?.Count ?? 0
            };
        }

        private static TemplateComment MapComment(Comment comment)
        {
            return new TemplateComment
            {
                Msg = comment.Msg,
                CreatedAt = comment.CreatedAt,
                IsEdited = comment.IsEdited,

                UserComment = new TemplateUserPost
                {
                    Id = comment.User?.Id ?? 0,
                    FullName =
                        $"{comment.User?.FName} {comment.User?.LName}",

                    PrifleUser =
                        comment.User?.TravelerProfile != null
                            ? comment.User.TravelerProfile.PhotoUrl
                            : comment.User?.TourGuideProfile != null
                                ? comment.User.TourGuideProfile.PhotoUrl
                                : comment.User?.TravelerCompanyProfile != null
                                    ? comment.User.TravelerCompanyProfile.PhotoUrl
                                    : null
                }
            };
        }

        private static TemplateTrip MapPrivateTrip(PublicTrip trip)
        {
            return new TemplateTrip
            {
                Id = trip.Id,
                Title = trip.Title,
                Price = trip.Price,
                Destination = trip.Destination,
                Duration = trip.Duration,
                StartDate = trip.StartDate,
                From = trip.From,
                IncludedPackages = Enum.GetValues(typeof(Package)).Cast<Package>().Where(p => p != Package.None && trip.IncludedPackages.HasFlag(p)).Select(p => (int)p).ToList(),
                NumberOfMember = trip.CurrentNumberOfMember,
                TripCategory = trip.TripCategory,


                Activities = trip.PublicActivities?.Select(x=>new TemplateActivity
                {
                    Id = x.Id,
                    Title = x.Title,
                    DataId = x.DataId,
                    Description = x.Description,
                    Destination = x.Destination,
                    ActivityType = x.ActivityType,
                    SelectedDay = x.SelectedDay,
                    Address = x.Address,
                    EndAt = x.EndAt,
                    FullPrice = x.FullPrice,
                    Image = x.Image,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    Phone = x.Phone,
                    PlaceId = x.PlaceId,
                    PriceRange = x.PriceRange,
                    Rating = x.Rating,
                    Reviews = x.Reviews,
                    StartAt = x.StartAt,
                    TripCategory = x.TripCategory,
                    Website = x.Website,
                    serviceOption = x.serviceOption?
    .Split(',', StringSplitOptions.RemoveEmptyEntries)
    .ToList()
    ?? new List<string>(),
                }).ToList(),


                // Complete based on your model
            };
        }

        private static BookingTripTemplate MapBookingTrip(Booking booking)
        {
            return new BookingTripTemplate
            {
                Id = booking.Id,
                BookingDate = booking.BookingDate,
                IsPaid = booking.IsPaid,
                TotalBookingPrice = booking.TotalBookingPrice,
                //TripTilte = booking.t
                
                // Complete based on your model
            };
        }
    }


}
