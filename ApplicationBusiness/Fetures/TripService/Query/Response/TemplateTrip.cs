using Domain.Entity.Identity;
using Domain.Entity.TripEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.TripService.Query.Response
{
    public class TemplateTrip
    {

        public int CreatedById { get; set; }
        public List<int>? IncludedPackages { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string From { get; set; }
        public string Destination { get; set; }
        public int Duration { get; set; }
        public decimal Price { get; set; }
        public TripCategory TripCategory { get; set; }

        public int? NumberOfMember { get; set; }
        public DateTime? StartDate { get; set; }
        public List<TemplateActivity>? Activities { get; set; }
        public decimal TravelerFee { get; internal set; }
        public int? TourGuideId { get; internal set; }
    }
    public class PrivateTemplateTrip
    {

        public int CreatedById { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string From { get; set; }
        public string Destination { get; set; }
        public int Duration { get; set; }
        public decimal Price { get; set; }
        public TripCategory TripCategory { get; set; }
        public DateTime? StartDate { get; set; }

        //public ICollection<Review> Reviews { get; set; }
        public int? TourGuideId { get; set; }
        public decimal? CustomizationFee { get; set; }
        public List<TemplateActivity> Activities { get; set; }
        public decimal TravelerFee { get;  set; }
    }

    public static class TripMappingExtensions
    {
        // 1. Map Private Trip Entity -> PrivateTemplateTrip DTO
        public static Expression<Func<PrivateTrip, PrivateTemplateTrip>> MapToPrivateTemplate => x => new PrivateTemplateTrip
        {
            Id = x.Id,
            Title = x.Title,
            Destination = x.Destination,
            Duration = x.Duration,
            From = x.From,
            TripCategory = x.TripCategory,
            Price = x.Price,
            CustomizationFee = x.CustomizationFee,
            TourGuideId = x.TourGuideId,
            StartDate = x.StartDate.HasValue ? x.StartDate.Value : default,

            Activities = x.PrivateActivities.Select(a => new TemplateActivity
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
        };

        // 2. Map Public Trip Entity -> PublicTemplateTrip DTO
        public static Expression<Func<PublicTrip, TemplateTrip>> MapToPublicTemplate => x => new TemplateTrip
        {
            Id = x.Id,
            Title = x.Title,
            Destination = x.Destination,
            Duration = x.Duration,
            From = x.From,
            TripCategory = x.TripCategory,
            Price = x.Price,
            // Assuming public trips might have different properties like MaxPeople instead of CustomizationFee
            NumberOfMember = x.MaxNumberOfMember,
            TourGuideId = x.TourGuideId,
            StartDate = x.StartDate.HasValue ? x.StartDate.Value : default,

            Activities = x.PublicActivities.Select(a => new TemplateActivity
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
        };
    }

}
