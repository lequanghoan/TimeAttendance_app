/**
* <copyright company="nhantinsoft.vn">
* Author: Vũ Văn Văn
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
(function () {
    angular.module('app').factory('NTS0201Service', NTS0201Service);
    NTS0201Service.$inject = ['$http', '$q', 'ngAuthSettings'];

    function NTS0201Service($http, $q, ngAuthSettings) {
        var serviceBase = ngAuthSettings.apiServiceBaseUri;
        var NTS0201ServiceFactory = {
            SearchEmployee: searchEmployee,
            DeleteEmployee: deleteEmployee,
            GetEmployeeById: getEmployeeById,
            CreateEmployee: createEmployee,
            UpdateEmployee: updateEmployee,
            GetAllDepartment: getAllDepartment,
            GetAllJobTitle: getAllJobTitle,
        };

        return NTS0201ServiceFactory;

        //Tìm kiếm
        function searchEmployee(model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0201/SearchEmployee';
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

        //Thêm mới
        function createEmployee(file, model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0201/CreateEmployee';
            var fd = new FormData();
            for (var i = 0; i < file.length; i++) {
                fd.append('File0' + i, file[i].file);
            }
            fd.append('Model', angular.toJson(model));
            $http.post(url, fd, {
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            })
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

        //Cập nhật thông tin
        function updateEmployee(file, model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0201/UpdateEmployee';
            var fd = new FormData();
            for (var i = 0; i < file.length; i++) {
                fd.append('File0' + i, file[i].file);
            }
            fd.append('Model', angular.toJson(model));
            $http.post(url, fd, {
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            })
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
        function deleteEmployee(id, CreateBy) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0201/DeleteEmployee?id=' + id + '&CreateBy=' + CreateBy;
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

        //Lấy thông tin theo Id
        function getEmployeeById(id) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0201/GetEmployeeById?id=' + id;
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

        //Lấy thông tin combobox nhóm người dùng
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

    }
})();