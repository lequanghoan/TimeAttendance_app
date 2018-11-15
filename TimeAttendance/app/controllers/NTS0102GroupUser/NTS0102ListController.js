/**
* <copyright company="nhantinsoft.vn">
* Author: Vũ Văn Văn
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
app.controller('NTS0102ListCtrl', ['$scope', '$http', 'NTS0102Service', '$uibModal', 'message', 'notify', 'common', 'authService',
  function ($scope, $http, NTS0102Service, $uibModal, message, notify, common, authService) {
      var vm = this;
      $scope.Common = common;
      vm.ListStatus = common.ListStatus;
      vm.ListData = [];
      vm.InfoLogin = authService.authentication;
      vm.ListPageSize = [];
      fnInitPage();

      //Gán sự kiện
      vm.FnSearch = fnSearchGroupUser;
      vm.FnClear = fnClearData;
      vm.FnDelete = fnConfirmDeleteGroupUser;
      vm.FnUpdateStatusGroup = fnConfirmUpdateStatusGroup;
      vm.FnShowModalMember = fnShowModalMember;

      //Khởi tạo trang
      function fnInitPage() {
          fnInitialModel();
          InitPageSize();
          fnSearchGroupUser();
      }
      function InitPageSize() {
          vm.ListPageSize.push({ Id: 10, Name: '10 bản ghi' });
          vm.ListPageSize.push({ Id: 15, Name: '15 bản ghi' });
          vm.ListPageSize.push({ Id: 20, Name: '20 bản ghi' });
          vm.ListPageSize.push({ Id: 30, Name: '30 bản ghi' });
          vm.ListPageSize.push({ Id: 40, Name: '40 bản ghi' });
          vm.ListPageSize.push({ Id: 50, Name: '50 bản ghi' });

      }
      //Khởi tạo model
      function fnInitialModel() {
          //Khởi thông số phân trang
          $scope.totalItems = 0;
          $scope.currentPage = 1;
          $scope.numPerPage = common.NumPerPage;
          $scope.maxSize = common.MaxSize;

          vm.Model = {
              Name: '',
              Status: '',
              Description: '',
              PageSize: $scope.numPerPage,
              PageNumber: $scope.currentPage,
              OrderBy: 'Name',
              OrderType: true
          };
          vm.Model.UserId = vm.InfoLogin.userid;
      }

      //Tìm kiếm
      function fnSearchGroupUser(objectorder) {
          vm.Model.PageNumber = $scope.currentPage;
          if (objectorder != null) {
              vm.Model.OrderBy = objectorder.by;
              vm.Model.OrderType = objectorder.type;
          }

          NTS0102Service.SearchGroupUser(vm.Model, vm.Model.PageSize, $scope.currentPage).then(function (data) {
              vm.ListData = data.ListResult;
              $scope.totalItems = data.TotalItem;

              //Tính index bắt đầu
              $scope.StartIndex = (($scope.currentPage - 1) * vm.Model.PageSize);

          }, function (error) {
              vm.ListData = [];
              message.ShowMessage(error.message, 1);
          });
      }

      //Xóa
      function fnDeleteGroupUser(id) {
          vm.Model.UserId = vm.InfoLogin.userid;

          NTS0102Service.DeleteGroupUser(id, vm.Model).then(function () {
              notify('Xóa nhóm quyền thành công!');
              fnSearchGroupUser();
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      //Xác nhân xóa
      function fnConfirmDeleteGroupUser(id) {
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
                      return { Title: "Bạn có chắc chắn muốn xóa nhóm quyền này không?" };
                  }
              }
          });
          modalInstance.result.then(function (rs) {
              if (rs) {
                  fnDeleteGroupUser(id);
              }
          }, function () {
          });
      }

      //Làm mới tìm kiếm
      function fnClearData() {
          fnInitialModel();
          fnSearchGroupUser();
      }

      //Cập nhật trạng thái
      function fnUpdateStatusGroup(id) {
          vm.Model.UserId = vm.InfoLogin.userid;

          NTS0102Service.UpdateStatusGroup(id, vm.Model).then(function () {
              notify('Thay đổi trạng thái nhóm quyền thành công!');
              fnSearchGroupUser();
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      //Xác thay dổi trạng thái
      function fnConfirmUpdateStatusGroup(id, status) {
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
                      return { Title: "Bạn có chắc chắn muốn " + (status == common.UnLock ? "khóa" : "kích hoạt") + " nhóm quyền này không?" };
                  }
              }
          });
          modalInstance.result.then(function (rs) {
              if (rs) {
                  fnUpdateStatusGroup(id);
              }
          }, function () {
          });
      }

      //Hiển thị dánh sách thành viên trong nhóm
      function fnShowModalMember(id, name) {
          var modalInstance = $uibModal.open({
              templateUrl: 'app/views/NTS0102GroupUser/NTS0102Member.html',
              controller: 'NTS0102MemberCtrl',
              controllerAs: 'vm',
              windowClass: 'app-modal',
              resolve: {
                  deps: ['$ocLazyLoad',
                      function ($ocLazyLoad) {
                          return $ocLazyLoad.load([
                              'app/controllers/NTS0102GroupUser/NTS0102MemberController.js',
                              'app/services/NTS0102GroupUser/NTS0102Service.js',
                          ]);
                      }
                  ], items: function () {
                      return { id: id, name: name };
                  }
              }
          });
          modalInstance.result.then(function (rs) {
              if (rs)
                  fnSearchMemberGroup();
          }, function () {
          });
      }
  }
]);