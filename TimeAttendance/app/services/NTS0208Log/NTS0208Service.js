/**
* <copyright company="nhantinsoft.vn">
* Author: Vũ Văn Văn
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
(function () {
    angular.module('app').factory('NTS0208Service', NTS0208Service);
    NTS0208Service.$inject = ['$http', '$q', 'ngAuthSettings'];

    function NTS0208Service($http, $q, ngAuthSettings) {
        var serviceBase = ngAuthSettings.apiServiceBaseUri;
        var NTS0208ServiceFactory = {
            SearchUserEventLog: searchUserEventLog,

        };

        return NTS0208ServiceFactory;

        //Tìm kiếm
        function searchUserEventLog(model, saveOption) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0208/SearchUserEventLog?' + 'saveOption=' + saveOption;
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
    }
})();