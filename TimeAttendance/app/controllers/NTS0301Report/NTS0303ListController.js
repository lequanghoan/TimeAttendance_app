/**
* <copyright company="nhantinsoft.vn">
* Author: Vũ Văn Văn
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
app.controller('NTS0303ListCtrl', ['$scope', '$http', 'NTS0301Service', '$uibModal', 'message', 'notify', 'common', 'authService', 'ngAuthSettings',
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
      vm.VehicleInStatistic = [];
      vm.DateNow = new Date();
      vm.ListDay = [];
      vm.ListTime = [];
      fnInitPage();

      //Gán sự kiện
      vm.FnSearch = fnDensityLog;
      vm.FnSearchEx = fnDensityLogEx;
      vm.FnClear = fnClearData;
      vm.ChangeMonth = changeMonth;
      //Khởi tạo trang
      function fnInitPage() {
          fnInitialModel();
          Init();
          fnDensityLog();
      }
      //Khởi tạo model
      function fnInitialModel() {
          vm.Model = {
              WeekView: '',
              DateFrom: '',
              DateTo: '',
              Export: 0,
              Week: '1',
              Month: vm.DateNow.getMonth() + 1,
              Year: vm.DateNow.getFullYear(),
          };
          vm.Model.Week = GetWeek(vm.DateNow) - 1;
          changeMonth();
      }
      function Init() {
          for (var i = 2017; i < 2038; i++) {
              vm.ListYear.push({ Id: i, Name: 'Năm ' + i });
          }
          for (var i = 1; i < 13; i++) {
              vm.ListMonth.push({ Id: i, Name: 'Tháng ' + i });
          }
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
      //Làm mới tìm kiếm
      function fnClearData() {
          fnInitialModel();
          fnDensityLog();
      }
      //Tìm kiếm
      function fnDensityLog() {
          if (isNaN(vm.Model.Week)) {
              notify('Tuần phải là số!');
              return false;
          } else {
              var week = parseInt(vm.Model.Week);
              if (week < 1 || week > 52) {
                  notify('Tuần từ 1-52!');
                  return false;
              }
          }
          vm.Model.DateFrom = moment(getDateOfISOWeek(vm.Model.Week, vm.Model.Year)).locale('vi').format('YYYY-MM-DD');
          vm.Model.Export = 0;
          vm.VehicleInStatistic = [];
          vm.ListDay = [];
          vm.ListTime = [];
          NTS0301Service.DensityLog(vm.Model).then(function (data) {
              vm.ListDay = data.listDay;
              vm.ListTime = data.listTime;
              angular.forEach(data.ListResult, function (item) {
                  vm.VehicleInStatistic.push([item.x, item.y, item.VehicleInStatistic]);
              });

              $(function () {
                  $('#containerheat').highcharts({

                      chart: {
                          type: 'heatmap',
                          marginTop: 40,
                          marginBottom: 80,
                          plotBorderWidth: 1,
                          style: {
                              fontFamily: 'Open Sans',
                          }
                      },
                      title: {
                          text: "Biểu đồ thống kê mật độ nhận diện nhân viên, khách hàng"
                      },

                      xAxis: {
                          categories: vm.ListTime
                      },

                      yAxis: {
                          categories: vm.ListDay,
                          title: null
                      },

                      colorAxis: {
                          min: 0,
                          minColor: '#FFFFFF',
                          maxColor: '#A52A2A'
                          //  maxColor: Highcharts.getOptions().colors[0]
                      },

                      legend: {
                          align: 'right',
                          layout: 'vertical',
                          margin: 0,
                          verticalAlign: 'top',
                          y: 25,
                          symbolHeight: 280
                      },

                      tooltip: {
                          formatter: function () {
                              return '<b>' + this.series.xAxis.categories[this.point.x] + '</b> <br><b>' +
                              this.point.value + '</b> lượt nhận diện <br><b>' + this.series.yAxis.categories[this.point.y] + '</b>';
                          }
                      },

                      series: [{
                          name: 'Sales per employee',
                          borderWidth: 1,
                          data: vm.VehicleInStatistic,
                          dataLabels: {
                              //enabled: true,
                              color: '#000000'
                          }
                      }],
                  });
              });
          }, function (error) {
              vm.ListResult = [];
              message.ShowMessage(error.message, 1);
          });
      }

      function fnDensityLogEx() {

          vm.Model.Export = 1;
          NTS0301Service.DensityLog(vm.Model).then(function (data) {
              vm.ListResult = data.ListResult;
              $scope.totalItems = data.TotalItem;

              if (data.PathFile != "") {
                  var link = document.createElement('a');
                  link.href = $scope.ServiceBase + data.PathFile;
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
  }
]);