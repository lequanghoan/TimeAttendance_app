app.controller('ConfirmWithTitleCtrl', ['$scope', '$uibModalInstance', '$uibModal', 'items', function ($scope, $modalInstance,$uibModal, items) {
    var vm = this;
    vm.title = items.Title;
    vm.check = true;
    vm.id = items.id;
    vm.ViewHistory = viewHistory;

    function viewHistory(id)
    {
        var modalInstance = $uibModal.open({
            templateUrl: 'app/views/CSC0301CarSearch/CSC0301History.html',
            controller: 'CSC0301HistoryCtrl',
            controllerAs: 'vm',
            windowClass: 'app-modal',
            resolve: {
                deps: ['$ocLazyLoad',
                    function ($ocLazyLoad) {
                        return $ocLazyLoad.load([
                            'app/controllers/CSC0301CarSearch/CSC0301HistoryController.js',
                            'app/services/CSC0301CarSearch/CSC0301Service.js',
                        ]);
                    }
                ], items: function () {
                    return { id: id };
                }
            }
        });
    }

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