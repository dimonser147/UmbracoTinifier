angular.module('umbraco').controller('Tinifier.EditTImage.Controller', function ($scope, $routeParams, $http) {

    // Get the ID from the route parameters (URL)
    var timageId = $routeParams.id;

    // Get the animal from the API
    $scope.timage = null;

    $scope.getImage = function () {
        $http.get('/umbraco/backoffice/api/Tinifier/GetTImage?timageId=' + timageId).success(function (response) {
            $scope.timage = response;
        });
    };    
});