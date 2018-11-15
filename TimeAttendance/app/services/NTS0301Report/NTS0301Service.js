/**
* <copyright company="nhantinsoft.vn">
* Author: Vũ Văn Văn
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
(function () {
    angular.module('app').factory('NTS0301Service', NTS0301Service);
    NTS0301Service.$inject = ['$http', '$q', 'ngAuthSettings'];

    function NTS0301Service($http, $q, ngAuthSettings) {
        var serviceBase = ngAuthSettings.apiServiceBaseUri;
        var NTS0301ServiceFactory = {
            SearchTimeAttendanceLog: searchTimeAttendanceLog,
            SearchTimeAttendanceLogDetail: searchTimeAttendanceLogDetail,
            TransactionLog: transactionLog,
            DensityLog: densityLog,
            GetAllDepartment: getAllDepartment,
            GetAllJobTitle: getAllJobTitle,
        };

        return NTS0301ServiceFactory;
        function getAllJobTitle() {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTSCombobox/GetAllJobTitle';
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
        //Lấy thông tin combobox
        function getAllDepartment() {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTSCombobox/GetAllDepartment';
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
        //Tìm kiếm
        function searchTimeAttendanceLogDetail(model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0201/SearchTimeAttendanceLogDetail';
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

        function searchTimeAttendanceLog(model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0201/SearchTimeAttendanceLog';
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

        function transactionLog(model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0201/TransactionLog';
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

        function densityLog(model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0201/DensityLog';
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