angular.module("umbraco").controller("Tinifier.TinifierOrganizeImages.Controller", function ($scope, $routeParams, $http, notificationsService, dialogService, navigationService) {

    $scope.organizeImages = function () {
        var folderId = $routeParams.id > 0 ? $routeParams.id : -1;
        var url = `/umbraco/backoffice/api/Tinifier/OrganizeImages?folderId=${folderId}`;
        navigationService.hideDialog();
        notificationsService.success("Organizing is in progress...");
        $http.get(url).then(
            function (response) {
                console.log('get', response);
                successHandler(response);
            },
            function (data) {
                errorHandler(data);
            })
    };

    $scope.discardOrganizing = function () {
        var url = "/umbraco/backoffice/api/Tinifier/DiscardOrganizing";
        notificationsService.success("Discarding is in progress...");
        navigationService.hideDialog();
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
        if (response.status == 409) {
            notificationsService.error("Forbidden", "Please wait until images saving are completed. Then try again");
        }
        else {
            notificationsService.error("Error");
        }
    }
});