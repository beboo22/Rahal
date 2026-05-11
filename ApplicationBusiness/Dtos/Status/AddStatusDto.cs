using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Dtos.Status
{
    public class AddStatusDto
    {
        public string? Title { get; set; }


        [Required]
        public IFormFile ItemUrl { get; set; }

    }
}
