/**
* <copyright company="nhantinsoft.vn">
* Author: Vũ Văn Văn
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
app.controller('Department001Ctrl', ['$scope', '$http', 'DepartmentService', '$uibModal', 'message', 'notify', 'common', 'authService',
  function ($scope, $http, DepartmentService, $uibModal, message, notify, common, authService) {
      var vm = this;
      vm.ListData = [];
      vm.InfoLogin = authService.authentication;

      //Gán sự kiện
      vm.FnSearch = fnSearchDepartment;
      vm.FnClear = fnClearData;
      vm.FnDelete = fnConfirmDeleteDepartment;
      vm.FnShowModal = fnShowModal;

      fnInitPage();

      //Khởi tạo trang
      function fnInitPage() {
          fnInitialModel();
          fnSearchDepartment();
      }
      //Khởi tạo model
      function fnInitialModel() {
          vm.Model = {
              Name: '',
          };
      }

      //Tìm kiếm
      function fnSearchDepartment() {

          DepartmentService.GetListDepartment(vm.Model).then(function (data) {
              vm.ListData = data;
              vm.TotalItems = vm.ListData.length;
          }, function (error) {
              vm.ListData = [];
              message.ShowMessage(error.message, 1);
          });
      }

      //Xóa
      function fnDeleteDepartment(id) {
          vm.Model.UserId = vm.InfoLogin.userid;

          DepartmentService.DeleteDepartment(id).then(function (data) {
              if (data == 4) {
                  notify('Phòng ban này đang được sử dụng trong hệ thống, không được xóa!')
              }
              else {
                  notify('Xóa phòng ban thành công!');
              }
              fnSearchDepartment();
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      //Xác nhân xóa
      function fnConfirmDeleteDepartment(id) {
          var modalInstance = $uibModal.open({
              templateUrl: 'app/shared/modalConfirmWithTitle/view.html',
              controller: 'ConfirmWithTitleCtrl',
              controllerAs: 'vmPopup',
              resolve: {
                  deps: ['uiLoad',
                                  function (uiLoad) {
                                      return uiLoad.load('app/shared/modalConfirmWithTitle/controller.js');
                                  }
                  ], items: function () {
                      return { Title: "Bạn có chắc chắn muốn xóa phòng ban này không?" };
                  }
              }
          });
          modalInstance.result.then(function (rs) {
              if (rs) {
                  fnDeleteDepartment(id);
              }
          }, function () {
          });
      }

      function fnShowModal(id) {
          var modalInstance = $uibModal.open({
              templateUrl: 'app/views/Department/Department002.html',
              controller: 'Department002Ctrl',
              controllerAs: 'vm',
              resolve: {
                  deps: ['$ocLazyLoad',
                      function ($ocLazyLoad) {
                          return $ocLazyLoad.load([
                              'app/controllers/Department/Department002Controller.js',
                              'app/services/Department/DepartmentService.js'
                          ]);
                      }
                  ], items: function () {
                      return { id: id };
                  }
              }
          });
          modalInstance.result.then(function (rs) {
              if (rs == 1) {
                  fnSearchDepartment();
              }
              else if (rs == 2) {
                  notify('Không tìm thấy phòng ban này!');
              }
          }, function () {
          });
      }

      //Làm mới tìm kiếm
      function fnClearData() {
          fnInitialModel();
          fnSearchDepartment();
      }

  }
]);