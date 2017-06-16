using System.Threading.Tasks;
using Tinifier.Core.Models.API;

namespace Tinifier.Core.Interfaces
{
    public interface ITinyPNGConnector
    {
        // CreateRequest to TinyPng Service with input byte array
        Task<TinyResponse> TinifyByteArray(byte[] imageByteArray);

        // CreateRequest to TinyPng Service with input imageUrl
        Task<TinyResponse> TinifyJsonObject(string imageUrl);
    }
}
