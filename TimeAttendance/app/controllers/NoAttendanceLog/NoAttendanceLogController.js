/**
* <copyright company="nhantinsoft.vn">
* Author: Vũ Văn Văn
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
app.controller('NoAttendanceLogCtrl', ['$scope', '$http', 'AttendanceLogService', 'NTS0301Service', '$uibModal', 'message', 'notify', 'common', 'authService', 'ngAuthSettings',
  function ($scope, $http, AttendanceLogService, NTS0301Service, $uibModal, message, notify, common, authService, ngAuthSettings) {
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

      //Khởi tạo model
      function fnInitialModel() {
          $scope.totalItems = 0;
          $scope.currentPage = 1;
          $scope.numPerPage = common.NumPerPage;
          $scope.maxSize = common.MaxSize;

          vm.Model = {
              WeekView: '',
              Code: '',
              Name: '',
              EmployeeName: '',
              EmployeeCode: '',
              DepartmentId: '',
              JobTitleId: '',
              OrderBy: 'EmployeeName',
              OrderType: true,
              PageSize: $scope.numPerPage,
              PageNumber: $scope.currentPage,
              Export: 0,
              Type: '2',
              Date: vm.DateNow,//ProcessTimeFrom(),
              DateFrom: vm.DateNow,//ProcessTimeFrom(),
              DateTo: vm.DateNow,// ProcessTimeTo()
              Week: '1',
              Month: vm.DateNow.getMonth() + 1,
              Year: vm.DateNow.getFullYear(),
          };
          vm.Model.Week = GetWeek(vm.DateNow) - 1;
          vm.Model.DateFrom = moment(getDateOfISOWeek(vm.Model.Week, vm.Model.Year)).locale('vi').format('YYYY-MM-DD');
      }

      //Gán sự kiện
      vm.ChangeWeek = changeWeek;
      vm.ChangeMonth = changeMonth;
      vm.ChangeYear = changeYear;
      vm.UpdateModel = updateModel;
      vm.FnSearch = fnSearchNoAttendanceLog;
      vm.FnClear = fnClearData;

      //Khởi tạo trang
      function fnInitPage() {
          fnInitialModel();
          InitMonthYear();
          InitPageSize();
          fnGetAllJobTitle();
          fnGetAllDepartment();
          fnSearchNoAttendanceLog();
          changeWeek();
      }

      function InitPageSize() {
          vm.ListPageSize.push({ Id: 10, Name: '10 bản ghi' });
          vm.ListPageSize.push({ Id: 15, Name: '15 bản ghi' });
          vm.ListPageSize.push({ Id: 20, Name: '20 bản ghi' });
          vm.ListPageSize.push({ Id: 30, Name: '30 bản ghi' });
          vm.ListPageSize.push({ Id: 40, Name: '40 bản ghi' });
          vm.ListPageSize.push({ Id: 50, Name: '50 bản ghi' });

      }

      function updateModel()
      {
          if (vm.Model.Type == '3')
          {
              vm.Model.DateFrom = moment( new Date(vm.Model.Year, vm.Model.Month - 1, 1)).locale('vi').format('YYYY-MM-DD');
              vm.Model.DateTo = moment( new Date(vm.Model.Year, vm.Model.Month - 1, getDaysOfMonth(vm.Model.Year, vm.Model.Month))).locale('vi').format('YYYY-MM-DD');
          }
          if (vm.Model.Type == '2')
          {
              vm.Model.DateFrom = moment(getDateOfISOWeek(vm.Model.Week, vm.Model.Year)).locale('vi').format('YYYY-MM-DD');
              var dateTo = new Date(vm.Model.DateFrom);
              vm.Model.DateTo = moment(addDaysWRONG(dateTo, 6)).locale('vi').format('YYYY-MM-DD');
          }
      }

      //Hàm thay đổi tuần
      function changeWeek() {
          if (!isNaN(vm.Model.Week)) {
              var week = parseInt(vm.Model.Week);
              if (week > 0 && week < 53) {
                  vm.Model.DateFrom = moment(getDateOfISOWeek(vm.Model.Week, vm.Model.Year)).locale('vi').format('YYYY-MM-DD');
                  var dateTo = new Date(vm.Model.DateFrom);
                  vm.Model.DateTo = moment( addDaysWRONG(dateTo, 6)).locale('vi').format('YYYY-MM-DD');
                  vm.Model.WeekView = moment(vm.Model.DateFrom).locale('vi').format('DD/MM/YYYY') + ' - ' + moment(dateTo).locale('vi').format('DD/MM/YYYY');
              }
          }
      }

      //Hàm thay đổi tháng
      function changeMonth()
      {
          if (vm.Model.Month != null)
          {
              vm.Model.DateFrom = moment(new Date(vm.Model.Year, vm.Model.Month - 1, 1)).locale('vi').format('YYYY-MM-DD');
              vm.Model.DateTo = moment(new Date(vm.Model.Year, vm.Model.Month - 1, getDaysOfMonth(vm.Model.Year, vm.Model.Month))).locale('vi').format('YYYY-MM-DD');
          }
          else
          {
              vm.Model.DateFrom = moment(new Date(vm.Model.Year, 0, 1)).locale('vi').format('YYYY-MM-DD');
              vm.Model.DateTo = moment(new Date(vm.Model.Year, 11, 31)).locale('vi').format('YYYY-MM-DD');
          }
      }

      function changeYear()
      {
          if (vm.Model.Year != null)
          {
              if (vm.Model.Type == '2')
              {
                  changeWeek();
              }
              else if (vm.Model.Type == '3')
              {
                  changeMonth();
              }
          }
      }
      //trả về ngày cuối của tuần
      function addDaysWRONG(date, days) {
          var result = date;
          result.setDate(date.getDate() + days);
          return result;
      }


      //lấy tuần 
      function GetWeek(d) {

          // Create a copy of this date object  
          var target = new Date(d.valueOf());

          // ISO week date weeks start on monday  
          // so correct the day number  
          var dayNr = (d.getDay() + 6) % 7;

          // Set the target to the thursday of this week so the  
          // target date is in the right year  
          target.setDate(target.getDate() - dayNr + 3);

          // ISO 8601 states that week 1 is the week  
          // with january 4th in it  
          var jan4 = new Date(target.getFullYear(), 0, 4);

          // Number of days between target date and january 4th  
          var dayDiff = (target - jan4) / 86400000;

          // Calculate week number: Week 1 (january 4th) plus the    
          // number of weeks between target date and january 4th    
          var weekNr = 1 + Math.ceil(dayDiff / 7);

          return weekNr;
      }

      //Khởi tạo danh sách tháng và năm
      function InitMonthYear() {
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
          fnSearchNoAttendanceLog();
          changeWeek();
      }

      //lấy ngày đầu tuần
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

      //Hàm tính số ngày trong tháng
      function getDaysOfMonth(year, month) {

          return new Date(year, month, 0).getDate();
      }

      //Tìm kiếm
      function fnSearchNoAttendanceLog(objectorder) {
          vm.Model.PageNumber = $scope.currentPage;
          if (objectorder != null) {
              vm.Model.OrderBy = objectorder.by;
              vm.Model.OrderType = objectorder.type;
          }

          checkInput();

          vm.Model.Export = 0;
          AttendanceLogService.GetListNoAttendanceLog(vm.Model).then(function (data) {
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

      function checkInput() {
          if (vm.Model.Type == '1') {
              if (vm.Model.Date == null || vm.Model.Date == '') {
                  notify('Chọn ngày thống kê từ!');
                  return false;
              }
              else if (vm.Model.DateTo == null || vm.Model.DateTo == '') {
                  notify('Chọn ngày thống kê đến!');
                  return false;
              }
          }
          else if (vm.Model.Type == '2') {
              if (isNaN(vm.Model.Week)) {
                  notify('Tuần phải là số!');
                  return false;
              }
              else {
                  var week = parseInt(vm.Model.Week);
                  if (week < 1 || week > 52) {
                      notify('Tuần phải từ 1 - 52');
                      return false;
                  }
              }
          }
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
  }
]);