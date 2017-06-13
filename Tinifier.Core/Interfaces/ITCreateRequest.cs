using System.Threading.Tasks;

namespace Tinifier.Core.Interfaces
{
    public interface ITCreateRequest
    {
        //CreateRequest to TinyPng Service with input byte array
        Task<string> CreateRequestByteArray(byte[] imageByteArray);

        //CreateRequest to TinyPng Service with input imageUrl
        Task<string> CreateRequestJsonObject(string imageUrl);
    }
}
