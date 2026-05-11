
using ApplicationBusiness.Fetures.PostService.Query.Response;
using Domain.Entity.PostEntity;

namespace ApplicationBusiness.Fetures.likesSerive.Query
{
    public class TemplateuserLikePost
    {
        public TemplateUserPost UserLike { get; set; }

        public LikeType LikeType { get; set; }
    }
}