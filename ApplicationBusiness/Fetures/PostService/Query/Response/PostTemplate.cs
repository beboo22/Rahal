using Domain.Entity.Identity;
using Domain.Entity.PostEntity;
using Domain.Entity.TravelerCompanyEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.PostService.Query.Response
{
    public class TemplateUserPost
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string? PrifleUser { get; set; }

    }
    public class HiringPostTemplate
    {
        public TemplateUserPost UserPost { get; set; }
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Requirements { get; set; }
        public string Status { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PhotoUrl { get; set; }

        public ICollection<TemplateComment> Comments { get; set; }
        public ICollection<likesSerive.Query.TemplateuserLikePost> Likes { get; set; }
        public int numLikes { get; set; }
    }
    public class ExperiencePostTemplate
    {
        public TemplateUserPost UserPost { get; set; }
        public ICollection<TemplateComment> Comments { get; set; }
        public ICollection<likesSerive.Query.TemplateuserLikePost> Likes { get; set; }
        public int numLikes { get; set; }
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? PhotoUrl { get; set; }
    }
      
    public class TemplateComment
    {
        public string Msg { get; set; }
        public bool IsEdited { get; set; } = false;
        public TemplateUserPost UserComment { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
