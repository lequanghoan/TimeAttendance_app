/**
* <copyright company="nhantinsoft.vn">
* Author: Vũ Văn Văn
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
app.controller('NTS0102CreateCtrl', ['$scope', '$http', 'NTS0102Service', 'message', '$state', 'authService', 'notify', 'common',
  function ($scope, $http, NTS0102Service, message, $state, authService, notify, common) {
      var vm = this;
      vm.ListFunction = [];
      vm.TotalGroup = 0;
      vm.TotalFunc = 0;
      vm.TotalChoose = 0;
      vm.InfoLogin = authService.authentication;

      fnInitPage();
      vm.FnContinue = fnAddAndContinue;
      vm.FnAdd = fnAddGroupUser;
      vm.FnClear = fnClearData;
      vm.FnSelect = fnSelect;
      vm.FnSelectAll = fnSelectAll;

      //Khởi tạo trang
      function fnInitPage() {
          fnInitialModel();
      }

      //Khởi tạo model
      function fnInitialModel() {
          vm.SelectAll = true;
          fnFillFunction();
          vm.Model = {
              Name: '',
              Description: '',
              HomePage:'',
              CreateBy: authService.authentication.id,
          };
          vm.Model.UserId = vm.InfoLogin.userid;
      }

      //Thêm mới và tiếp tục
      function fnAddAndContinue() {
          vm.Model.ListPermission = jQuery.grep(vm.ListFunction, function (item, i) {
              return (item.GroupFunctionId != "" && item.Select);
          });
          NTS0102Service.CreateGroupUser(vm.Model).then(function () {
              fnClearData();
              notify('Thêm mới nhóm quyền thành công!');
              $state.go('app.group-user-create');
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      //Thêm mới và trở về danh sách
      function fnAddGroupUser() {
          vm.Model.ListPermission = jQuery.grep(vm.ListFunction, function (item, i) {
              return (item.GroupFunctionId != "" && item.Select);
          });
          NTS0102Service.CreateGroupUser(vm.Model).then(function () {
              fnClearData();
              notify('Thêm mới nhóm quyền thành công!');
              $state.go('app.group-user-list');
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      //Xóa thông tin trên giao diện
      function fnClearData() {
          //Khỏi tạo lại model
          fnInitialModel();
      }

      //Fill danh sách quyền
      function fnFillFunction() {
          NTS0102Service.GetFunction().then(function (data) {
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
  }
]);