using Tinifier.Core.Models.Db;

namespace Tinifier.Core.Services.State
{
    public interface IStateService
    {
        /// <summary>
        /// Get state from database to show for user
        /// </summary>
        /// <returns>TState</returns>
        TState GetState();

        /// <summary>
        /// Update state
        /// </summary>
        void UpdateState();

        /// <summary>
        /// Create state 
        /// </summary>
        /// <param name="numberOfImages">Number of Images</param>
        void CreateState(int numberOfImages);

        /// <summary>
        /// Delete active state
        /// </summary>
        void Delete();
    }
}
