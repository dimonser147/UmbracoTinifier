angular.module('umbraco').controller('Tinifier.EditTSetting.Controller', function ($scope, $http) {

    $scope.tsetting = {};

    $scope.submitForm = function () {
        $http.post('/umbraco/backoffice/api/Tinifier/CreateSettings', tsetting).success(function () { notificationsService.success("Success", response); });
    };
});