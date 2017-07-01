angular.module("umbraco").controller("Tinifier.EditTSetting.Controller", function($scope, $http, $routeParams, notificationsService) {

    // Get settings
    $scope.timage = {};

    // Fill select dropdown
    $scope.options = [
        { value: false, label: "False" },
        { value: true, label: "True" }
    ];

    // Fill form from web api
    $http.get("/umbraco/backoffice/api/Settings/GetTSetting").success(function(response) {
        $scope.timage = response;
    });

    // Submit form with settings
    $scope.submitForm = function() {
        timage = $scope.timage;
        $http.post("/umbraco/backoffice/api/Settings/CreateSettings", JSON.stringify(timage))
            .success(function(response) {
                notificationsService.success("Success", response);
            }).error(function(response) {
                notificationsService.error("Error", response);
            });
    };
});