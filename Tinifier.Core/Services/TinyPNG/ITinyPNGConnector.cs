using System.Threading.Tasks;
using Tinifier.Core.Models.API;

namespace Tinifier.Core.Services.TinyPNG
{
    public interface ITinyPNGConnector
    {
        // Send Image to TinyPNG Service 
        Task<TinyResponse> SendImageToTinyPngService(string imageUrl);
    }
}
