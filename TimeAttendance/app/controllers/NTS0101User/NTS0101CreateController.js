/**
* <copyright company="nhantinsoft.vn">
* Author: Vũ Văn Văn
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
app.controller('NTS0101CreateCtrl', ['$scope', '$http', 'NTS0101Service', 'message', '$state', 'authService', 'notify', 'common',
  function ($scope, $http, NTS0101Service, message, $state, authService, notify, common) {
      var vm = this;
      $scope.Common = common;
      vm.InfoLogin = authService.authentication;
      vm.ListGroup = [];
      vm.ListFunction = [];
      vm.ListUnit = [];
      vm.SelectAll = false;
      vm.TotalGroup = 0;
      vm.TotalFunc = 0;
      vm.TotalChoose = 0;
      $scope.obj = {};

      fnInitPage();
      vm.FnContinue = fnAddAndContinue;
      vm.FnAdd = fnAddUser;
      vm.FnClear = fnClearData;
      vm.FnChangeGroup = fnChangeGroup;
      vm.FnSelect = fnSelect;
      vm.FnSelectAll = fnSelectAll;

      //Khởi tạo trang
      function fnInitPage() {
          //Khởi tạo model
          fnInitialModel();   
      }

      //Khởi tạo model
      function fnInitialModel() {
          vm.Model = {
              Name: '',
              GroupId: '',
              FullName: '',
              BirthDay: '',
              Email: '',
              UnitId: '',
              Role: '',
              Type: common.AccountCompany,
              PhoneNumber: '',
              Description: '',
              ImageLink: '',
              CreateBy: authService.authentication.id,
              LogUserId : vm.InfoLogin.userid,
          };
          //Nhóm quyền
          fnGetSelectGroup();
          //danh sách đơn vị
         // fnGetSelectUnit();
      }

      //Thêm mới và tiếp tục
      function fnAddAndContinue() {
          //Danh sách quyền và phân cấp quyền
          getPermission();

          NTS0101Service.CreateUser($scope.obj.flow.files, vm.Model).then(function () {
              notify('Thêm mới tài khoản thành công');
              $state.go('app.user-create');
              fnClearData();
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      //Thêm mới và trở về danh sách
      function fnAddUser() {
          //Danh sách quyền và phân cấp quyền
          getPermission();

          NTS0101Service.CreateUser($scope.obj.flow.files, vm.Model).then(function () {
              notify('Thêm mới tài khoản thành công');
              $state.go('app.user-list');
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      //Danh sách quyền và phân cấp quyền
      function getPermission() {
          vm.Model.ListPermission = jQuery.grep(vm.ListFunction, function (item, i) {
              return (item.GroupFunctionId !== "" && item.Select);
          });
      }

      //Xóa thông tin trên giao diện
      function fnClearData() {
          vm.ManagementUnitId = "";
          vm.UnitId = "";
          vm.ListFunction = [];
          vm.SelectAll = false;
          vm.TotalGroup = 0;
          vm.TotalFunc = 0;
          vm.TotalChoose = 0;
          $scope.obj.flow.cancel();
          //Khỏi tạo lại model
          fnInitialModel();
      }

      //Lấy thông tin combobox nhóm tài khoản
      function fnGetSelectGroup() {
          NTS0101Service.GetSelectGroup().then(function (data) {
              vm.ListGroup = data;
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      //Fill danh sách quyền
      function fnFillFunction() {
          NTS0101Service.GetFunction(vm.Model.GroupId).then(function (data) {
              vm.ListFunction = data;
              angular.forEach(vm.ListFunction, function (item, index) {
                  item.Select = true;
              });
              
              var ListGroup = jQuery.grep(vm.ListFunction, function (item, i) {
                  return (item.GroupFunctionId == "");
              });
              var ListFunc = jQuery.grep(vm.ListFunction, function (item, i) {
                  return (item.GroupFunctionId != "");
              });
              vm.TotalGroup = ListGroup.length;
              vm.TotalFunc = ListFunc.length;
              vm.TotalChoose = ListFunc.length;
              if (vm.TotalFunc > 0) {
                  vm.SelectAll = true;
              }
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      //Fill danh sách quyền theo id tài khoản
      function fnFillFunctionByUser() {
          NTS0101Service.GetFunctionByUser(vm.InfoLogin.id).then(function (data) {
              vm.ListFunction = data;

              angular.forEach(vm.ListFunction, function (item, index) {
                  item.Select = true;
              });

              var ListGroup = jQuery.grep(vm.ListFunction, function (item, i) {
                  return (item.GroupFunctionId == "");
              });
              var ListFunc = jQuery.grep(vm.ListFunction, function (item, i) {
                  return (item.GroupFunctionId != "");
              });
              vm.TotalGroup = ListGroup.length;
              vm.TotalFunc = ListFunc.length;
              vm.TotalChoose = ListFunc.length;
              if (vm.TotalFunc > 0) {
                  vm.SelectAll = true;
              }
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      //Thay đổi trạng thái check
      function fnSelect(itemFunc) {
          //Trường hợp thay đổi trạng thái cho cha
          if (itemFunc.GroupFunctionId == "") {
              $.each(vm.ListFunction, function (j, item) {
                  //Gán giá trị staus cho nhóm cha
                  if (item.FunctionId == itemFunc.FunctionId && item.GroupFunctionId == "") {
                      item.Select = itemFunc.Select;
                  }
                  //Gán giá trị cho nhóm con
                  if (item.GroupFunctionId == itemFunc.FunctionId && item.GroupFunctionId != "") {
                      item.Select = itemFunc.Select;
                  }
              });
          }
              //Thay đổi trạng thái con
          else {
              $.each(vm.ListFunction, function (i, item) {
                  //Gán lại giá trị status cho list
                  if (item.FunctionId == itemFunc.FunctionId && item.GroupFunctionId == itemFunc.GroupFunctionId) {
                      item.Select = itemFunc.Select;
                      return;
                  }
              });


              var chekGroup = jQuery.grep(vm.ListFunction, function (item, i) {
                  return (item.GroupFunctionId == itemFunc.GroupFunctionId);
              });
              var chekSelect = jQuery.grep(vm.ListFunction, function (item, i) {
                  return (item.GroupFunctionId == itemFunc.GroupFunctionId && item.Select);
              });
              var selectall = ((chekGroup.length == chekSelect.length && chekSelect.length > 0) ? true : false);
              $.each(vm.ListFunction, function (i, item) {
                  //Gán lại giá trị status cho list
                  if (item.FunctionId == itemFunc.GroupFunctionId && item.GroupFunctionId == "") {
                      item.Select = selectall;
                      return;
                  }
              });
          }
          //Kiểm tra trạng  thái check all
          var checkAll = true;
          $.each(vm.ListFunction, function (i, item) {
              if (!item.Select) {
                  checkAll = false;
              }
          });
          vm.SelectAll = checkAll;
          var ListFuncChoose = jQuery.grep(vm.ListFunction, function (item, i) {
              return (item.GroupFunctionId != "" && item.Select);
          });
          vm.TotalChoose = ListFuncChoose.length;
      }

      //Thay đổi trạng thái check all
      function fnSelectAll() {
          vm.SelectAll = vm.SelectAll;
          $.each(vm.ListFunction, function (i, item) {
              item.Select = vm.SelectAll;
          });
          var ListFuncChoose = jQuery.grep(vm.ListFunction, function (item, i) {
              return (item.GroupFunctionId != "" && item.Select);
          });
          vm.TotalChoose = ListFuncChoose.length;
      }

      //Thay đổi tài khoản
      function fnChangeGroup() {
          if (vm.Model.GroupId == "" || vm.Model.GroupId == null || vm.Model.GroupId == undefined) {
              vm.SelectAll = false;
          }
          fnFillFunction();
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