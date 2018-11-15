/**
* <copyright company="nhantinsoft.vn">
* Author: Vũ Văn Văn
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
(function () {
    angular.module('app').factory('JobTitleService', JobTitleService);
    JobTitleService.$inject = ['$http', '$q', 'ngAuthSettings'];

    function JobTitleService($http, $q, ngAuthSettings) {
        var serviceBase = ngAuthSettings.apiServiceBaseUri;
        var JobTitleServiceFactory = {
            GetListJobTitle: getListJobTitle,
            GetJobTitleInfo: getJobTitleInfo,
            AddJobTitle: addJobTitle,
            UpdateJobTitle: updateJobTitle,
            DeleteJobTitle: deleteJobTitle
        };

        return JobTitleServiceFactory;

        //Tìm kiếm
        function getListJobTitle(model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/JobTitle001/GetListJobTitle';
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

        //Xóa thông tin
        function deleteJobTitle(id) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/JobTitle001/DeleteJobTitle?jobTitleId=' + id;
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

        //Cập nhật trạng thái
        function updateJobTitle(model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/JobTitle001/UpdateJobTitle';
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

        //Thêm mới
        function addJobTitle(model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/JobTitle001/AddJobTitle';
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

        //Lấy thông tin theo Id
        function getJobTitleInfo(id) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/JobTitle001/GetJobTitleInfo?JobTitleId=' + id;
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