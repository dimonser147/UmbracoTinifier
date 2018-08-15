using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace Tinifier.Core.Services.BlobStorage
{
    public class AzureBlobStorageService : IBlobStorage
    {
        private CloudBlobClient _blobClient;
        private CloudBlobContainer _blobContainer;
        private string _containerName;

        public void SetDataForBlobStorage()
        {
            var path = HttpContext.Current.Server.MapPath("~/config/FileSystemProviders.config");
            var doc = new XmlDocument();
            doc.Load(path);
            var node = doc.SelectSingleNode("//Provider");

            if (node != null)
            {
                var keysNodes = node.LastChild.SelectNodes("//add");
                var dictionary = new Dictionary<string, string>();

                foreach (XmlNode xmlNode in keysNodes)
                    dictionary.Add(xmlNode.Attributes.GetNamedItem("key").Value,
                        xmlNode.Attributes.GetNamedItem("value").Value);

                var connectionString = dictionary.First(x => x.Key == "connectionString").Value;
                var containerName = dictionary.First(x => x.Key == "containerName").Value;

                var storageAccount = CloudStorageAccount.Parse(connectionString);
                _blobClient = storageAccount.CreateCloudBlobClient();
                _containerName = containerName;
                _blobContainer = _blobClient.GetContainerReference(containerName);
            }
        }

        public byte[] GetBlob(string blobName)
        {
            byte[] fileAsByteArray;
            var blockBlob = _blobContainer.GetBlockBlobReference(blobName);

            using (var memoryStream = new MemoryStream())
            {
                blockBlob.DownloadToStream(memoryStream);
                fileAsByteArray = memoryStream.ToArray();
            }

            return fileAsByteArray;
        }

        public void PutBlob(string blobName, byte[] content)
        {
            var blockBlob = _blobContainer.GetBlockBlobReference(blobName);

            using (var stream = new MemoryStream(content, false))
                 blockBlob.UploadFromStream(stream);
        }

        public void DeleteBlob(string blobName)
        {
            var blockBlob = _blobContainer.GetBlockBlobReference(blobName);
            blockBlob.DeleteIfExists();
        }

        public IEnumerable<IListBlobItem> GetAllBlobsInContainer()
        {
            return _blobContainer.ListBlobs(useFlatBlobListing: true);
        }

        public int CountBlobsInContainer()
        {
            return _blobContainer.ListBlobs(useFlatBlobListing: true).Count();
        }

        public bool DoesContainerExist()
        {
            var containers = _blobClient.ListContainers();
            return containers.Any(one => one.Name == _containerName);
        }

        public bool DoesBlobExist(string blobName)
        {
            var blockBlob = _blobContainer.GetBlockBlobReference(blobName);
            return blockBlob.Exists();
        }
    }
}
