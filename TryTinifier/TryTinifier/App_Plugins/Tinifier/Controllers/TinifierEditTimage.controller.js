angular.module("umbraco").controller("Tinifier.TinifierEditTImage.Controller", function ($scope, $routeParams, $http, notificationsService) {

    // Get the ID from the route parameters (URL)
    var timageId = $routeParams.id;

    // RecycleBinFolderId
    var recycleBinFolderId = -21;

    // Get the timage from the API
    $scope.timage = null;
    $scope.thistory = {
        OccuredAt: "-",
        Ratio: "-",
        OriginSize: "-",
        OptimizedSize: "-"
    };
    $scope.date = null;
    $scope.percent = "-";
    $scope.notOptimized = null;

    // Check if user choose Image or recycle bin folder
    if (timageId === recycleBinFolderId) {
        notificationsService.error("Error", "You cant`t tinify Folder!");
        return;
    }

    // Get Image information
    $http.get("/umbraco/backoffice/api/Tinifier/GetTImage?timageId=" + timageId).success(function(response) {

        if (response.history != null && response.history.IsOptimized) {
            $scope.date = response.history.OccuredAt.replace("T", " ");
            response.history.OccuredAt = $scope.date;
            $scope.thistory = response.history;
            $scope.percent = ((1 - response.history.Ratio) * 100).toFixed(2) + "%";
        } else {
            $scope
                .notOptimized =
                "Image is not optimized, please click 'Tinify' button in the 'Actions' menu to optimize it";
            document.getElementById("pandaDiv").style.display = "none";
        }

        $scope.timage = response.timage;

    }).error(function(response) {
        if (response.Error === 1) {
            notificationsService.warning("Warning", response.message);
        }
        else {
            notificationsService.error("Error", response.message);
        }
    });
});