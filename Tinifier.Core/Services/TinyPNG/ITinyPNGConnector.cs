using System.Threading.Tasks;
using Tinifier.Core.Models.API;
using Tinifier.Core.Models.Db;
using Umbraco.Core.IO;

namespace Tinifier.Core.Services.TinyPNG
{
    public interface ITinyPNGConnector
    {
        /// <summary>
        /// Send Image to TinyPNG Service
        /// </summary>
        /// <param name="imageUrl">Image url</param>
        /// <returns>Task<TinyResponse></returns>
        Task<TinyResponse> TinifyAsync(TImage tImage, IFileSystem fs);
    }
}
