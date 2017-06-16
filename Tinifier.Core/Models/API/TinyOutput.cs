namespace Tinifier.Core.Models.API
{
    // Response from TinyPng Service
    public class TinyOutput
    {
        // Compressed size of image
        public int Size { get; set; }

        // Image type
        public string Type { get; set; }

        // Image width
        public int Width { get; set; }

        // Image height
        public int Height { get; set; }

        // Compressing ratio
        public double Ratio { get; set; }

        // Image url for downloading
        public string Url { get; set; }

        // If tinyPng returns error
        public string Error { get; set; }

        // If image was optimized
        public bool IsOptimized { get; set; }
    }
}
