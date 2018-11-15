app.controller('ConfirmCtrl', ['$scope', '$uibModalInstance', function ($scope, $modalInstance) {
    var vm = this;

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