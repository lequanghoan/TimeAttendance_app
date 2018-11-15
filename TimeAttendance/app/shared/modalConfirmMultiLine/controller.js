app.controller('ConfirmMultiLineCtrl', ['$scope', '$uibModalInstance', 'items', function ($scope, $modalInstance, items) {
    var vm = this;
    vm.title = items.Title;
    vm.check = true;

    vm.ok = function () {
        $modalInstance.close(true);
    };

    vm.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.$on('$locationChangeStart', function () {
        $modalInstance.dismiss('cancel');
    });
}]);