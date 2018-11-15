/**
* <copyright company="nhantinsoft.vn">
* Author: Vũ Văn Văn
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
(function () {
    angular.module('app').factory('AttendanceLogService', AttendanceLogService);
    AttendanceLogService.$inject = ['$http', '$q', 'ngAuthSettings'];

    function AttendanceLogService($http, $q, ngAuthSettings) {
        var serviceBase = ngAuthSettings.apiServiceBaseUri;
        var AttendanceLogServiceFactory = {
            GetListAttendanceLog: getListAttendanceLog,
            GetListNoAttendanceLog: getListNoAttendanceLog,
            ExportExcel: exportExcel,
            DeleteAttendanceLog: deleteAttendanceLog,
            UpdateAttendanceLog: updateAttendanceLog,
            GetListEmployee: getListEmployee,
            GetAttendanceLogInfo: getAttendanceLogInfo,
            GetListDepartment: getListDepartment,
            Training: training,
            GetAvataEmp: getAvataEmp,
        };

        return AttendanceLogServiceFactory;

        //Tìm kiếm
        function getListAttendanceLog(model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/AttendanceLog/GetListAttendanceLog';
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

        function getListNoAttendanceLog(model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NoAttendanceLog/GetListNoAttendanceLog';
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


        function getAttendanceLogInfo(model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/AttendanceLog/GetAttendanceLogInfo';
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

        function exportExcel(model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/AttendanceLog/ExportExcel';
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

        function updateAttendanceLog(model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/AttendanceLog/UpdateAttendanceLog';
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

        function deleteAttendanceLog(id) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/AttendanceLog/DeleteAttendanceLog?attendanceLogId=' + id;
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

        function getListEmployee(id) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/AttendanceLog/GetListEmployee?departmentId=' + id;
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

        function getListDepartment() {
            var deferred = $q.defer();
            var url = serviceBase + 'api/AttendanceLog/GetListDepartment';
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

        function training(model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/AttendanceLog/Training';
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

        function getAvataEmp(AttendanceLogId) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/AttendanceLog/GetAvataEmp?AttendanceLogId=' + AttendanceLogId;
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