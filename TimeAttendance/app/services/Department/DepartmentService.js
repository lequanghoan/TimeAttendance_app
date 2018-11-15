/**
* <copyright company="nhantinsoft.vn">
* Author: Vũ Văn Văn
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
(function () {
    angular.module('app').factory('DepartmentService', DepartmentService);
    DepartmentService.$inject = ['$http', '$q', 'ngAuthSettings'];

    function DepartmentService($http, $q, ngAuthSettings) {
        var serviceBase = ngAuthSettings.apiServiceBaseUri;
        var DepartmentServiceFactory = {
            GetListDepartment: getListDepartment,
            GetDepartmentInfo: getDepartmentInfo,
            AddDepartment: addDepartment,
            UpdateDepartment: updateDepartment,
            DeleteDepartment: deleteDepartment
        };

        return DepartmentServiceFactory;

        //Tìm kiếm
        function getListDepartment(model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/Department001/GetListDepartment';
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
        function deleteDepartment(id) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/Department001/DeleteDepartment?departmentId=' + id;
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
        function updateDepartment( model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/Department001/UpdateDepartment';
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
        function addDepartment(model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/Department001/AddDepartment';
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
        function getDepartmentInfo(id) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/Department001/GetDepartmentInfo?departmentId=' + id;
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