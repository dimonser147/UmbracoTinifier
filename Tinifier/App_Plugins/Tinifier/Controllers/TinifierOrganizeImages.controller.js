angular.module("umbraco").controller("Tinifier.TinifierOrganizeImages.Controller", function ($scope, $routeParams, $http, notificationsService, dialogService) {

    $scope.organizeImages = function () {
        var folderId = $routeParams.id > 0 ? $routeParams.id : -1;
        var url = `/umbraco/backoffice/api/ProTinifier/OrganizeImages?folderId=${folderId}`;

        $http.get(url)
            .success(successHandler)
            .error(errorHandler);
    };

    $scope.discardOrganizing = function () {
        var url = "/umbraco/backoffice/api/ProTinifier/DiscardOrganizing";

        $http.get(url)
            .success(successDiscardСhanges)
            .error(errorHandler);
    };

    function successHandler(response) {
        notificationsService.success("Success", "The images organized successfully!");
    }

    function successDiscardСhanges(response) {
        notificationsService.success("Success", "Changes are discarded!");
    }

    function errorHandler(response) {

    }
});