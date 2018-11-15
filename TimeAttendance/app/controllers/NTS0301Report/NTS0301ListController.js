/**
* <copyright company="nhantinsoft.vn">
* Author: Vũ Văn Văn
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
app.controller('NTS0301ListCtrl', ['$scope', '$http', 'NTS0301Service', '$uibModal', 'message', 'notify', 'common', 'authService', 'ngAuthSettings',
  function ($scope, $http, NTS0301Service, $uibModal, message, notify, common, authService, ngAuthSettings) {
      var vm = this;
      $scope.ServiceBase = ngAuthSettings.apiServiceBaseUri;
      vm.InfoLogin = authService.authentication;
      $scope.Common = common;
      $scope.totalItems = 0;
      $scope.StartIndex = 0;
      vm.ListStatus = common.ListStatus;
      vm.ListResult = [];
      vm.ListYear = [];
      vm.ListMonth = [];
      vm.ListDepartment = [];
      vm.ListJobTitle = [];
      vm.ListPageSize = [];
      vm.DateNow = new Date();
      vm.DateNowFrom = new Date();
      vm.DateNowTo = new Date();
      vm.dayInWeek = vm.DateNow.getDay();
      fnInitPage();

      //Gán sự kiện
      vm.ChangeMonth = changeMonth;
      vm.FnSearch = fnSearchTimeAttendanceLogDetail;
      vm.FnSearchEx = fnSearchTimeAttendanceLogDetailEx;
      vm.FnClear = fnClearData;
      vm.FnShowImage = fnShowImage;
      //Khởi tạo trang
      function fnInitPage() {
          fnInitialModel();
          Init();
          InitPageSize();
          fnGetAllJobTitle();
          fnGetAllDepartment();
          fnSearchTimeAttendanceLogDetail();
          changeMonth();
      }
      function InitPageSize() {
          vm.ListPageSize.push({ Id: 10, Name: '10 bản ghi' });
          vm.ListPageSize.push({ Id: 15, Name: '15 bản ghi' });
          vm.ListPageSize.push({ Id: 20, Name: '20 bản ghi' });
          vm.ListPageSize.push({ Id: 30, Name: '30 bản ghi' });
          vm.ListPageSize.push({ Id: 40, Name: '40 bản ghi' });
          vm.ListPageSize.push({ Id: 50, Name: '50 bản ghi' });

      }
      function changeMonth() {
          if (!isNaN(vm.Model.Week)) {
              var week = parseInt(vm.Model.Week);
              if (week > 0 && week < 53) {
                  vm.Model.DateFrom = moment(getDateOfISOWeek(vm.Model.Week, vm.Model.Year)).locale('vi').format('YYYY-MM-DD');
                  var dateGet = new Date(vm.Model.DateFrom);
                  dateGet = addDaysWRONG(dateGet, 6);
                  vm.Model.WeekView = moment(vm.Model.DateFrom).locale('vi').format('DD/MM/YYYY') + ' - ' + moment(dateGet).locale('vi').format('DD/MM/YYYY');
              }
          }

      }
      function addDaysWRONG(date, days) {
          var result = date;
          result.setDate(date.getDate() + days);
          return result;
      }
      function ProcessTimeFrom()
      {
          vm.DateNowFrom.setDate(vm.DateNow.getDate() - vm.dayInWeek + 1);
          var dd = vm.DateNowFrom.getDate();
          var mm = vm.DateNowFrom.getMonth() + 1;
          var y = vm.DateNowFrom.getFullYear();
          var someFormattedDate = new Date(mm + '/' + dd + '/' + y);
          return someFormattedDate;
      }
      function ProcessTimeTo() {
          vm.DateNowTo.setDate(vm.DateNow.getDate() + (7 - vm.dayInWeek));
          var dd = vm.DateNowTo.getDate();
          var mm = vm.DateNowTo.getMonth() + 1;
          var y = vm.DateNowTo.getFullYear();
          var someFormattedDate = new Date(mm + '/' + dd + '/' + y);
          return someFormattedDate;
      }
      //Khởi tạo model
      function fnInitialModel() {
          $scope.totalItems = 0;
          $scope.currentPage = 1;
          $scope.numPerPage = common.NumPerPage;
          $scope.maxSize = common.MaxSize;

          vm.Model = {
              WeekView:'',
              Code: '',
              Name: '',
              DepartmentId: '',
              JobTitleId:'',
              OrderBy: 'Date',
              OrderType: false,
              PageSize: $scope.numPerPage,
              PageNumber: $scope.currentPage,
              Export:0,
              Type: '2',
              Date: vm.DateNow,//ProcessTimeFrom(),
              DateFrom: vm.DateNow,//ProcessTimeFrom(),
              DateTo: vm.DateNow,// ProcessTimeTo()
              Week: '1',
              Month: vm.DateNow.getMonth()+1,
              Year: vm.DateNow.getFullYear(),
          };
          vm.Model.Week = GetWeek(vm.DateNow)-1;
         
      }
      //lấy tuần 
      function GetWeek( d ) { 

          // Create a copy of this date object  
          var target  = new Date(d.valueOf());  
  
          // ISO week date weeks start on monday  
          // so correct the day number  
          var dayNr   = (d.getDay() + 6) % 7;  

          // Set the target to the thursday of this week so the  
          // target date is in the right year  
          target.setDate(target.getDate() - dayNr + 3);  

          // ISO 8601 states that week 1 is the week  
          // with january 4th in it  
          var jan4    = new Date(target.getFullYear(), 0, 4);  

          // Number of days between target date and january 4th  
          var dayDiff = (target - jan4) / 86400000;    

          // Calculate week number: Week 1 (january 4th) plus the    
          // number of weeks between target date and january 4th    
          var weekNr = 1 + Math.ceil(dayDiff / 7);    

          return weekNr;    

      }

      function Init() {
          for (var i = 2017; i < 2038; i++) {
              vm.ListYear.push({ Id: i, Name: 'Năm ' + i });
          }
          for (var i = 1; i < 13; i++) {
              vm.ListMonth.push({ Id: i, Name: 'Tháng ' + i });
          }
      }
      //Làm mới tìm kiếm
      function fnClearData() {
          fnInitialModel();
          fnSearchTimeAttendanceLogDetail();
      }
      //lấy ngày 
      function getDateOfISOWeek(w, y) {
          var simple = new Date(y, 0, 1 + (w - 1) * 7);
          var dow = simple.getDay();
          var ISOweekStart = simple;
          if (dow <= 4)
              ISOweekStart.setDate(simple.getDate() - simple.getDay() + 1);
          else
              ISOweekStart.setDate(simple.getDate() + 8 - simple.getDay());
          return ISOweekStart;
      }
      //Tìm kiếm
      function fnSearchTimeAttendanceLogDetail(objectorder) {
          vm.Model.PageNumber = $scope.currentPage;
          if (objectorder != null) {
              vm.Model.OrderBy = objectorder.by;
              vm.Model.OrderType = objectorder.type;
          }

          if (vm.Model.Type == '1') {
              if (vm.Model.Date == null || vm.Model.Date == '') {
                  notify('Chọn ngày thống kê từ!');
                  return false;
              }else
                  if (vm.Model.DateTo == null || vm.Model.DateTo == '') {
                      notify('Chọn ngày thống kê đến!');
                      return false;
                  }
          } else if (vm.Model.Type == '2') {
              if (isNaN(vm.Model.Week))
              {
                  notify('Tuần phải là số!');
                  return false;
              } else {
                  var week = parseInt(vm.Model.Week);
                  if (week<1 || week>52) {
                      notify('Tuần từ 1-52!');
                      return false;
                  }
              }
          }

          vm.Model.Export = 0;
          vm.Model.DateFrom = moment(getDateOfISOWeek(vm.Model.Week, vm.Model.Year)).locale('vi').format('YYYY-MM-DD');
          NTS0301Service.SearchTimeAttendanceLogDetail(vm.Model).then(function (data) {
              vm.ListResult = data.ListResult;
              $scope.totalItems = data.TotalItem;
              $scope.ServiceBase = data.PathFile;
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

      function fnSearchTimeAttendanceLogDetailEx(objectorder) {
          if (objectorder != null) {
              vm.Model.OrderBy = objectorder.by;
              vm.Model.OrderType = objectorder.type;
          }
          if (vm.Model.Type == '1') {
              if (vm.Model.Date == null || vm.Model.Date == '') {
                  notify('Chọn ngày thống kê!');
                  return false;
              } else
                  if (vm.Model.DateTo == null || vm.Model.DateTo == '') {
                      notify('Chọn ngày thống kê đến!');
                      return false;
                  }
          } else if (vm.Model.Type == '2') {
              if (isNaN(vm.Model.Week)) {
                  notify('Tuần phải là số!');
                  return false;
              } else {
                  var week = parseInt(vm.Model.Week);
                  if (week < 1 || week > 52) {
                      notify('Tuần phải từ 1 đến 52!');
                      return false;
                  }
              }
          }
          vm.Model.DateFrom = moment(getDateOfISOWeek(vm.Model.Week, vm.Model.Year)).locale('vi').format('YYYY-MM-DD');
          vm.Model.Export = 1;
          NTS0301Service.SearchTimeAttendanceLogDetail(vm.Model).then(function (data) {
              vm.ListResult = data.ListResult;
              $scope.totalItems = data.TotalItem;
              //Tính index bắt đầu
              $scope.StartIndex = (($scope.currentPage - 1) * vm.Model.PageSize);

              if (data.PathFile != "") {
                  var link = document.createElement('a');
                  link.href = $scope.ServiceBase + data.PathFile;
                  link.download = 'Download.xlsx';
                  document.body.appendChild(link);
                  link.click();
              }
         
              setTimeout(function () {
                  $('.scroller').perfectScrollbar('update');
              }, 100);
          }, function (error) {
              vm.ListResult = [];
              message.ShowMessage(error.message, 1);
          });
      }

      function fnGetAllDepartment() {
          NTS0301Service.GetAllDepartment().then(function (data) {
              vm.ListDepartment = data;
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }
      function fnGetAllJobTitle() {
          NTS0301Service.GetAllJobTitle().then(function (data) {
              vm.ListJobTitle = data;
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
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
              if (rs)
                  fnSearchEmployee();
          }, function () {
          });
      }
  }
]);