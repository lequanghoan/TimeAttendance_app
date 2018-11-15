// <copyright company="nhantinsoft.vn">
// Author: NTS-TUNGNT
// Created Date: 06/03/2017
// </copyright>
'use strict';
app.controller('AttendanceLogUpdateCtrl', ['$scope', 'AttendanceLogService', '$uibModal', '$uibModalInstance', 'message', 'items', 'authService', 'ngAuthSettings', 'notify',
  function ($scope, AttendanceLogService, $uibModal, $uibModalInstance, message, items, authService, ngAuthSettings, notify) {
      var vm = this;
      vm.serviceBase = ngAuthSettings.apiServiceBaseUri;
      vm.FnSave = fnUpdateAttendanceLog;
      vm.FnClose = fnCloseModal;
      vm.FnChange = fnChangeDepartment;
      vm.ListEmployee = [];
      vm.ListDepartment = [];
      vm.Model = {
          AttendanceLogId: '',
          DepartmentId: '',
          EmployeeId: '',
          EmployeeOldId:''
      }

      fnInitPage();

      // Khởi tạo trang
      function fnInitPage() {
          fnGetListDepartment();
          fnGetAttendanceLogInfo();
      }

      // Cập nhật
      function fnUpdateAttendanceLog() {
          vm.Model.UpdateBy = authService.authentication.id;
          AttendanceLogService.UpdateAttendanceLog(vm.Model).then(function () {
              notify('Cập nhật thông tin thành công!');
              fnCloseModal();
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      // Hiển thị thông tin
      function fnGetAttendanceLogInfo() {
          vm.Model.AttendanceLogId = items.attendanceLogId;
          AttendanceLogService.GetAttendanceLogInfo(vm.Model).then(function (data) {
              vm.Model = data.AtendanceLogSearchResult;
              vm.Model.EmployeeOldId = data.AtendanceLogSearchResult.EmployeeId;
              vm.ListEmployee = data.ListEmployee;
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      function fnChangeDepartment() {
          AttendanceLogService.GetListEmployee(vm.Model.DepartmentId).then(function (data) {
              vm.ListEmployee = data;
          }, function (error) {
              message.ShowMessage(error.message, 1);
          })
      }

      function fnGetListDepartment() {
          AttendanceLogService.GetListDepartment().then(function (data) {
              vm.ListDepartment = data;
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
