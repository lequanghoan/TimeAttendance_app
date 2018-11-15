/**
* <copyright company="nhantinsoft.vn">
* Author: Vũ Văn Văn
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
(function () {
    angular.module('app').factory('ConfigService', ConfigService);
    ConfigService.$inject = ['$http', '$q', 'ngAuthSettings'];

    function ConfigService($http, $q, ngAuthSettings) {
        var serviceBase = ngAuthSettings.apiServiceBaseUri;
        var ConfigServiceFactory = {
            UpdateConfig: updateConfig,
            GetConfig: getConfig,
          
        };

        return ConfigServiceFactory;

        //Tìm kiếm
        function updateConfig(model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTSCombobox/UpdateConfigResult';
            $http.post(url, JSON.stringify(model), { headers: { 'Content-Type': 'application/json;charset=utf-8' } })
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

        //Lấy thông tin 
        function getConfig() {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTSCombobox/GetConfigResult';
            $http.post(url, { headers: { 'Content-Type': 'application/json' } })
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