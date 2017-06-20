angular.module("umbraco").controller("Tinifier.TinyTImage.Controller", function ($scope, $routeParams, $http, notificationsService, $window, $timeout) {

    // Get the ID from the route parameters (URL)
    var timageId = $routeParams.id;

    // ImageFolderId
    var imagesFolderId = 1090;

    // RecycleBinFolderId
    var recycleBinFolderId = -21;

    // Get the animal from the API
    $scope.timage = null;
    
    // Tinify Image and show notification
    $scope.tinify = function () {

        // Check if user choose Image or recycle bin folder
        if (timageId == undefined || timageId == imagesFolderId || timageId == recycleBinFolderId)
        {
            notificationsService.error("Error", "You cant`t tinify Folder!");
            return;
        }

        notificationsService.info("Loading.....");

        $http.get('/umbraco/backoffice/api/Tinifier/TinyTImage?timageId=' + timageId).success(function (response) {
            notificationsService.success("Success", response);
            $timeout(function () { $window.location.reload(); }, 3000);
        }).error(function (response) {
            notificationsService.error("Error", response);
        });
    };
});
