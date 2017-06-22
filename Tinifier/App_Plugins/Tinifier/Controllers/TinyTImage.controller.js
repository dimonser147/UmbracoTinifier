angular.module("umbraco").controller("Tinifier.TinyTImage.Controller", function ($scope, $routeParams, $http, notificationsService, dialogService) {

    // Get the ID from the route parameters (URL)
    var timageId = $routeParams.id;

    // RecycleBinFolderId
    var recycleBinFolderId = -21;

    // Get from the API
    $scope.timage = null;
    
    // Tinify Image and show notification
    $scope.tinify = function () {

        // Check if user choose Image or recycle bin folder
        if (timageId == recycleBinFolderId)
        {
            notificationsService.error("Error", "You cant`t tinify Folder!");
            return;
        }

        notificationsService.info("Tinifing.....");

        $http.get('/umbraco/backoffice/api/Tinifier/TinyTImage?timageId=' + timageId).success(function (response) {
            notificationsService.success("Success", response);

            dialogService.open({
                template: "/App_Plugins/Tinifier/BackOffice/timages/edit.html"
            });

        }).error(function (response) {
            notificationsService.error("Error", response);
        });
    };
});
