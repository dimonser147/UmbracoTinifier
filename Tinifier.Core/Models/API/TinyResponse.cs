namespace Tinifier.Core.Models.API
{
    //Response from TinyPng Service consists of two parts : Input and Output
    public class TinyResponse
    {
        public TinyInput Input { get; set; }

        public TinyOutput Output { get; set; }
    }
}
