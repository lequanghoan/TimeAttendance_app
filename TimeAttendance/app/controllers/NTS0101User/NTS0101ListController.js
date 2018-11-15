/**
* <copyright company="nhantinsoft.vn">
* Author: Vũ Văn Văn
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
app.controller('NTS0101ListCtrl', ['$scope', '$http', 'NTS0101Service', '$uibModal', 'message', 'notify', 'common', 'authService', 'ngAuthSettings',
  function ($scope, $http, NTS0101Service, $uibModal, message, notify, common, authService, ngAuthSettings) {
      var vm = this;
      $scope.ServiceBase = ngAuthSettings.apiServiceBaseUri;
      vm.InfoLogin = authService.authentication;
      $scope.Common = common;
      vm.ListStatus = common.ListStatus;
      vm.ListUserRole = [];
      vm.ListGroup = [];
      vm.ListUnit = [];
      vm.ListResult = [];
      vm.ListPageSize = [];
      fnInitPage();

      //Gán sự kiện
      vm.FnSearch = fnSearchUser;
      vm.FnClear = fnClearData;
      vm.FnDelete = fnConfirmDeleteUser;
      vm.FnUpdateStatusUser = fnConfirmUpdateStatusGroup;
      vm.FnResetPass = fnResetPass;

      //Khởi tạo trang
      function fnInitPage() {
          fnInitialModel();
          InitPageSize();
          fnGetSelectGroup();
          fnSearchUser();
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
              Role: '',
              Phone: '',
              Name: '',
              FullName: '',
              GroupId: '',
              Status: '',
              UnitId: '',
              Type: common.AccountCompany,
              LogUserId: vm.InfoLogin.userid,
              PageSize: $scope.numPerPage,
              PageNumber: $scope.currentPage,
              OrderBy: 'Name',
              OrderType: true
          };

          vm.Model.LogUserId = vm.InfoLogin.userid;
          //fnGetSelectUnit();
      }

      //Tìm kiếm
      function fnSearchUser(objectorder) {
          vm.Model.PageNumber = $scope.currentPage;
          if (objectorder != null) {
              vm.Model.OrderBy = objectorder.by;
              vm.Model.OrderType = objectorder.type;
          }
          NTS0101Service.SearchUser(vm.Model).then(function (data) {
              vm.ListResult = data.ListResult;
              $scope.totalItems = data.TotalItem;

              //Tính index bắt đầu
              $scope.StartIndex = (($scope.currentPage - 1) * vm.Model.PageSize);

              setTimeout(function () {
                  $('.scroller').perfectScrollbar('update');
              }, 100);
          }, function (error) {
              vm.ListResult = [];
              message.ShowMessage(error.message, 1);
          });
      }

      //Xóa
      function fnDeleteUser(id) {
          vm.Model.UserId = vm.InfoLogin.userid;
          NTS0101Service.DeleteUser(id, vm.Model).then(function (data) {
              notify('Xóa tài khoản thành công!');
              fnSearchUser();
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      //Xác nhân xóa
      function fnConfirmDeleteUser(id) {
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
                      return { Title: "Bạn có chắc chắn muốn xóa tài khoản này không?" };
                  }
              }
          });
          modalInstance.result.then(function (rs) {
              if (rs) {
                  fnDeleteUser(id);
              }
          }, function () {
          });
      }

      //Làm mới tìm kiếm
      function fnClearData() {
          fnInitialModel();
          fnSearchUser();
      }

      //Lấy thông tin combobox nhóm tài khoản
      function fnGetSelectGroup() {
          NTS0101Service.GetSelectGroupAll().then(function (data) {
              vm.ListGroup = data;
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      //Cập nhật trạng thái
      function fnUpdateStatusUser(id) {
          vm.Model.UserId = vm.InfoLogin.userid;

          NTS0101Service.UpdateStatusUser(id, vm.Model).then(function () {
              notify('Cập nhật trạng thái tài khoản thành công!');
              fnSearchUser();
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      //Xác nhân xóa
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
                      return { Title: "Bạn có chắc chắn muốn " + (status == common.UnLock ? "khóa" : "kích hoạt") + " tài khoản này không?" };
                  }
              }
          });
          modalInstance.result.then(function (rs) {
              if (rs) {
                  fnUpdateStatusUser(id);
              }
          }, function () {
          });
      }

      //Reset mật khẩu
      function fnResetPass(id) {
          vm.Model.UserId = vm.InfoLogin.userid;

          NTS0101Service.ResetPass(id, vm.Model).then(function () {
              notify("Đặt lại mật khẩu về mật khẩu mặc định cho tài khoản thành công!");
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      //Lấy thông tin combobox
      function fnGetSelectUnit() {
          NTS0101Service.GetSelectUnit().then(function (data) {
              vm.ListUnit = data;
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }
  }
]);