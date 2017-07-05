using Tinifier.Core.Models.Db;

namespace Tinifier.Core.Services.Interfaces
{
    public interface IStateService
    {
        // Get state from database to show for user
        TState GetState();

        // Update state 
        void UpdateState();

        // Create state in bulk tinifing
        void CreateState(int numberOfImages);
    }
}
