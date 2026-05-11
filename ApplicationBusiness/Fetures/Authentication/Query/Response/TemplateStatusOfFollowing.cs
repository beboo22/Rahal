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

                BusinessGalaries = user.TourGuideProfile
                    .tourGuidBusinessGalaries?
                    .Select(x => new BusinessGalaryDto
                    {
                        Date = x.Date,
                        Description = x.Description,
                        Location = x.Location,
                        PhotoUrl = x.PhotoUrl
                    }).ToList(),

                Adresses = user.TourGuideProfile
                    .tourGuidAddresses.Select(x => new Adress
                    {
                        City = x.City,
                        Country = x.Country,
                        BuildingNumber = x.BuildingNumber,
                        Street = x.Street,
                    })
                    .ToList(),

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
                Adresses = user.TravelerProfile.trvelerAddresses? .Select(x => new Adress
                {
                    City = x.City,
                    Country = x.Country,
                    BuildingNumber = x.BuildingNumber,
                    Street = x.Street,
                }).ToList(),

                ExperiencePostTemplates = user.Posts?
                    .OfType<ExperiencePost>()
                    .Select(MapExperiencePost)
                    .ToList(),

                //PrivateTrips = user.CreatedTrips?
                //    .Select(MapPrivateTrip)
                //    .ToList(),

                //BookedTrip = user.Bookings?
                //    .Select(MapBookingTrip)
                //    .ToList()
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
                Id = user.Id,
                PhotoUrl = user.TravelerProfile.PhotoUrl,
                Bio = user.TravelerProfile.Bio,
                Ssn = user.TravelerProfile.Ssn,
                Adresses = user.TravelerProfile.trvelerAddresses?.Select(x => new Adress
                {
                    City = x.City,
                    Country = x.Country,
                    BuildingNumber = x.BuildingNumber,
                    Street = x.Street,
                }).ToList(),

                ExperiencePostTemplates = user.Posts?
                    .OfType<ExperiencePost>()
                    .Select(MapExperiencePost)
                    .ToList(),

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

                BusinessGalaries = user.TravelerCompanyProfile
                    .travelCompanyBusinessGalaries?
                    .Select(x => new BusinessGalaryDto
                    {
                        Date = x.Date,
                        Description = x.Description,
                        Location = x.Location,
                        PhotoUrl = x.PhotoUrl
                    }).ToList(),

                Adresses = user.TravelerCompanyProfile
                    .traveleCompanyAddresses?.Select(x=>new Adress
                    {
                        City= x.City,
                        Country =x.Country,
                        BuildingNumber = x.BuildingNumber,
                        Street = x.Street,
                    })
                    .ToList(),

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

                Likes = post.Likes?
                    .Select(x => new likesSerive.Query.TemplateuserLikePost
                    {
                        UserLike= new TemplateUserPost
                        {
                        Id = x.UserId,
                        FullName = x.User.FName+" "+x.User.LName,
                        PrifleUser = x.User.TravelerProfile != null
                                    ? x.User.TravelerProfile.PhotoUrl
                                    : x.User.TourGuideProfile != null
                                        ? x.User.TourGuideProfile.PhotoUrl
                                        : null
                        }
                        
                    }).ToList(),

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

        private static PrivateTemplateTrip MapPrivateTrip(Trip trip)
        {
            return new PrivateTemplateTrip
            {
                Id = trip.Id,
                Title = trip.Title
                // Complete based on your model
            };
        }

        private static BookingTripTemplate MapBookingTrip(Booking booking)
        {
            return new BookingTripTemplate
            {
                Id = booking.Id
                // Complete based on your model
            };
        }
    }


}
