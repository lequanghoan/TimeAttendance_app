/**
* <copyright company="nhantinsoft.vn">
* Author: Vũ Văn Văn
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
(function () {
    angular.module('app').factory('NTS0101Service', NTS0101Service);
    NTS0101Service.$inject = ['$http', '$q', 'ngAuthSettings'];

    function NTS0101Service($http, $q, ngAuthSettings) {
        var serviceBase = ngAuthSettings.apiServiceBaseUri;
        var NTS0101ServiceFactory = {
            SearchUser: searchUser,
            DeleteUser: deleteUser,
            GetSelectUserRole: getSelectUserRole,
            GetSelectGroup: getSelectGroup,
            GetSelectGroupAll: getSelectGroupAll,
            UpdateStatusUser: updateStatusUser,
            ResetPass: resetPass,
            GetSelectUnit: getSelectUnit,
            CreateUser: createUser,
            GetFunction: getFunction,
            GetFunctionByUser: getFunctionByUser,
            UpdateUser: updateUser,
            GetUserById: getUserById,
        };

        return NTS0101ServiceFactory;

        //Tìm kiếm
        function searchUser(model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0101/SearchUser';
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
        function createUser(file, model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0101/CreateUser';
            var fd = new FormData();
            if (file != null && file.length > 0) {
                fd.append('File', file[0].file);
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
        function updateUser(file, model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0101/UpdateUser';
            var fd = new FormData();
            if (file != null && file.length > 0) {
                fd.append('File', file[0].file);
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
        function deleteUser(id, model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0101/DeleteUser?id=' + id;
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

        //Lấy thông tin theo Id
        function getUserById(id) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0101/GetUserById?id=' + id;
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
        function getSelectUserRole() {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0101/GetSelectUserRole';
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
        function getSelectGroupAll() {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0101/GetSelectGroupAll';
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

        ///Nhóm quyền người dùng đang hoạt động
        function getSelectGroup() {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0101/GetSelectGroup';
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
        function updateStatusUser(id, model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0101/UpdateStatusUser?id=' + id;
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

        //Reset mật khẩu
        function resetPass(id, model) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0101/ResetPass?id=' + id;
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

        //Lấy thông tin combobox
        function getSelectUnit() {
            var deferred = $q.defer();
            var url = serviceBase + 'api/ComboboxCommon/GetAllUnit';
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

        //Danh sách quyền the nhóm người dùng
        function getFunction(id) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0101/GetFunction?id=' + id;
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

        //Danh sách quyền theo người dùng
        function getFunctionByUser(id) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/NTS0101/GetFunctionByUser?id=' + id;
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