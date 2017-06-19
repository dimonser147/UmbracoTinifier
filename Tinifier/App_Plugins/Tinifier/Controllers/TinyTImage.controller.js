angular.module("umbraco").controller("Tinifier.TinyTImage.Controller", function ($scope, $routeParams, $http, notificationsService, $window, $timeout) {

    // Get the ID from the route parameters (URL)
    var timageId = $routeParams.id;

    // Get the animal from the API
    $scope.timage = null;

    $scope.tinify = function () {
        $http.get('/umbraco/backoffice/api/Tinifier/TinyTImage?timageId=' + timageId).success(function (response) {
            notificationsService.success("Success", response);
            $timeout(function () { $window.location.reload(); }, 2000);
        }).error(function (response) {
            notificationsService.error("Error", response);
        });
    };
});
