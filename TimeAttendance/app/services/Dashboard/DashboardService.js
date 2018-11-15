/**
* <copyright company="nhantinsoft.vn">
* Author: Vũ Văn Văn
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
(function () {
    angular.module('app').factory('DashboardService', DashboardService);
    DashboardService.$inject = ['$http', '$q', 'ngAuthSettings'];

    function DashboardService($http, $q, ngAuthSettings) {
        var serviceBase = ngAuthSettings.apiServiceBaseUri;
        var DashboardServiceFactory = {
            GetDataDashboard: getDataDashboard
        };

        return DashboardServiceFactory;

        //Tìm kiếm
        function getDataDashboard() {
            var deferred = $q.defer();
            var url = serviceBase + 'api/Dashboard/GetDataDashboard';
            $http.post(url, { headers: { 'Content-Type': 'application/json;charset=utf-8' } })
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