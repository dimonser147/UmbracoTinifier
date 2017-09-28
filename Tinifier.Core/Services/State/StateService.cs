using Tinifier.Core.Infrastructure.Enums;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Repository.State;

namespace Tinifier.Core.Services.State
{
    public class StateService : IStateService
    {
        private readonly TStateRepository _stateRepository;

        public StateService()
        {
            _stateRepository = new TStateRepository();
        }

        public void CreateState(int numberOfImages)
        {
            var state = new TState
            {
                CurrentImage = 0,
                AmounthOfImages = numberOfImages,
                StatusType = Statuses.InProgress
            };

            _stateRepository.Create(state);
        }

        public TState GetState()
        {
            return _stateRepository.Get((int)Statuses.InProgress);
        }

        public void UpdateState()
        {
            var state = _stateRepository.Get((int) Statuses.InProgress);

            if(state != null)
            {
                if (state.CurrentImage < state.AmounthOfImages)
                    state.CurrentImage++;

                if (state.CurrentImage == state.AmounthOfImages)
                    state.StatusType = Statuses.Done;

                _stateRepository.Update(state);
            }
        }

        public void Delete()
        {
            _stateRepository.Delete();
        }
    }
}
