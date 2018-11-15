
// <copyright company="nhantinsoft.vn">
// Author: NTS-TUNGNT
// Created Date: 06/03/2017
// </copyright>
'use strict';
app.controller('AttendanceLogTrainingCtrl', ['$scope', 'AttendanceLogService', '$uibModal', '$uibModalInstance', 'message', 'items', 'authService', 'ngAuthSettings', 'notify',
  function ($scope, AttendanceLogService, $uibModal, $uibModalInstance, message, items, authService, ngAuthSettings, notify) {
      var vm = this;
      vm.serviceBase = items.sv;// ngAuthSettings.apiServiceBaseUri;
      vm.FnClose = fnCloseModal;
      vm.FnTraining = fnTraining;

      vm.ListResult = [];
      vm.ListResultSelect = [];

      //Gán sự kiện
      fnInitPage();

      //Khởi tạo trang
      function fnInitPage() {
          fnInitialModel();
      }
      function fnTraining() {
          vm.ListResultSelect = [];
          vm.ListResultSelect.push(items.row);
          if (vm.ListResultSelect.length == 0) {
              notify('Bạn chưa chọn bản ghi nào!'); return false;
          } else {
              AttendanceLogService.Training(vm.ListResultSelect).then(function (data) {
                  if (data == '0') {
                      notify('Training ảnh thành công!');
                      fnCloseModal();
                  } else {
                      notify('Đã xảy ra lỗi trong quá trình training ảnh!');
                  }
              }, function (error) {
                  message.ShowMessage(error.message, 1);
              });
          }
      }
      //Khởi tạo model
      function fnInitialModel() {

          vm.Model = null;
          vm.Model =angular.copy(items.row);
          fnGetAvataEmp();
      }


      function fnGetAvataEmp() {
          AttendanceLogService.GetAvataEmp(vm.Model.AttendanceLogId).then(function (data) {
              vm.Model.Avata = data;
          }, function (error) {
              message.ShowMessage(error.message, 1);
          })
      }

      // Đóng model
      function fnCloseModal() {
          $uibModalInstance.close(true);
      }
  }
]);


