using System;
using Tinifier.Core.Infrastructure;
using Tinifier.Core.Infrastructure.Enums;
using Tinifier.Core.Infrastructure.Exceptions;
using Tinifier.Core.Repository.Image;
using Tinifier.Core.Repository.State;

namespace Tinifier.Core.Services.Validation
{
    public class ValidationService : IValidationService
    {
        private readonly TStateRepository _stateRepository;
        private readonly TImageRepository _imageRepository;

        public ValidationService()
        {
            _stateRepository = new TStateRepository();
            _imageRepository = new TImageRepository();
        }

        public void CheckConcurrentOptimizing()
        {
            var state = _stateRepository.GetByKey((int)Statuses.InProgress);

            if (state != null)
            {
                throw new ConcurrentOptimizingException(PackageConstants.ConcurrentOptimizing);
            }
        }

        public bool IsFolder(int itemId)
        {
            var item = _imageRepository.GetByKey(itemId);

            return string.Equals(item.ContentType.Alias, PackageConstants.FolderAlias, StringComparison.OrdinalIgnoreCase);
        }

        public bool CheckExtension(string source)
        {
            var fileName = source.ToLower();

            return fileName.Contains(PackageConstants.PngExtension) || fileName.Contains(PackageConstants.JpgExtension) 
                || fileName.Contains(PackageConstants.JpeExtension) || fileName.Contains(PackageConstants.JpegExtension);
        }
    }
}
