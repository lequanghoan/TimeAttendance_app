/**
* <copyright company="nhantinsoft.vn">
* Author: Hiepdv
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
app.controller('NTS0201CreateCtrl', ['$scope', '$http', '$uibModalInstance', 'NTS0201Service', 'message', '$state', 'authService', 'notify', 'common',
  function ($scope, $http,$uibModalInstance, NTS0201Service, message, $state, authService, notify, common) {
      var vm = this;
      $scope.Common = common;
      vm.InfoLogin = authService.authentication;
      vm.ListJobTitle = [];
      vm.ListDepartment = [];
      $scope.obj = {};

      fnInitPage();
      vm.FnContinue = fnAddAndContinue;
      vm.FnAdd = fnAdd;
      vm.FnClear = fnClearData;
      vm.FnClose = fnCloseModal;
      //Khởi tạo trang
      function fnInitPage() {
          fnInitialModel();
          fnGetAllDepartment();
          fnGetAllJobTitle();
      }

      //Khởi tạo model
      function fnInitialModel() {
          vm.Model = {
              EmployeeId: '',
              DepartmentId: '',
              JobTitleId: '',
              FaceId: '',
              Name: '',
              Code: '',
              DateOfBirth: '',
              Gender: '',
              InComeDate: '',
              OutComeDate: '',
              Address: '',
              IdentifyCardNumber: '',
              Description: '',
              LinkImage: '',
              LinkImageFaceId: '',
              CreateBy: vm.InfoLogin.userid,
          };
      }

      //Thêm mới và tiếp tục
      function fnAddAndContinue() {
          NTS0201Service.CreateEmployee($scope.obj.flow.files, vm.Model).then(function () {
              notify('Thêm mới nhân viên thành công');
              fnClearData();
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      //Thêm mới và trở về danh sách
      function fnAdd() {
          NTS0201Service.CreateEmployee($scope.obj.flow.files, vm.Model).then(function () {
              notify('Thêm mới nhân viên thành công');
              fnCloseModalOk();
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      //Đóng model
      function fnCloseModalOk() {
              $uibModalInstance.close(true);
      }
      function fnCloseModal() {
          $uibModalInstance.close(true);
      }
      //Xóa thông tin trên giao diện
      function fnClearData() {
          $scope.obj.flow.cancel();
          //Khỏi tạo lại model
          fnInitialModel();
          fnGetAllJobTitle();
          fnGetAllDepartment();
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

  }
]);