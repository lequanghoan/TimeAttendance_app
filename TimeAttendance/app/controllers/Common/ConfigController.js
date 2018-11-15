/**
* <copyright company="nhantinsoft.vn">
* Author: Hiepdv
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
app.controller('ConfigCtrl', ['$scope', '$http', '$uibModalInstance', 'ConfigService', 'message', '$state', 'authService', 'notify', 'common',
  function ($scope, $http,$uibModalInstance, ConfigService, message, $state, authService, notify, common) {
      var vm = this;
      $scope.Common = common;
      vm.InfoLogin = authService.authentication;
      $scope.obj = {};

      fnInitPage();
      vm.FnUpdate = fnUpdate;
      vm.FnClose = fnCloseModal;
      //Khởi tạo trang
      function fnInitPage() {
          fnInitialModel();
          fnGetConfig();
      }

      //Khởi tạo model
      function fnInitialModel() {
          vm.Model = {
              TimeIn: '',
              TimeOut: '',
              Percent: 0,
              TimeAttendanceLog:0,
          };
      }

    

      //Thêm mới và trở về danh sách
      function fnUpdate() {
          var ok = false;
          var checkIn = vm.Model.TimeIn.split(':');
          var checkOut = vm.Model.TimeOut.split(':');
          if (parseInt(checkIn[0])>23) {
              notify('Giờ vào không được lớn hơn 23 giờ');
          } else if (parseInt(checkIn[1]) > 59) {
              notify('Phút vào không được lớn hơn 59 phút');
          }
          else if (parseInt(checkOut[0]) > 23) {
              notify('Giờ ra không được lớn hơn 23 giờ');
          } else if (parseInt(checkOut[1]) > 59) {
              notify('Phút ra không được lớn hơn 59 phút');
          }
          else if (vm.Model.Percent>100 ) {
              notify('Ngưỡng chính xác không được lớn hơn 100%');
          } else {
              ok = true;;
          }
          if (ok==false) {
              return;
          }
          ConfigService.UpdateConfig(vm.Model).then(function () {
              notify('Cập nhật cấu hình hệ thống thành công');
              fnCloseModal();
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      function fnCloseModal() {
          $uibModalInstance.close(false);
      }
 
      function fnGetConfig() {
          ConfigService.GetConfig().then(function (data) {
              vm.Model = data;
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

  }
]);