angular.module('umbraco').controller('Tinifier.EditTSetting.Controller', function ($scope, $http, $routeParams, $window) {

    // Get the ID from the route parameters (URL)
    var timageId = $routeParams.id;

    // Get settings
    $scope.timage = {};

    // Fill select dropdown
    $scope.options = [
        { value: false, label: 'False' },
        { value: true, label: 'True' },
    ];

    // Fill form from web api
    $http.get('/umbraco/backoffice/api/Tinifier/GetTSetting').success(function (response) {
        $scope.timage = response;
    });

    // Submit form with settings
    $scope.submitForm = function () {
        timage = $scope.timage;
        $http.post('/umbraco/backoffice/api/Tinifier/CreateSettings', JSON.stringify(timage)).success(function (response) {
            $window.location.reload();
            $scope.timage = response;
        });
    };
});