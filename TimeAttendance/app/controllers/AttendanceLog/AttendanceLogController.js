/**
* <copyright company="nhantinsoft.vn">
* Author: Vũ Văn Văn
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
app.controller('AttendanceLogCtrl', ['$scope', '$http', 'AttendanceLogService', '$uibModal', 'message', 'notify', 'common', 'authService', 'ngAuthSettings',
  function ($scope, $http, AttendanceLogService, $uibModal, message, notify, common, authService, ngAuthSettings) {
      var vm = this;
      $scope.ServiceBase = ngAuthSettings.apiServiceBaseUri;
      vm.InfoLogin = authService.authentication;
      $scope.Common = common;
      vm.ListStatus = common.ListStatus;
      vm.ListDepartment = [];
      vm.ListJobTitle = [];
      vm.ListResult = [];
      vm.ListResultSelect = [];
      vm.ListPageSize = [];

      //Gán sự kiện
      vm.FnSearch = fnSearchAttendanceLog;
      vm.FnClear = fnClearData;
      vm.ExportExcel = fnExportExcel;
      vm.FnShowImage = fnShowImage;
      vm.FnShowUpdate = fnShowUpdateModal;
      vm.FnDelete = fnConfirmDelete;
      vm.FnTraining = fnTraining;
      fnInitPage();

      //Khởi tạo trang
      function fnInitPage() {
          fnInitialModel();
          InitPageSize();
          fnGetListDepartment();
          fnSearchAttendanceLog();
      }
      function fnTraining_cu()
      {
          vm.ListResultSelect = [];
          for (var i = 0; i < vm.ListResult.length; i++) {
              if (vm.ListResult[i].ObjSelect == true) {
                  vm.ListResultSelect.push(vm.ListResult[i]);
              }
          }
          if (vm.ListResultSelect.length==0) {
              notify('Bạn chưa chọn bản ghi nào!'); return false;
          } else {
              AttendanceLogService.Training(vm.ListResultSelect).then(function (data) {
                  if (data=='0') {
                      notify('Training ảnh thành công!');
                  } else {
                      notify('Đã xảy ra lỗi trong quá trình training ảnh!');
                  }
              }, function (error) {
                  message.ShowMessage(error.message, 1);
              });
          }
      }
      function fnTraining(sv,row) {
          var modalInstance = $uibModal.open({
              templateUrl: 'app/views/AttendanceLog/AttendanceLogTraining.html',
              controller: 'AttendanceLogTrainingCtrl',
              controllerAs: 'vm',
              windowClass: 'app-modal',
              resolve: {
                  deps: ['$ocLazyLoad',
                      function ($ocLazyLoad) {
                          return $ocLazyLoad.load([
                              'app/controllers/AttendanceLog/AttendanceLogTrainingController.js',
                              'app/services/AttendanceLog/AttendanceLogService.js',
                          ]);
                      }
                  ], items: function () {
                      return { row: row ,sv:sv};
                  }
              }
          });
          modalInstance.result.then(function (rs) {
             
          }, function () {
          });
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
      function fnShowImage(Image, EmployeeCode, EmployeeName, Time, JobTitleName) {
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
          }, function () {
          });
      }

      function fnGetListDepartment() {
          AttendanceLogService.GetListDepartment().then(function (data) {
              vm.ListDepartment = data;
          }, function (error) {
              message.ShowMessage(error.message, 1);
          })
      }

      function fnDeleteAttendanceLog(id) {
          AttendanceLogService.DeleteAttendanceLog(id).then(function (rs) {
              if (rs) {
                  notify('Xóa thông tin thành công!');
                  fnSearchAttendanceLog();
              }
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
                      return { Title: "Bạn có chắc chắn muốn xóa thông tin này không?" };
                  }
              }
          });
          modalInstance.result.then(function (rs) {
              if (rs) {
                  fnDeleteAttendanceLog(id);
              }
          }, function () {
          });
      }

      //Show modal sửa AttendanceLog
      function fnShowUpdateModal(id, employeeId) {
          var modalInstance = $uibModal.open({
              templateUrl: 'app/views/AttendanceLog/AttendanceLogUpdate.html',
              controller: 'AttendanceLogUpdateCtrl',
              controllerAs: 'vm',
              windowClass: 'app-modal',
              resolve: {
                  deps: ['$ocLazyLoad',
                      function ($ocLazyLoad) {
                          return $ocLazyLoad.load([
                              'app/controllers/AttendanceLog/AttendanceLogUpdateController.js',
                          ]);
                      }
                  ], items: function () {
                      return { attendanceLogId: id };
                  }
              }
          });
          modalInstance.result.then(function (rs) {
              if (rs == 1) {
                  fnSearchAttendanceLog();
              }
              else if (rs == 2) {
                  notify('Không tìm thấy thông tin muốn sửa, vui lòng kiểm tra lại!');
              }
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
              EmployeeCode: '',
              EmployeeName: '',
              DepartmentId: '',
              JobTitleId: '',
              DateFrom: new Date(),
              DateTo: new Date(),
              TimeFrom: '00:00',
              TimeTo: '23:59',
              ConfidenceFrom: '0.00',
              ConfidenceTo: '0.99',
              PageSize: $scope.numPerPage,
              PageNumber: $scope.currentPage,
              CameraIPAddress: '',
              OrderBy: 'Date',
              OrderType: false,
              FaceCount:''
          };
      }


      //Tìm kiếm
      function fnSearchAttendanceLog(objectOrder) {
          if (vm.Model.FaceCount != '' && vm.Model.FaceCount != ' ') {
              if (isNaN(vm.Model.FaceCount))
              {
                  notify('Số khuôn mặt phải là số!');
                  return false;
              }
          }
          if (!checkInput()) {
              return false;
          }
          if (objectOrder != null) {
              vm.Model.OrderBy = objectOrder.by;
              vm.Model.OrderType = objectOrder.type;
          }
          vm.Model.PageNumber = $scope.currentPage;
          AttendanceLogService.GetListAttendanceLog(vm.Model).then(function (data) {
              $scope.ServiceBase = data.PathFile;
              for (var i = 0; i < data.ListResult.length; i++) {
                  if (data.ListResult[i].EmployeeCode == null || data.ListResult[i].EmployeeCode == '') {
                      data.ListResult[i].EmployeeCode = 'Unknow';
                  }
                  if (data.ListResult[i].EmployeeName == null || data.ListResult[i].EmployeeName == '') {
                      data.ListResult[i].EmployeeName = 'Unknow';
                  }
                  if (data.ListResult[i].DepartmentName == null || data.ListResult[i].DepartmentName == '') {
                      data.ListResult[i].DepartmentName = 'Unknow';
                  }

              }

              vm.ListResult = data.ListResult;
              $scope.totalItems = data.TotalItem;

              //Tính index bắt đầu
              $scope.StartIndex = (($scope.currentPage - 1) * vm.Model.PageSize);

              //setTimeout(function () {
              //    $('.scroller').perfectScrollbar('update');
              //}, 100);
          }, function (error) {
              vm.ListResult = [];
              message.ShowMessage(error.message, 1);
          });
      }

      function checkInput() {
          if (vm.Model.DateFrom == null || vm.Model.DateFrom == '') {
              notify('Chưa chọn thời gian từ ngày!');
              return false;
          }
          if (vm.Model.DateTo == null || vm.Model.DateTo == '') {
              notify('Chưa chọn thời gian tới ngày!');
              return false;
          }

          var checkIn = vm.Model.TimeFrom.split(':');
          var checkOut = vm.Model.TimeTo.split(':');
          checkTime(checkIn);
          checkTime(checkOut);
          return true;
      }

      //Kiểm tra giờ đầu vào
      function checkTime(check) {
          if (parseInt(check[0]) > 23) {
              notify('Giờ không được lớn hơn 23 giờ');
              return false;
          }
          if (parseInt(check[1]) > 59) {
              notify('Phút không được lớn hơn 59 phút');
              return false;
          }
      }

      function fnExportExcel() {
          AttendanceLogService.ExportExcel(vm.Model).then(function (data) {
              vm.ListResult = data.ListResult;
              $scope.totalItems = data.TotalItem;

              if (data.PathExport != "") {
                  var link = document.createElement('a');
                  link.href = $scope.ServiceBase + data.PathExport;
                  link.download = 'Download.xlsx';
                  document.body.appendChild(link);
                  link.click();
              }

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

      //Làm mới tìm kiếm
      function fnClearData() {
          fnInitialModel();
          fnSearchAttendanceLog();
      }
  }
]);