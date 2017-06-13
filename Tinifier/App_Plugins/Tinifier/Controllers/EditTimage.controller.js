angular.module('umbraco').controller('Tinifier.EditTImage.Controller', function ($scope, $routeParams, $http) {

    // Get the ID from the route parameters (URL)
    var timageId = $routeParams.id;

    // Get the animal from the API
    $scope.timage = null;
    $http.get('/umbraco/backoffice/api/Tinifier/GetTImage?timageId=' + timageId).success(function (response) {
        $scope.timage = response;
    });

    //Get tiny image from the API
    $scope.tinyImage = function () {
        $http.put('/umbraco/backoffice/api/Tinifier/TinyTImage?timageId=' + timageId).success(function (response) {
            $scope.timage = response;
        });
    };
});