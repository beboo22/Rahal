using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Dtos.Post
{
    public record UpdateExperiencePostDto
    {
        public int Id { get;  set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string TipsAndRecommendations { get; set; }
        public decimal Budget { get; set; }
    }
    public record UpdateHiringPostDto
    {
        public int Id { get;  set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Requirements { get; set; }
        public string Status { get; set; }
    }
}
