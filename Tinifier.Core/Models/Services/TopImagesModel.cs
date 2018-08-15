using System;

namespace Tinifier.Core.Models.Services
{
    public class TopImagesModel
    {
        public string ImageId { get; set; }

        public long OriginSize { get; set; }

        public long OptimizedSize { get; set; }

        public DateTime OccuredAt { get; set; }
    }
}
