/**
* <copyright company="nhantinsoft.vn">
* Author: Vũ Văn Văn
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
app.controller('NTS0201ListCtrl', ['$scope', '$http', 'NTS0201Service', '$uibModal', 'message', 'notify', 'common', 'authService', 'ngAuthSettings',
  function ($scope, $http, NTS0201Service, $uibModal, message, notify, common, authService, ngAuthSettings) {
      var vm = this;
      $scope.ServiceBase = ngAuthSettings.apiServiceBaseUri;
      vm.InfoLogin = authService.authentication;
      $scope.Common = common;
      vm.ListStatus = common.ListStatus;
      vm.ListDepartment = [];
      vm.ListJobTitle = [];
      vm.ListResult = [];
      vm.ListPageSize = [];
      fnInitPage();

      //Gán sự kiện
      vm.FnSearch = fnSearchEmployee;
      vm.FnClear = fnClearData;
      vm.FnDelete = fnConfirmDelete;
      vm.FnShowCreate = fnShowModalCreate;
      vm.FnShowUpdate = fnShowModalUpdate;
      vm.FnShowImage = fnShowImage;
      //Khởi tạo trang
      function fnInitPage() {
          fnInitialModel();
          InitPageSize();
          fnGetAllJobTitle();
          fnGetAllDepartment();
          fnSearchEmployee();
      }
      function InitPageSize() {
          vm.ListPageSize.push({ Id: 10, Name: '10 bản ghi' });
          vm.ListPageSize.push({ Id: 15, Name: '15 bản ghi' });
          vm.ListPageSize.push({ Id: 20, Name: '20 bản ghi' });
          vm.ListPageSize.push({ Id: 30, Name: '30 bản ghi' });
          vm.ListPageSize.push({ Id: 40, Name: '40 bản ghi' });
          vm.ListPageSize.push({ Id: 50, Name: '50 bản ghi' });

      }

      //Show modal img
      function fnShowImage(Image, EmployeeCode, EmployeeName, Time,JobTitleName) {
          var modalInstance = $uibModal.open({
              templateUrl: 'app/views/NTS0301Report/NTS0304Model.html',
              controller: 'NTS0304ModelCtrl',
              controllerAs: 'vm',
              windowClass: 'app-modal',
              resolve: {
                  deps: ['$ocLazyLoad',
                      function ($ocLazyLoad) {
                          return $ocLazyLoad.load([
                              'app/controllers/NTS0301Report/NTS0304ModelController.js',
                          ]);
                      }
                  ], items: function () {
                      return { Image: Image, EmployeeCode: EmployeeCode, EmployeeName: EmployeeName, Time: Time, JobTitleName: JobTitleName };
                  }
              }
          });
          modalInstance.result.then(function (rs) {
              if (rs)
                  fnSearchEmployee();
          }, function () {
          });
      }
      //Khởi tạo model
      function fnInitialModel() {
          //Khởi thông số phân trang
          $scope.totalItems = 0;
          $scope.currentPage = 1;
          $scope.numPerPage = common.NumPerPage;
          $scope.maxSize = common.MaxSize;

          vm.Model = {
              Code: '',
              Name: '',
              DepartmentId: '',
              JobTitleId: '',
              DateFrom: '',
              DateTo: '',
              Gender: '-1',
              IdentifyCardNumber:'',
              CreateBy: vm.InfoLogin.userid,
              PageSize: $scope.numPerPage,
              PageNumber: $scope.currentPage,
              OrderBy: 'Name',
              OrderType: true
          };

      }

      //Tìm kiếm
      function fnSearchEmployee(objectorder) {
          vm.Model.PageNumber = $scope.currentPage;
          if (objectorder != null) {
              vm.Model.OrderBy = objectorder.by;
              vm.Model.OrderType = objectorder.type;
          }

          NTS0201Service.SearchEmployee(vm.Model).then(function (data) {
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
      function fnDelete(id) {
          NTS0201Service.DeleteEmployee(id, vm.Model.CreateBy).then(function (data) {
              notify('Xóa nhân viên thành công!');
              fnSearchEmployee();
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      //Xác nhân xóa
      function fnConfirmDelete(id) {
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
                      return { Title: "Bạn có chắc chắn muốn xóa nhân viên này không?" };
                  }
              }
          });
          modalInstance.result.then(function (rs) {
              if (rs) {
                  fnDelete(id);
              }
          }, function () {
          });
      }

      //Làm mới tìm kiếm
      function fnClearData() {
          fnInitialModel();
          fnSearchEmployee();
      }

      //Lấy thông tin combobox 
      function fnGetAllJobTitle() {
          NTS0201Service.GetAllJobTitle().then(function (data) {
              vm.ListJobTitle = data;
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }
      function fnGetAllDepartment() {
          NTS0201Service.GetAllDepartment().then(function (data) {
              vm.ListDepartment = data;
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      //Cập nhật trạng thái
      function fnUpdateStatusUser(id) {
          vm.Model.UserId = vm.InfoLogin.userid;

          NTS0201Service.UpdateStatusUser(id, vm.Model).then(function () {
              notify('Cập nhật trạng thái tài khoản thành công!');
              fnSearchUser();
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      //Show modal thêm mới
      function fnShowModalCreate() {
          var modalInstance = $uibModal.open({
              templateUrl: 'app/views/NTS0201Employee/NTS0201Create.html',
              controller: 'NTS0201CreateCtrl',
              controllerAs: 'vm',
              windowClass: 'app-modal',
              resolve: {
                  deps: ['$ocLazyLoad',
                      function ($ocLazyLoad) {
                          return $ocLazyLoad.load([
                              'app/controllers/NTS0201Employee/NTS0201CreateController.js',
                              'app/services/NTS0201Employee/NTS0201Service.js',
                          ]);
                      }
                  ], items: function () {
                      return {};
                  }
              }
          });
          modalInstance.result.then(function (rs) {
              if (rs)
                  fnSearchEmployee();
          }, function () {
          });
      }

      //Show modal cập nhật
      function fnShowModalUpdate(id) {
          var modalInstance = $uibModal.open({
              templateUrl: 'app/views/NTS0201Employee/NTS0201Update.html',
              controller: 'NTS0201UpdateCtrl',
              controllerAs: 'vm',
              windowClass: 'app-modal',
              resolve: {
                  deps: ['$ocLazyLoad',
                      function ($ocLazyLoad) {
                          return $ocLazyLoad.load([
                              'app/controllers/NTS0201Employee/NTS0201UpdateController.js',
                              'app/services/NTS0201Employee/NTS0201Service.js',
                          ]);
                      }
                  ], items: function () {
                      return { id: id };
                  }
              }
          });
          modalInstance.result.then(function (rs) {
              if (rs)
                  fnSearchEmployee();
          }, function () {
          });
      }
  }
]);