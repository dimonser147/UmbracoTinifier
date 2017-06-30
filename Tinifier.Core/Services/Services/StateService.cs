using Tinifier.Core.Infrastructure.Enums;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Repository.Repository;
using Tinifier.Core.Services.Interfaces;

namespace Tinifier.Core.Services.Services
{
    public class StateService : IStateService
    {
        private readonly TStateRepository _stateRepository;
        private readonly TImageRepository _imageRepository;

        public StateService()
        {
            _stateRepository = new TStateRepository();
            _imageRepository = new TImageRepository();
        }

        public void CreateState(int folderId)
        {
            var state = new TState
            {
                CurrentImage = 0,
                AmounthOfImages = _imageRepository.AmounthImagesFromFolder(folderId),
                StatusType = Statuses.InProgress
            };

            _stateRepository.Create(state);
        }

        public TState GetState()
        {
            var state = _stateRepository.GetByKey((int)Statuses.InProgress);

            return state;
        }

        public void UpdateState()
        {
            var state = _stateRepository.GetByKey((int)Statuses.InProgress);

            if(state.CurrentImage < state.AmounthOfImages)
            {
                state.CurrentImage++;
            }

            if (state.CurrentImage == state.AmounthOfImages)
            {
                state.StatusType = Statuses.Done;
            }

            _stateRepository.Update(state);
        }
    }
}
