namespace Tinifier.Core.Services.Validation
{
    public interface IValidationService
    {
        /// <summary>
        /// Check folder this or Image
        /// </summary>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        bool IsFolder(int mediaId);

        // Check concurrent folder optimizing
        void CheckConcurrentOptimizing();

        // Check Image Extension
        bool CheckExtension(string source);
    }
}
