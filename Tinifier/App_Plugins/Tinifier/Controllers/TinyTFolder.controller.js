angular.module("umbraco").controller("Tinifier.TinyTFolder.Controller", function ($scope, $routeParams, $http, notificationsService, dialogService) {

    // Get the ID from the route parameters (URL)
    var folderId = $routeParams.id;

    // RecycleBinFolderId
    var recycleBinFolderId = -21;

    // Get from the API
    $scope.timage = null;

    // Tinify Image and show notification
    $scope.tinify = function() {

        // Check if user choose Image or recycle bin folder
        if (folderId === recycleBinFolderId) {
            notificationsService.error("Error", "You cant`t tinify Folder!");
            return;
        }

        notificationsService
            .info("Tinifing..... Bulk optimization started and you can see progress in the Tinifier section");

        $http.get("/umbraco/backoffice/api/Tinifier/TinyTFolder?folderId=" + folderId).success(function(response) {
            notificationsService.success("Success", response);

        }).error(function(response) {
            notificationsService.error("Error", response);
        });
    };
});