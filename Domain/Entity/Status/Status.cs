using Domain.Entity.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.Status
{
    public class Status:BaseEntity
    {
        public Status()
        {
            EndDate = CreatedAt.AddDays(1);
        }
        public User CreatedBy { get; set; }
        public int CreatedById { get; set; }



        public string Title { get; set; }


        [Required]
        public string ItemUrl {  get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<StatusUser> StatusUsers { get; set; }
    }
    public class StatusUser:BaseEntity
    {
        public int StatusId { get; set; }
        public Status Status { get; set; }
        public int viewById { get; set; }
        public User viewBy { get; set; }
        public bool Isloved { get; set; }
    }
}
