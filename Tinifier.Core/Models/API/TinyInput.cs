namespace Tinifier.Core.Models.API
{
    /// <summary>
    /// Response from TinyPng Service
    /// </summary>
    public class TinyInput
    {
        /// <summary>
        /// Origin size of image
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Type of Image
        /// </summary>
        public string Type { get; set; }
    }
}
