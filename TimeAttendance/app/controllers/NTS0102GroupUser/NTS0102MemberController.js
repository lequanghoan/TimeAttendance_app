/**
* <copyright company="nhantinsoft.vn">
* Author: Vũ Văn Văn
* Created Date: 30/09/2017 13:08
* </copyright>
*/
app.controller('NTS0102MemberCtrl', ['$scope','$uibModal', '$uibModalInstance', 'NTS0102Service', 'items', 'message', 'authService',
  function ($scope,$uibModal, $uibModalInstance, NTS0102Service, items, message, authService) {
      var vm = this;
      vm.ListData = [];
      vm.Total = 0;
      vm.GroupName = items.name;

      fnInitPage();

      vm.FnSearch = fnListMemberGroup;
      vm.FnClose = fnCloseModal;

      //Khởi tạo trang
      function fnInitPage() {
          $scope.totalItemsM = 0;
          $scope.currentPageM = 1;
          $scope.numPerPageM = 5;
          $scope.maxSizeM = 5;

          fnListMemberGroup();
      }

      //Danh sách thành viên trong nhóm
      function fnListMemberGroup() {
          NTS0102Service.ListMemberGroup(items.id).then(function (data) {
              vm.ListData = data;
              angular.forEach(vm.ListData, function (item, index) {
                  item.index = index + 1;
              });

              //phan trang
              $scope.totalItemsM = vm.ListData.length;
          }, function (error) {
              message.ShowMessage(error.message, 1);
          });
      }

      //Đóng model
      function fnCloseModal() {
          $uibModalInstance.close(false);
      }
  }
]);
