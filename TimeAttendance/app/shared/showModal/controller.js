app.controller('ShowModalCtrl', ['$scope', '$uibModalInstance', 'items', function ($scope, $modalInstance, items) {
    var vm = this;
    vm.Messageinfo = items;
    // type: 0: Thông báo bình thường; 1: Thông báo lỗi; 2: Thông báo cảnh báo
    vm.ok = function () {
        $modalInstance.close(true);
    };
}]);