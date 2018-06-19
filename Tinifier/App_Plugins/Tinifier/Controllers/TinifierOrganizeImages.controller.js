angular.module("umbraco").controller("Tinifier.TinifierOrganizeImages.Controller", function ($scope, $routeParams, $http, notificationsService, dialogService, navigationService) {

    $scope.organizeImages = function () {
        var folderId = $routeParams.id > 0 ? $routeParams.id : -1;
        var url = `/umbraco/backoffice/api/ProTinifier/OrganizeImages?folderId=${folderId}`;

        $http.get(url)
            .success(successHandler)
            .error(errorHandler);
    };

    $scope.discardOrganizing = function () {
        var url = "/umbraco/backoffice/api/ProTinifier/DiscardOrganizing";
        notificationsService.success("Discarding is in progress...");

        $http.get(url)
            .success(successDiscardСhanges)
            .error(errorHandler);
    };

    function successHandler(response) {
        notificationsService.success("Success", "The images organized successfully!");
        navigationService.syncTree({ tree: 'media', path: "/umbraco/backoffice/UmbracoTrees/MediaTree/GetMenu?id=-1&application=media&tree=&isDialog=false", forceReload: false, activate: true });

    }

    function successDiscardСhanges(response) {
        notificationsService.success("Success", "Changes are discarded!");
        navigationService.syncTree({ tree: 'media', path: "/umbraco/backoffice/UmbracoTrees/MediaTree/GetMenu?id=-1&application=media&tree=&isDialog=false", forceReload: false, activate: true });

    }

    function errorHandler(response) {

    }
});