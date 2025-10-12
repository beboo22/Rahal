using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace ApplicationBusiness.Dtos.Post
{
    public class AddExperiencePostDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string? PhotoUrl { get;  set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string TipsAndRecommendations { get; set; }
        public decimal Budget { get; set; }
    }
    public class AddExperiencePostControllerDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile Photo { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string TipsAndRecommendations { get; set; }
        public decimal Budget { get; set; }
    }
    public class AddHiringPostDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string? PhotoUrl { get;  set; }
        public string Requirements { get; set; }
        public string Status { get; set; }
    }
    public class AddHiringPostControllerDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile Photo {  get; set; }
        public string Requirements { get; set; }
        public string Status { get; set; }
    }
}
