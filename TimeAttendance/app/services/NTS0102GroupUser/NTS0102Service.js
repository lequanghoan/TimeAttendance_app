/**
* <copyright company="nhantinsoft.vn">
* Author: Vũ Văn Văn
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
(function () {
    angular.module('app').factory('NTS0102Service', NTS0102Service);
    NTS0102Service.$inject = ['$http', '$q', 'ngAuthSettings'];

    function NTS0102Service($http, $q, ngAuthSettings) {
        var serviceBase = ngAuthSettings.apiServiceBaseUri;
        var NTS0102ServiceFactory = {
            SearchGroupUser: searchGroupUser,
            DeleteGroupUser: deleteGroupUser,
            UpdateStatusGroup: updateStatusGroup,
            CreateGroupUser: createGroupUser,
            UpdateGroupUser: updateGroupUser,
            GetGroupUserById: getGroupUserById,
            GetFunction: getFunction,
            ListMemberGroup: listMemberGroup
        };

        return NTS0102ServiceFactory;

        //Tìm kiếm
        function searchGroupUser(model, pageSize, pageNumber) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0102/SearchGroupUser?' + 'pageSize=' + pageSize + '&pageNumber=' + pageNumber;
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
        function deleteGroupUser(id, model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0102/DeleteGroupUser?id=' + id;
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

        //Cập nhật trạng thái
        function updateStatusGroup(id, model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0102/UpdateStatusGroup?id=' + id;
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
        function createGroupUser(model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0102/CreateGroupUser';
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

        //Cập nhật thông tin
        function updateGroupUser(model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0102/UpdateGroupUser';
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
        function getGroupUserById(id) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0102/GetGroupUserById?id=' + id;
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
		
        //Danh sách quyền
        function getFunction() {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0102/GetFunction';
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

        //Danh sách thành viên trong nhóm
        function listMemberGroup(id) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0102/ListMemberGroup?id=' + id;
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