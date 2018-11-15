/**
* <copyright company="nhantinsoft.vn">
* Author: Vũ Văn Văn
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
app.controller('NTS0302ListCtrl', ['$scope', '$http', 'NTS0301Service', '$uibModal', 'message', 'notify', 'common', 'authService', 'ngAuthSettings',
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
      vm.ListPageSize = [];
      vm.DateNow = new Date();
      fnInitPage();

      //Gán sự kiện
      vm.FnSearch = fnTransactionLog;
      vm.FnSearchEx = fnTransactionLogEx;
      vm.FnClear = fnClearData;
      vm.FnShowImage = fnShowImage;
      vm.FnViewTime = fnViewTime;
      //Khởi tạo trang
      function fnInitPage() {
          fnInitialModel();
          InitPageSize();
          fnTransactionLog();
      }
      //Khởi tạo model
      function fnInitialModel() {
          //Khởi thông số phân trang
          $scope.totalItems = 0;
          $scope.currentPage = 1;
          $scope.numPerPage = common.NumPerPage;
          $scope.maxSize = common.MaxSize;

          vm.Model = {
              Export: 0,
              ClientIPAddress: '',
              CameraIPAdress: '',
              DateFromv: vm.DateNow,
              DateTov: vm.DateNow,
              StatusCode: '',
              TimeFrom: '00:01',
              TimeTo: '23:59',
              PageSize: $scope.numPerPage,
              PageNumber: $scope.currentPage,
              OrderBy: 'Date',
              OrderType: false
          };

      }
      //Làm mới tìm kiếm
      function fnClearData() {
          fnInitialModel();
          fnTransactionLog();
      }
      function fnViewTime(time) {
          if (time != '' && time != null) {
              var timeArray = time.toString().split('.');
              return timeArray[0];
          } else {
              return '';
          }

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
      function InitPageSize() {
          vm.ListPageSize.push({ Id: 10, Name: '10 bản ghi' });
          vm.ListPageSize.push({ Id: 15, Name: '15 bản ghi' });
          vm.ListPageSize.push({ Id: 20, Name: '20 bản ghi' });
          vm.ListPageSize.push({ Id: 30, Name: '30 bản ghi' });
          vm.ListPageSize.push({ Id: 40, Name: '40 bản ghi' });
          vm.ListPageSize.push({ Id: 50, Name: '50 bản ghi' });

      }
      //Tìm kiếm
      function fnTransactionLog(objectorder) {
          vm.Model.PageNumber = $scope.currentPage;
          if (objectorder != null) {
              vm.Model.OrderBy = objectorder.by;
              vm.Model.OrderType = objectorder.type;
          }

          if (vm.Model.DateFromv == null || vm.Model.DateFromv == '') {
              notify('Chọn thời gian từ!');
              return false;
          } else
              if (vm.Model.DateTov == null || vm.Model.DateTov == '') {
                  notify('Chọn thời gian đến!');
                  return false;
              }
          var checkIn = vm.Model.TimeFrom.split(':');
          var checkOut = vm.Model.TimeTo.split(':');
          if (parseInt(checkIn[0]) > 23) {
              notify('Giờ từ không được lớn hơn 23 giờ'); return false;
          } else if (parseInt(checkIn[1]) > 59) {
              notify('Phút từ không được lớn hơn 59 phút'); return false;
          }
          else if (parseInt(checkOut[0]) > 23) {
              notify('Giờ đến không được lớn hơn 23 giờ'); return false;
          } else if (parseInt(checkOut[1]) > 59) {
              notify('Phút đến không được lớn hơn 59 phút'); return false;
          }
          var to = moment(vm.Model.DateTov).locale('vi').format('YYYY-MM-DD');
          vm.Model.DateTo = null;
          vm.Model.DateTo = to;
          var from = moment(vm.Model.DateFromv).locale('vi').format('YYYY-MM-DD');
          vm.Model.DateFrom = null;
          vm.Model.DateFrom = from;

          vm.Model.Export = 0;
          NTS0301Service.TransactionLog(vm.Model).then(function (datarp) {
              vm.ListResult = datarp.ListResult;
              $scope.totalItems = datarp.TotalItem;
              $scope.TotalItemOkCount = datarp.TotalItemOkCount;

              //Tính index bắt đầu
              $scope.StartIndex = (($scope.currentPage - 1) * vm.Model.PageSize);

              if (datarp.TotalItem > 0) {

                  //xử lý bản đồ
                  // Load google charts
                  google.charts.load('current', { 'packages': ['corechart'] });
                  google.charts.setOnLoadCallback(drawChart);

                  // Draw the chart and set the chart values
                  function drawChart() {
                      var data = google.visualization.arrayToDataTable([
                      ['Task', 'Hours per Day'],
                      ['Thành công', datarp.TotalItemOk],
                      ['Lỗi', datarp.TotalItemNotOk],
                      ]);

                      // Optional; add a title and set the width and height of the chart
                      var options = { 'title': 'Biểu đồ thống kê tỉ lệ nhận diện thành công, thất bại ', 'width': 500, 'height': 450 };

                      // Display the chart inside the <div> element with id="piechart"
                      var chart = new google.visualization.PieChart(document.getElementById('piechart'));
                      chart.draw(data, options);
                  }

              } else {
                  var html = document.getElementById('piechart').innerHTML = '';
              }

              setTimeout(function () {
                  $('.scroller').perfectScrollbar('update');
              }, 100);
          }, function (error) {
              vm.ListResult = [];
              message.ShowMessage(error.message, 1);
          });
      }

      function fnTransactionLogEx() {
          if (vm.Model.Type == '1') {
              if (vm.Model.Date == null || vm.Model.Date == '') {
                  notify('Chọn ngày thống kê!');
                  return false;
              }
          }
          vm.Model.Export = 1;
          NTS0301Service.TransactionLog(vm.Model).then(function (data) {
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