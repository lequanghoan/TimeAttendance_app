/**
* <copyright company="nhantinsoft.vn">
* Author: Mr.VanVV
* Created Date: 10/10/2017
* </copyright>
*/
'use strict';
app.controller('NotifyCtrl', ['$scope', '$http', 'NotifyService', 'authService', 
  function ($scope, $http, NotifyService, authService) {
      var vm = this;

      vm.TotalDocument = 0;

      fnInitPage();

      //Khởi tạo trang
      function fnInitPage() {
          vm.Model = {
              UserId: authService.authentication.id,
          };

          fnGetNotifyDocument();
      }


      //Tìm kiếm
      function fnGetNotifyDocument() {
         
      }

   
  }
]);