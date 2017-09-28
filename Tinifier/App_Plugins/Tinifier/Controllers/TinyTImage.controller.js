angular.module("umbraco").controller("Tinifier.TinyTImage.Controller", function($scope, $routeParams, $http, notificationsService, dialogService) {

    // Get the ID from the route parameters (URL)
    var timageId = $routeParams.id;

    var arrOfNames = [];
    var selectedImages = document.querySelectorAll(".-selected");

    for (var i = 0; i < selectedImages.length; i++) {
        var innerHtml = selectedImages[i].innerHTML;
        var regex = /<img.*?src=['"](.*?)['"]/;
        var src = regex.exec(innerHtml)[1];
        var slice = src.split("?")[0];
        arrOfNames.push(slice);
    }

    // RecycleBinFolderId
    var recycleBinFolderId = -21;

    // Get from the API
    $scope.timage = null;

    // Tinify Image and show notification
    $scope.tinify = function() {

        // Check if user choose Image or recycle bin folder
        if (timageId === recycleBinFolderId) {
            notificationsService.error("Error", "You can`t tinify RecycleBin Folder!");
            return;
        }

        notificationsService
            .info("Tinifing.... Optimization started ! If you tinifing more than one Image you can see progress in the Tinifier 'Statistic' section");

        if (arrOfNames.length !== 0) {
            $http.get("/umbraco/backoffice/api/Tinifier/TinyTImage?" + $.param({ imageRelativeUrls: arrOfNames, mediaId: 0 }))
                .success(function(response) {
                    notificationsService.success("Success", response.SuccessOptimized);
                }).error(function(response) {
                    notificationsService.error("Error", response.Message);
                });
        } else {
            $http.get("/umbraco/backoffice/api/Tinifier/TinyTImage?mediaId=" + timageId).success(function (response) {
                notificationsService.success("Success", response.SuccessOptimized);
                if (response.sourceType === 0) {
                    dialogService.open({
                        template: "/App_Plugins/Tinifier/BackOffice/timages/TinifierEdit.html"
                    });
                }
            }).error(function (response) {
                if (response.Error === 1){
                    notificationsService.warning("Warning", response.Message);
                }
                else {
                    notificationsService.error("Error", response.Message);
                }
            });
        }
    };

    $scope.tinifyAll = function () {
        $http.put("/umbraco/backoffice/api/Tinifier/TinifyEverything")
                .success(function (response) {
                    notificationsService.success("Success", response.SuccessOptimized);
                }).error(function (response) {
                    notificationsService.error("Error", response.Message);
                });
    };
});
