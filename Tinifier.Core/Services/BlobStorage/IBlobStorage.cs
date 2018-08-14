using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;

namespace Tinifier.Core.Services.BlobStorage
{
    public interface IBlobStorage
    {
        void SetDataForBlobStorage();

        byte[] GetBlob(string blobName);

        void PutBlob(string blobName, byte[] content);

        void DeleteBlob(string blobName);

        IEnumerable<IListBlobItem> GetAllBlobsInContainer();

        int CountBlobsInContainer();

        bool DoesContainerExist();

        bool DoesBlobExist(string blobName);
    }
}
