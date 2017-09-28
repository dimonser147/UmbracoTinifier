namespace Tinifier.Core.Models.API
{
    /// <summary>
    /// Response from TinyPng Service consists of two parts : Input and Output
    /// </summary>
    public class TinyResponse
    {
        /// <summary>
        /// Input nonOptimized image
        /// </summary>
        public TinyInput Input { get; set; }

        /// <summary>
        /// Output optimized image
        /// </summary>
        public TinyOutput Output { get; set; }
    }
}
