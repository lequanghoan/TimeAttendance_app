// <copyright company="nhantinsoft.vn">
// Author: NTS-TUNGNT
// Created Date: 06/03/2017
// </copyright>
'use strict';
app.controller('JobTitle002Ctrl', ['$scope', 'JobTitleService', '$uibModal', '$uibModalInstance', 'message', 'items', 'authService', 'ngAuthSettings', 'notify',
  function ($scope, JobTitleService, $uibModal, $uibModalInstance, message, items, authService, ngAuthSettings, notify) {
      var vm = this;
      vm.serviceBase = ngAuthSettings.apiServiceBaseUri;
      vm.FnSave = fnSave;
      vm.FnClear = fnClearData;
      vm.FnClose = fnCloseModal;

      fnInitPage();

      // Khởi tạo trang
      function fnInitPage() {
          fnInitialObject();
      }

      // Khởi tạo model
      function fnInitialObject() {
          vm.Model = {
              Name: '',
              Description: '',
              CreateBy: '',
              UpdateBy: ''
          };
          fnGetJobTitleInfo();
      }

      // Lưu thông tin người dùng
      function fnSave() {
          if (items.id != undefined) {
              fnUpdateJobTitle();
              vm.ModalTitle = "Cập nhật thông tin phòng ban";
          }
          else {
              fnAddJobTitle();
          }
      }

      // Thêm mới và trở về danh sách
      function fnAddJobTitle() {
          vm.Model.CreateBy = authService.authentication.id;
          JobTitleService.AddJobTitle(vm.Model).then(function () {
              notify('Thêm mới chức danh thành công!');
              fnCloseModal();
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      // Cập nhật
      function fnUpdateJobTitle() {
          vm.Model.UpdateBy = authService.authentication.id;
          JobTitleService.UpdateJobTitle(vm.Model).then(function () {
              notify('Cập nhật chức danh thành công!');
              fnCloseModal();
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      // Hiển thị thông tin
      function fnGetJobTitleInfo() {

          // Kiểm tra hiển thị thêm mới hoặc sửa
          if (items.id != undefined) {
              vm.ModalTitle = "Cập nhật thông tin chức danh";
              vm.SaveText = "Cập nhật";
              vm.ShowClear = false;

              vm.ShowEdit = true;
              vm.StatusEdit = false;
              JobTitleService.GetJobTitleInfo(items.id).then(function (data) {
                  vm.Model = data;
              }, function (error) {
                  message.ShowMessage(error.message, 1);
              });
          }
          else {
              vm.ModalTitle = "Thêm mới thông tin chức danh";
              vm.SaveText = "Lưu";
              vm.ShowClear = true;

              vm.ShowEdit = false;
              vm.StatusEdit = true;
          }
      }

      // Xóa thông tin trên giao diện
      function fnClearData() {
          //Khỏi tạo lại model
          fnInitialObject();
      }

      // Đóng model
      function fnCloseModal() {
          $uibModalInstance.close(true);
      }

      // Xác nhận sửa
      function fnEdit() {
          vm.ShowEdit = false;
          vm.StatusEdit = true;
      }
  }
]);
