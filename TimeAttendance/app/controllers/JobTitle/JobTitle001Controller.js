/**
* <copyright company="nhantinsoft.vn">
* Author: Vũ Văn Văn
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
app.controller('JobTitle001Ctrl', ['$scope', '$http', 'JobTitleService', '$uibModal', 'message', 'notify', 'common', 'authService',
  function ($scope, $http, JobTitleService, $uibModal, message, notify, common, authService) {
      var vm = this;
      vm.ListData = [];
      vm.InfoLogin = authService.authentication;

      //Gán sự kiện
      vm.FnSearch = fnSearchJobTitle;
      vm.FnClear = fnClearData;
      vm.FnDelete = fnConfirmDeleteJobTitle;
      vm.FnShowModal = fnShowModal;

      fnInitPage();

      //Khởi tạo trang
      function fnInitPage() {
          fnInitialModel();
          fnSearchJobTitle();
      }
      //Khởi tạo model
      function fnInitialModel() {
          vm.Model = {
              Name: '',
          };
      }

      //Tìm kiếm
      function fnSearchJobTitle() {

          JobTitleService.GetListJobTitle(vm.Model).then(function (data) {
              vm.ListData = data;
              vm.TotalItems = vm.ListData.length;
          }, function (error) {
              vm.ListData = [];
              message.ShowMessage(error.message, 1);
          });
      }

      //Xóa
      function fnDeleteJobTitle(id) {
          vm.Model.UserId = vm.InfoLogin.userid;

          JobTitleService.DeleteJobTitle(id).then(function (data) {
              if (data == 4) {
                  notify('Chức danh này đang được sử dụng trong hệ thống, không được xóa!')
              }
              else {
                  notify('Xóa chức danh thành công!');
              }
              fnSearchJobTitle();
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      //Xác nhân xóa
      function fnConfirmDeleteJobTitle(id) {
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
                      return { Title: "Bạn có chắc chắn muốn xóa chức danh này không?" };
                  }
              }
          });
          modalInstance.result.then(function (rs) {
              if (rs) {
                  fnDeleteJobTitle(id);
              }
          }, function () {
          });
      }

      function fnShowModal(id) {
          var modalInstance = $uibModal.open({
              templateUrl: 'app/views/JobTitle/JobTitle002.html',
              controller: 'JobTitle002Ctrl',
              controllerAs: 'vm',
              resolve: {
                  deps: ['$ocLazyLoad',
                      function ($ocLazyLoad) {
                          return $ocLazyLoad.load([
                              'app/controllers/JobTitle/JobTitle002Controller.js',
                              'app/services/JobTitle/JobTitle001Service.js'
                          ]);
                      }
                  ], items: function () {
                      return { id: id };
                  }
              }
          });
          modalInstance.result.then(function (rs) {
              if (rs == 1) {
                  fnSearchJobTitle();
              }
              else if (rs == 2) {
                  notify('Không tìm thấy chức danh này!');
              }
          }, function () {
          });
      }

      //Làm mới tìm kiếm
      function fnClearData() {
          fnInitialModel();
          fnSearchJobTitle();
      }

  }
]);