using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net;
using System.Threading.Tasks;
using Tinifier.Core.Controllers;
using Tinifier.Core.Infrastructure.Exceptions;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Services.Interfaces;

namespace Tinifier.UnitTests
{
    [TestClass]
    public class OptimizationImagesTests
    {
        private readonly TinifierController _tinifierController;
        private readonly Mock<IHistoryService> _historyService;

        public OptimizationImagesTests()
        {
            _tinifierController = new TinifierController();
            _historyService = new Mock<IHistoryService>();
        }

        [TestInitialize]
        public void SetUp()
        {
        }

        [TestMethod]
        [ExpectedException(typeof(EntityNotFoundException), AllowDerivedTypes = true)]
        public async Task TinyTImage_ThrowNotFoundException_WhenImageWithSuchIdNotFound()
        {
            string[] imagesSrc = new string[0];

            await _tinifierController.TinyTImage(imagesSrc);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException), AllowDerivedTypes = true)]
        public async Task TinyTImage_ThrowNotSupportedTypeException_WhenImageHasNotSupportedType()
        {
            var imageId = 1520;
            string[] imagesSrc = new string[0];

            await _tinifierController.TinyTImage(imagesSrc, imageId);
        }

        [TestMethod]
        public async Task TinyTImage_ReturnsStatusCodeOK_WhenImageSuccessfullyOptimized()
        {
            var imageId = 1517;
            string[] imagesSrc = new string[0];

            var result = await _tinifierController.TinyTImage(imagesSrc, imageId);

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public async Task TinyTImage_ReturnsBadRequest_WhenImageAlreadyOptimized()
        {
            var imageId = 1517;
            string[] imagesSrc = new string[0];
            var responseHistory = new TinyPNGResponseHistory
            {
                IsOptimized = false
            };

            _historyService.Setup(x => x.GetImageHistory(imageId)).Returns(responseHistory);

            var result = await _tinifierController.TinyTImage(imagesSrc, imageId);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        public void TinyTImage_ReturnsStatusCodeOk_WhenFolderSuccessfullyOptimized()
        {
            string[] imagesSrc = new string[1521];

        /*    var result = await _tinifierController.TinyTImage(imagesSrc, imageId);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);*/
        }
    }
}
