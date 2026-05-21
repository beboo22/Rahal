using ApplicationBusiness.Fetures.TripService.Query.Response;
using Domain.Entity.TripEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.RequestTourGuideForTrip.Query.Response
{
    public class TemplateRequestTourGuide
    {
        public List<TourGuidePrivateRequestDto> PrivateRequests { get; set; } = new();
        public List<TourGuidePublicRequestDto> PublicRequests { get; set; } = new();
    }

    public class TourGuidePrivateRequestDto
    {
        public int RequestId { get; set; }
        public bool IsAccepted { get; set; }
        public PrivateTemplateTrip Trip { get; set; }
    }

    public class TourGuidePublicRequestDto
    {
        public int RequestId { get; set; }
        public bool IsAccepted { get; set; }
        public TemplateTrip Trip { get; set; } // Matches your PublicTemplateTrip target property type
    }

public static class RequestMappingExtensions
    {
        // Maps RequestTourGuidePrivateTrip -> TourGuidePrivateRequestDto
        public static Expression<Func<RequestTourGuidePrivateTrip, TourGuidePrivateRequestDto>> MapToPrivateRequestDto => reg => new TourGuidePrivateRequestDto
        {
            RequestId = reg.Id, // Inherited from BaseEntity
            IsAccepted = reg.Accept ?? false,

            // Inline execution of your original PrivateTrip mapping
            Trip = new PrivateTemplateTrip
            {
                Id = reg.PrivateTrip.Id,
                Title = reg.PrivateTrip.Title,
                Destination = reg.PrivateTrip.Destination,
                Duration = reg.PrivateTrip.Duration,
                From = reg.PrivateTrip.From,
                TripCategory = reg.PrivateTrip.TripCategory,
                Price = reg.PrivateTrip.Price,
                CustomizationFee = reg.PrivateTrip.CustomizationFee,
                TourGuideId = reg.PrivateTrip.TourGuideId,
                StartDate = reg.PrivateTrip.StartDate.HasValue ? reg.PrivateTrip.StartDate.Value : default,
                Activities = reg.PrivateTrip.PrivateActivities.Select(a => new TemplateActivity
                {
                    Id = a.Id,
                    TripCategory = a.TripCategory,
                    Destination = a.Destination,
                    EndAt = a.EndAt,
                    FullPrice = a.FullPrice,
                    Image = a.Image,
                    SelectedDay = a.SelectedDay,
                    StartAt = a.StartAt,
                    Title = a.Title,
                    PlaceId = a.PlaceId,
                    DataId = a.DataId,
                    ActivityType = a.ActivityType,
                    Latitude = a.Latitude,
                    Longitude = a.Longitude,
                    Address = a.Address,
                    Website = a.Website,
                    Phone = a.Phone,
                    Rating = a.Rating,
                    Reviews = a.Reviews,
                    PriceRange = a.PriceRange,
                    Description = a.Description,
                    serviceOption = string.IsNullOrEmpty(a.serviceOption)
                        ? new List<string>()
                        : a.serviceOption.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                }).ToList()
            }
        };

        // Maps RequestTourGuidePulicTrip -> TourGuidePublicRequestDto
        public static Expression<Func<RequestTourGuidePulicTrip, TourGuidePublicRequestDto>> MapToPublicRequestDto => reg => new TourGuidePublicRequestDto
        {
            RequestId = reg.Id, // Inherited from BaseEntity
            IsAccepted = reg.Accept ?? false,

            // Inline execution of your original PublicTemplateTrip mapping
            Trip = new TemplateTrip
            {
                Id = reg.PublicTrip.Id,
                Title = reg.PublicTrip.Title,
                Destination = reg.PublicTrip.Destination,
                Duration = reg.PublicTrip.Duration,
                From = reg.PublicTrip.From,
                TripCategory = reg.PublicTrip.TripCategory,
                Price = reg.PublicTrip.Price,
                NumberOfMember = reg.PublicTrip.CurrentNumberOfMember,
                TourGuideId = reg.PublicTrip.TourGuideId,
                StartDate = reg.PublicTrip.StartDate.HasValue ? reg.PublicTrip.StartDate.Value : default,
                Activities = reg.PublicTrip.PublicActivities.Select(a => new TemplateActivity
                {
                    Id = a.Id,
                    TripCategory = a.TripCategory,
                    Destination = a.Destination,
                    EndAt = a.EndAt,
                    FullPrice = a.FullPrice,
                    Image = a.Image,
                    SelectedDay = a.SelectedDay,
                    StartAt = a.StartAt,
                    Title = a.Title,
                    PlaceId = a.PlaceId,
                    DataId = a.DataId,
                    ActivityType = a.ActivityType,
                    Latitude = a.Latitude,
                    Longitude = a.Longitude,
                    Address = a.Address,
                    Website = a.Website,
                    Phone = a.Phone,
                    Rating = a.Rating,
                    Reviews = a.Reviews,
                    PriceRange = a.PriceRange,
                    Description = a.Description,
                    serviceOption = string.IsNullOrEmpty(a.serviceOption)
                        ? new List<string>()
                        : a.serviceOption.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                }).ToList()
            }
        };
    }

}
