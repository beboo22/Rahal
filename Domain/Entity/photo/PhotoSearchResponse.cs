using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.photo
{
    public class PhotoSearchResponse:BaseEntity
    {
        public string SearchId { get; set; } = string.Empty;
        public List<PhotoResultItem> Images { get; set; } = new();
    }

    public class PhotoResultItem:BaseEntity
    {
        public int Position { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public string Original { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public int OriginalWidth { get; set; }
        public int OriginalHeight { get; set; }
    }
}
