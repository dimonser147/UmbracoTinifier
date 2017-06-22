angular.module('umbraco').controller('Tinifier.EditTImage.Controller', function ($scope, $routeParams, $http, notificationsService) {

    // Get the ID from the route parameters (URL)
    var timageId = $routeParams.id;

    // ImageFolderId
    var imagesFolderId = 1090;

    // RecycleBinFolderId
    var recycleBinFolderId = -21;

    // Get the timage from the API
    $scope.timage = null;
    $scope.thistory = null;
    $scope.date = null;
    $scope.percent = null;

    // Check if user choose Image or recycle bin folder
    if (timageId == undefined || timageId == imagesFolderId || timageId == recycleBinFolderId) {
        notificationsService.error("Error", "You cant`t tinify Folder!");
        return;
    }

    // Get Image information
    $http.get('/umbraco/backoffice/api/Tinifier/GetTImage?timageId=' + timageId).then(function (response) {

        if (response.data.history != null)
        {
            $scope.date = response.data.history.OccuredAt.replace('T', ' ');
            response.data.history.OccuredAt = $scope.date;
        }
        
        $scope.timage = response.data.timage;
        $scope.thistory = response.data.history;
        $scope.percent = ((1 - response.data.history.Ratio) * 100).toFixed(2) + "%";
    });
});