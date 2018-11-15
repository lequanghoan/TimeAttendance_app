/**
* <copyright company="nhantinsoft.vn">
* Author: Vũ Văn Văn
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
app.controller('NTS0101UpdateCtrl', ['$scope', '$http', 'NTS0101Service', 'message', '$state', '$stateParams', 'authService', 'notify', 'common', 'ngAuthSettings',
  function ($scope, $http, NTS0101Service, message, $state, $stateParams, authService, notify, common, ngAuthSettings) {
      var vm = this;
      $scope.Common = common;
      $scope.ServiceBase = ngAuthSettings.apiServiceBaseUri;
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

      //Gán sự kiện
      vm.FnUpdate = fnUpdate;
      vm.FnClose = fnClose;
      vm.FnChangeGroup = fnChangeGroup;
      vm.FnSelect = fnSelect;
      vm.FnSelectAll = fnSelectAll;

      //Khởi tạo trang
      function fnInitPage() {
          //Khởi tạo model
          fnInitialModel();

          //Get thông tin user
          fnGetUserById();
      }

      //Khởi tạo model
      function fnInitialModel() {
          vm.Model = {};
          //Nhóm quyền
          fnGetSelectGroup();
          //danh sách đơn vị
         // fnGetSelectUnit();
      }

      //Cập nhật
      function fnUpdate() {
          vm.Model.ListPermission = jQuery.grep(vm.ListFunction, function (item, i) {
              return (item.GroupFunctionId != "" && item.Select);
          });

          vm.Model.LogUserId = vm.InfoLogin.userid;

          NTS0101Service.UpdateUser($scope.obj.flow.files, vm.Model).then(function () {
              notify('Cập nhật thông tin tài khoản thành công!');
              $state.go('app.user-list');
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      //Hiển thị thông tin
      function fnGetUserById() {
          NTS0101Service.GetUserById($stateParams.id).then(function (data) {
              vm.Model = data;
              vm.Model.BirthDay = vm.Model.BirthDay != null ? vm.Model.BirthDay : '';
              vm.Model.UpdateBy = authService.authentication.id;

              fnFillFunction();
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      //Thoát về danh sách
      function fnClose() {
          $state.go('app.user-list');
      }

      //Lấy thông tin combobox nhóm tài khoản
      function fnGetSelectGroup() {
          NTS0101Service.GetSelectGroup().then(function (data) {
              vm.ListGroup = data;
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      //Fill danh sách quyền theo nhóm tài khoản
      function fnFillFunction() {
          NTS0101Service.GetFunction(vm.Model.GroupId).then(function (data) {
              vm.ListFunction = data;

              if (vm.Model.ListPermission != null) {
                  $.each(vm.ListFunction, function (i, item) {
                      var listCheck = jQuery.grep(vm.Model.ListPermission, function (itemFunc, i) {
                          return (itemFunc.FunctionId == item.FunctionId && item.GroupFunctionId != "");
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

      //Thay đổi nhóm tài khoản
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