using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.StatusService.Qurey.res
{
    public class TemplateStatus
    {
        public int Id { get; set; }
        public string Title { get; set; }


        [Required]
        public string ItemUrl { get; set; }
    }
}
