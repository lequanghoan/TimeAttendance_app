/**
* <copyright company="nhantinsoft.vn">
* Author: Hiepdv
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
app.controller('NTS0304ModelCtrl', ['$scope', '$http', '$uibModalInstance', 'items', 'message', '$state', 'authService', 'notify', 'common', 'ngAuthSettings',
  function ($scope, $http, $uibModalInstance, items, message, $state, authService, notify, common, ngAuthSettings) {
      var vm = this;
      $scope.Image = items.Image;
      $scope.EmployeeCode = items.EmployeeCode;
      $scope.EmployeeName = items.EmployeeName;
      $scope.Time = items.Time;
      $scope.JobTitleName = items.JobTitleName;
      
      $scope.ServiceBase = ngAuthSettings.apiServiceBaseUri;
      $scope.Common = common;
      vm.InfoLogin = authService.authentication;
      $scope.obj = {};

      fnInitPage();
   
      vm.FnClose = fnCloseModal;
      //Khởi tạo trang
      function fnInitPage() {
        
      }
      function fnCloseModal() {
          $uibModalInstance.close(true);
      }
  }
]);