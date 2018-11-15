/**
* <copyright company="nhantinsoft.vn">
* Author: Lưu Đức Thành
* Created Date: 28/11/2016 10:36
* </copyright>
*/
'use strict';
(function () {
    angular.module('app').factory('DateUtilService', DateUtilService);
    DateUtilService.$inject = ['$http', '$q', 'ngAuthSettings'];

    function DateUtilService($http, $q, ngAuthSettings) {
        var DateUtilService = {
            convertToDate: convertToDate,
            convertStringDate: convertStringDate,
            getCurrentStringDate: getCurrentStringDate,
            getCurrentStringTime: getCurrentStringTime,
            getCurrentDate: getCurrentDate
        };

        return DateUtilService;

        //Tìm kiếm
        function convertToDate(date) {
            return new Date(date.substring(0, 4), parseInt(date.substring(5, 7), 10) - 1, date.substring(8, 10));
        }

        function convertStringDate(stringDate) {
            return stringDate.substring(0, 10);
        }

        function getCurrentStringDate() {
            var date = new Date();
            var year = date.getFullYear(),
                month = (date.getMonth() + 1) < 10 ? '0' + (date.getMonth() + 1) : (date.getMonth() + 1),
                day = date.getDate() < 10 ? '0' + date.getDate() : date.getDate();
            return year + '-' + month + '-' + day;
        }

        function getCurrentDate() {
            var date = new Date();
            var year = date.getFullYear(),
                month = (date.getMonth() + 1) < 10 ? '0' + date.getMonth() : date.getMonth(),
                day = date.getDate() < 10 ? '0' + date.getDate() : date.getDate();
            return new Date(year, month, day);
        }

        function getCurrentStringTime() {
            var date = new Date();
            var HH = date.getHours() < 10 ? '0' + date.getHours() : date.getHours(),
                mm = date.getMinutes() < 10 ? '0' + date.getMinutes() : date.getMinutes(),
                ss = date.getSeconds() < 10 ? '0' + date.getSeconds() : date.getSeconds();
            return HH + ":" + mm + ":" + ss;
        }
    }
})();