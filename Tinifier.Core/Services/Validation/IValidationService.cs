namespace Tinifier.Core.Services.Validation
{
    public interface IValidationService
    {
        /// <summary>
        /// Check folder this or Image
        /// </summary>
        /// <param name="mediaId">Media Id</param>
        /// <returns>If this folder or not</returns>
        bool IsFolder(int mediaId);

        /// <summary>
        /// Check concurrent folder optimizing
        /// </summary>
        void ValidateConcurrentOptimizing();

        /// <summary>
        /// Check Image Extension
        /// </summary>
        /// <param name="source"></param>
        /// <returns>Supported exception or not</returns>
        void ValidateExtension(Umbraco.Core.Models.Media media);
    }
}
