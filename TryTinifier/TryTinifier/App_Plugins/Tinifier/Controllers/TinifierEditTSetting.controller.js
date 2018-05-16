angular.module("umbraco").controller("Tinifier.TinifierEditTSetting.Controller", function ($scope, $http, $routeParams, notificationsService) {

    // Get settings
    $scope.timage = {};

    // Fill select dropdown
    $scope.options = [
        { value: false, label: "False" },
        { value: true, label: "True" }
    ];

    // Fill form from web api
    $http.get("/umbraco/backoffice/api/TinifierSettings/GetTSetting").success(function (response) {
        $scope.timage = response;
    });

    // Submit form with settings
    $scope.submitForm = function() {
        timage = $scope.timage;
        $http.post("/umbraco/backoffice/api/TinifierSettings/PostTSetting", JSON.stringify(timage))
            .success(function(response) {
                notificationsService.success("Success", response.message);
            }).error(function(response) {
                if (response.Error === 1) {
                    notificationsService.warning("Warning", response.message);
                }
                else {
                    notificationsService.error("Error", response.message);
                }
            });
    };

    $scope.stopTinifing = function () {
        $http.delete("/umbraco/backoffice/api/TinifierState/DeleteActiveState").success(function (response) {
            notificationsService.success("Success", "Tinifing successfully stoped!");
        }).error(function (response) {
            notificationsService.error("Error", "Tinifing can`t stop now!");
        });
    };
});