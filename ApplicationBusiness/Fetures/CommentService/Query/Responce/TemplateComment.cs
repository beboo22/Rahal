using ApplicationBusiness.Dtos.Auth;
using Domain.Entity.Identity;
using Domain.Entity.PostEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.CommentService.Query.Responce
{
    internal class TemplateComment
    {
        public string Msg { get; set; }
        public bool IsEdited { get; set; } = false;
        public int UserId { get; set; }

        public string? UserPhoto { get; set; }
        public string UserName { get; set; }

        public int PostId { get; set; }
    }
}
