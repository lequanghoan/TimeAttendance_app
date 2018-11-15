/**
* <copyright company="nhantinsoft.vn">
* Author: Vũ Văn Văn
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
app.controller('NTS0102UpdateCtrl', ['$scope', '$http', 'NTS0102Service', 'message', '$state', '$stateParams', 'authService', 'notify', 'common',
  function ($scope, $http, NTS0102Service, message, $state, $stateParams, authService, notify, common) {
      var vm = this;
      vm.ListFunction = [];
      vm.SelectAll = false;
      vm.TotalGroup = 0;
      vm.TotalFunc = 0;
      vm.TotalChoose = 0;
      vm.InfoLogin = authService.authentication;
      fnInitPage();

      //Gán sự kiện
      vm.FnUpdate = fnUpdate;
      vm.FnClose = fnClose;
      vm.FnSelect = fnSelect;
      vm.FnSelectAll = fnSelectAll;

      //Khởi tạo trang
      function fnInitPage() {
          fnInitialModel();
          fnGetGroupUserById();
      }

      //Khởi tạo model
      function fnInitialModel() {
          vm.Model = {
              GroupId: '',
              Name: '',
              HomePage: '',
              Description: '',
              UpdateBy: '',
          };
          vm.Model.UserId = vm.InfoLogin.userid;
      }

      //Cập nhật
      function fnUpdate() {
          vm.Model.UserId = vm.InfoLogin.userid;
          vm.Model.ListPermission = jQuery.grep(vm.ListFunction, function (item, i) {
              return (item.GroupFunctionId != "" && item.Select);
          });
          NTS0102Service.UpdateGroupUser(vm.Model).then(function (data) {
              fnInitialModel();
              notify('Cập nhật nhóm quyền thành công!');
              $state.go('app.group-user-list');
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      //Hiển thị thông tin
      function fnGetGroupUserById() {
          NTS0102Service.GetGroupUserById($stateParams.id).then(function (data) {
              vm.Model = data;
              vm.Model.UpdateBy = authService.authentication.id;
              fnFillFunction();
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      //Thoát về danh sách
      function fnClose() {
          $state.go('app.group-user-list');
      }

      //Fill danh sách quyền
      function fnFillFunction() {
          NTS0102Service.GetFunction().then(function (data) {
              vm.ListFunction = data;
              if (vm.Model.ListPermission != null) {
                  $.each(vm.ListFunction, function (i, item) {
                      var listCheck = jQuery.grep(vm.Model.ListPermission, function (itemFunc, i) {
                          return (item.GroupFunctionId != "" && itemFunc.FunctionId == item.FunctionId);
                      });
                      if (listCheck.length > 0) {
                          item.Select = true;
                          listCheck[0].GroupFunctionId = item.GroupFunctionId;
                      }
                  });

                  $.each(vm.Model.ListPermission, function (i, item) {
                      fnSelect(item);
                  });
              }

              var ListGroup = jQuery.grep(vm.ListFunction, function (item, i) {
                  return (item.GroupFunctionId == "");
              });
              var ListFunc = jQuery.grep(vm.ListFunction, function (item, i) {
                  return (item.GroupFunctionId != "");
              });
              vm.TotalGroup = ListGroup.length;
              vm.TotalFunc = ListFunc.length;

              //Check all trong nhóm

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