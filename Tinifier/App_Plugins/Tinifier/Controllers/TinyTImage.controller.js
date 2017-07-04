angular.module("umbraco").controller("Tinifier.TinyTImage.Controller", function($scope, $routeParams, $http, notificationsService, dialogService) {

    // Get the ID from the route parameters (URL)
    var timageId = $routeParams.id;

    var arrOfIds = [];
    var selectedImages = document.querySelectorAll(".-selected");

    for (var i = 0; i < selectedImages.length; i++)
    {
        var innerHtml = selectedImages[i].innerHTML;
        var regex = /<img.*?src=['"](.*?)['"]/;
        var src = regex.exec(innerHtml)[1];
        var Id = src.split('/')[2];
        arrOfIds.push(Id);
    }

    if (arrOfIds.length === 0)
    {
        arrOfIds.push(timageId);
    }

    // RecycleBinFolderId
    var recycleBinFolderId = -21;
    var sourceTypeImageId = 2;

    // Get from the API
    $scope.timage = null;

    // Tinify Image and show notification
    $scope.tinify = function() {

        // Check if user choose Image or recycle bin folder
        if (timageId === recycleBinFolderId) {
            notificationsService.error("Error", "You cant`t tinify Folder!");
            return;
        }

        notificationsService.info("Tinifing..... Optimization started and if you tinifing Folder you can see progress in the Tinifier section");

        $http.get("/umbraco/backoffice/api/Tinifier/TinyTImage", {"items": arrOfIds }).success(function (response) {
            notificationsService.success("Success", response.SuccessOptimized);

            if (response.sourceType === sourceTypeImageId)
            {
                dialogService.open({
                    template: "/App_Plugins/Tinifier/BackOffice/timages/edit.html"
                });
            }

        }).error(function(response) {
            notificationsService.error("Error", response);
        });
    };
});
