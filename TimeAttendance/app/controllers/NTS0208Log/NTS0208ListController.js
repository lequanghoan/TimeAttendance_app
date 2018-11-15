/**
* <copyright company="nhantinsoft.vn">
* Author: Vũ Văn Văn
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
app.controller('NTS0208ListCtrl', ['$scope', '$http', 'NTS0208Service', 'message', 'authService', 'ngAuthSettings', '$stateParams', 'common',
function ($scope, $http, NTS0208Service, message, authService, ngAuthSettings, $stateParams, common) {
    var vm = this;
    vm.ListUserType = common.ListUserType;
    vm.ListLogType = [{ LogType: '', LogTypeName: "<< Tất cả >>" }, { LogType: 0, LogTypeName: "Truy cập" }, { LogType: 1, LogTypeName: "Khai thác dữ liệu" }];
    vm.InfoLogin = authService.authentication;
    $scope.ServiceBase = ngAuthSettings.apiServiceBaseUri;
    vm.ListResult = [];
    vm.Total = 0;
    vm.ListPageSize = [];
    fnInitPage();

    //Gán sự kiện
    vm.FnSearch = fnSearchUserEventLog;
    vm.FnClear = fnClearData;

    //Khởi tạo trang
    function fnInitPage() {
        fnInitialModel();
        InitPageSize();
        fnSearchUserEventLog();
    }
    function InitPageSize() {
        vm.ListPageSize.push({ Id: 10, Name: '10 bản ghi' });
        vm.ListPageSize.push({ Id: 15, Name: '15 bản ghi' });
        vm.ListPageSize.push({ Id: 20, Name: '20 bản ghi' });
        vm.ListPageSize.push({ Id: 30, Name: '30 bản ghi' });
        vm.ListPageSize.push({ Id: 40, Name: '40 bản ghi' });
        vm.ListPageSize.push({ Id: 50, Name: '50 bản ghi' });

    }
    //Khởi tạo model
    function fnInitialModel() {
        //Khởi thông số phân trang
        $scope.totalItems = 0;
        $scope.currentPage = 1;
        $scope.numPerPage = common.NumPerPage;
        $scope.maxSize = common.MaxSize;

        var date = new Date();
        var firstDay = new Date(date.getFullYear(), date.getMonth(), 1);
        var lastDay = new Date(date.getFullYear(), date.getMonth() + 1, 0);

        vm.Model = {
            UserIdSearch: $stateParams.id,
            UserName: '',
            FullName: '',
            Description: '',
            LogType: '',
            UserType: '',
            LogDateFrom: firstDay,
            LogDateTo: lastDay,
            PageSize: $scope.numPerPage,
            PageNumber: $scope.currentPage,
            OrderBy: 'CreateDate',
            OrderType: false
        };
        vm.Model.UserId = vm.InfoLogin.userid;

      
    }

    //Tìm kiếm
    function fnSearchUserEventLog(objectorder, saveOption) {
        vm.Model.PageNumber = $scope.currentPage;
        if (objectorder != null && objectorder != "" && objectorder != undefined) {
            vm.Model.OrderBy = objectorder.by;
            vm.Model.OrderType = objectorder.type;
        }
        var dateFrom = moment(vm.Model.LogDateFrom).locale('vi').format('YYYY-MM-DD');
        vm.Model.LogDateFrom = null;
        vm.Model.LogDateFrom = dateFrom;
        var dateTo = moment(vm.Model.LogDateTo).locale('vi').format('YYYY-MM-DD');
        vm.Model.LogDateTo = null;
        vm.Model.LogDateTo = dateTo;
        NTS0208Service.SearchUserEventLog(vm.Model, saveOption).then(function (data) {
            vm.ListResult = data.ListResult;
            $scope.totalItems = data.TotalItem;

            //Tính index bắt đầu
            $scope.StartIndex = (($scope.currentPage - 1) * vm.Model.PageSize);

            setTimeout(function () {
                $('.scroller').perfectScrollbar('update');
            }, 100);

            if (saveOption != null && saveOption != undefined && saveOption != "" && $scope.totalItems == 0) {
                message.ShowMessage("Không có dữ liệu để xuất tệp", 1);
            } else {
                if (data.PathFile != "") {
                    var link = document.createElement('a');
                    link.href = $scope.ServiceBase + data.PathFile;
                    link.download = 'Download.xlsx';
                    document.body.appendChild(link);
                    link.click();
                }
            }
        }, function (error) {
            vm.ListResult = [];
            message.ShowMessage(error.message, 1);
        });
    }

    //Làm mới tìm kiếm
    function fnClearData() {
        fnInitialModel();
        fnSearchUserEventLog();
    }
}
]);