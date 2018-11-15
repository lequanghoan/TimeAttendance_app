/**
* <copyright company="nhantinsoft.vn">
* Author: Hiepdv
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
app.controller('NTS0201UpdateCtrl', ['$scope', '$http', '$uibModalInstance', 'NTS0201Service', 'items', 'message', '$state', 'authService', 'notify', 'common', 'ngAuthSettings',
  function ($scope, $http, $uibModalInstance, NTS0201Service, items, message, $state, authService, notify, common, ngAuthSettings) {
      var vm = this;
      $scope.Common = common;
      $scope.ServiceBase = ngAuthSettings.apiServiceBaseUri;
      vm.InfoLogin = authService.authentication;
      vm.ListJobTitle = [];
      vm.ListDepartment = [];
      $scope.obj = {};
      vm.ListImg = [];
      vm.ListImgName = [];
      fnInitPage();

      vm.FnUpdate = fnUpdate;
      vm.FnClear = fnClearData;
      vm.FnClose = fnCloseModal;
      vm.FnDeleteFile = fnDeleteFile;
      //Khởi tạo trang
      function fnInitPage() {
          fnInitialModel();
          fnGetAllDepartment();
          fnGetAllJobTitle();
          fnGetEmployeeById();
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
              LinkImageFaceId:'',
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
      function fnUpdate() {
          NTS0201Service.UpdateEmployee($scope.obj.flow.files, vm.Model).then(function () {
              notify('Cập nhật nhân viên thành công');
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
          $uibModalInstance.close(false);
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
      //get by id
      function fnGetEmployeeById() {
          NTS0201Service.GetEmployeeById(items.id).then(function (data) {
              vm.Model = data;
              //đường dẫn ảnh
              var lstImg = vm.Model.LinkImage.split(';')
              if (lstImg.length > 0) {
                  if (lstImg[0] != "" && lstImg[0] != " ") {
                      vm.ListImg = lstImg;
                  }
              }
              //faceid
              var lstImgName = vm.Model.LinkImageFaceId.split(';')
              if (lstImgName.length > 0) {
                  if (lstImgName[0] != "" && lstImgName[0] != " ") {
                      vm.ListImgName = lstImgName;
                  }
              }

          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      function fnDeleteFile(index) {
          vm.ListImg.splice(index, 1);
          vm.ListImgName.splice(index, 1);
          if (vm.ListImg.length > 0) {
              for (var i = 0; i < vm.ListImg.length; i++) {
                  if (i == 0) {
                      vm.Model.LinkImage = vm.ListImg[i];
                      vm.Model.LinkImageFaceId = vm.ListImgName[i];
                  } else {
                      vm.Model.LinkImage += ";" + vm.ListImg[i];
                      vm.Model.LinkImageFaceId += ";" + vm.ListImgName[i];
                  }
              }
          } else {
              vm.Model.LinkImage = "";
              vm.Model.LinkImageFaceId = "";
          }
      }



  }
]);