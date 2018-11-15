app.controller('ModalImageCtrl', ['$scope', '$http','$modalInstance', 'item', function ($scope, $http, $modalInstance, item) {
    var vm = this;
    vm.Model = item;

    vm.exit = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.fileInput = item.FullImagePath;
    $scope.overlays = [{ x: 50, y: 50, w: 106, h: 10, color: '#00FF00' }];
    $scope.options = { controls: { toolbar: true, fit: 'width' } };
    $scope.content = item;
}]);