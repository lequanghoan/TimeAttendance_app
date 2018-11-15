(function () {
    'use strict';

    angular.module('app').factory('NotifyService', NotifyService);

    // Inject some dependencies
    NotifyService.$inject = ['$http', '$q', 'ngAuthSettings'];

    function NotifyService($http, $q, ngAuthSettings) {
        var serviceBase = ngAuthSettings.apiServiceBaseUri;

        var NotifyServiceServiceFactory = {
            GetNotifyDocument: getNotifyDocument,
        };

        return NotifyServiceServiceFactory;

        //Get notify document
        function  getNotifyDocument(model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/PCTP0302/GetNotify';

            $http.post(url, JSON.stringify(model), { headers: { 'Content-Type': 'application/json' } })
                .success(function (response) {
                    deferred.resolve(response);
                    return response;
                })
                .error(function (errMessage, statusCode) {
                    var result = { isSuccess: false, status: statusCode, message: errMessage };
                    deferred.reject(result);
                    return result;
                });

            return deferred.promise;
        }
    }
})();