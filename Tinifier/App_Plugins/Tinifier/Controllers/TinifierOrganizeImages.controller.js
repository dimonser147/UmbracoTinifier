angular.module("umbraco").controller("Tinifier.TinifierOrganizeImages.Controller", function ($scope, $routeParams, $http, notificationsService,
    dialogService, navigationService, tinifierApiUrlsResource) {
    $scope.organizeImages = function () {
        var folderId = $routeParams.id > 0 ? $routeParams.id : -1;
        var url = `${tinifierApiUrlsResource.tinifier}/OrganizeImages?folderId=${folderId}`;
        navigationService.hideDialog();
        notificationsService.success("Organizing is in progress...");
        $http.get(url).then(
            function (response) {
                successHandler(response);
            },
            function (data) {
                errorHandler(data);
            })
    };

    $scope.discardOrganizing = function () {
        var folderId = $routeParams.id > 0 ? $routeParams.id : -1;
        var url = `${tinifierApiUrlsResource.tinifier}/DiscardOrganizing?folderId=${folderId}`;

        notificationsService.success("Discarding is in progress...");
        navigationService.hideDialog();
        $http.get(url).then(
            function (response) {
                successDiscardСhanges(response);
            },
            function (data) {
                errorHandler(data);
            })
    };

    function successHandler(response) {
        if (response.data && response.data.message)
            notificationsService.info("Info", response.data.message);
        else
            notificationsService.success("Success", "The images organized successfully!");
        navigationService.syncTree({ tree: 'media', path: `${tinifierApiUrlsResource.mediaTree}?id=-1&application=media&tree=&isDialog=false`, forceReload: false, activate: true });
    }

    function successDiscardСhanges(response) {
        if (response.data && response.data.message)
            notificationsService.info("Info", response.data.message);
        else
            notificationsService.success("Success", "Changes are discarded!");
        navigationService.syncTree({ tree: 'media', path: `${tinifierApiUrlsResource.mediaTree}?id=-1&application=media&tree=&isDialog=false`, forceReload: false, activate: true });
    }

    function errorHandler(response) {
        if (response.status == 409) {
            notificationsService.error("Forbidden", "Please wait until images saving are completed. Then try again");
        }
        else {
            notificationsService.error("Error");
        }
    }
});