namespace Tinifier.Core.Models.API
{
    //Response from TinyPng Service
    public class TinyOutput
    {
        //Compressed size of image
        public int Size { get; set; }

        //Image type
        public string Type { get; set; }

        //Image width
        public int Width { get; set; }

        //Image height
        public int Height { get; set; }

        //Compressing ratio
        public decimal Ratio { get; set; }

        //Image url for downloading
        public string Url { get; set; }
    }
}
