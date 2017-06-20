angular.module('umbraco').controller('Tinifier.EditTImage.Controller', function ($scope, $routeParams, $http) {

    // Get the ID from the route parameters (URL)
    var timageId = $routeParams.id;

    // Get the timage from the API
    $scope.timage = null;
    $scope.thistory = null;

    // Get Image information
    $http.get('/umbraco/backoffice/api/Tinifier/GetTImage?timageId=' + timageId).then(function (response) {
        $scope.timage = response.data.timage;
        $scope.thistory = response.data.history;
        });    
});