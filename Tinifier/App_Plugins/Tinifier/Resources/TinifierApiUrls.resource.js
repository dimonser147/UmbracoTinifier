function tinifierApiUrlsResource($q, $http, umbRequestHelper) {
    return {
        statistic: umbRequestHelper.getApiUrl('tinifierApiRoot', 'TinifierImagesStatistic'),
        state: umbRequestHelper.getApiUrl('tinifierApiRoot', 'TinifierState'),
        tinifier: umbRequestHelper.getApiUrl('tinifierApiRoot', 'Tinifier'),
        settings: umbRequestHelper.getApiUrl('tinifierApiRoot', 'TinifierSettings'),
        mediaTree: umbRequestHelper.getApiUrl('mediaTreeBaseUrl', 'GetMenu')
    };
}
angular.module("umbraco.resources").factory("tinifierApiUrlsResource", tinifierApiUrlsResource);