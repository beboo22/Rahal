using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Attachments : BaseEntity
    {
        public string FileId { get; set; } = string.Empty;
        public string Extend { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
    }
}
