app.controller('ModalVideoCtrl', ['$scope', '$http', '$modalInstance', 'item', 'ngAuthSettings', function ($scope, $http, $modalInstance, item, ngAuthSettings) {
    var vm = this;
    vm.Model = item;
    vm.serviceBase = ngAuthSettings.apiServiceBaseUri;
    vm.exit = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.videoPath = vm.serviceBase + item.VideoPath;

}]);