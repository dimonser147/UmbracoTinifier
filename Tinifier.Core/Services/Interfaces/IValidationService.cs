namespace Tinifier.Core.Services.Interfaces
{
    public interface IValidationService
    {
        // Check folder this or Image
        bool CheckFolder(int itemId);

        // Check concurrent folder optimizing
        void CheckConcurrentOptimizing();

        // Check Image Extension
        bool CheckExtension(string source);
    }
}
