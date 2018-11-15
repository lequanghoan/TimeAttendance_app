app.directive('datepickerNext', [function () {
    return {
        restrict: 'E',
        scope: {
            'isValidate': '@',
            'textValidate': '@',
            'modelDate': '=',
            'dateChange':'&',
            'disableDatepicker': '=',
        },
        templateUrl: 'app/shared/Directives/DatePicker/datepicker.html',
        replace: true,
        controller: ['$scope', '$timeout', function ($scope, $timeout) {
            var vm = $scope;
            vm.showValidate = false;
            vm.isRequired = false;
            vm.Format = 'dd/MM/yyyy';
            vm.DatePicker = {
                status: {
                    opened: false
                },
                open: function ($event) {
                    if (!$scope.disableDatepicker == 1) {
                        vm.DatePicker.status.opened = true;
                    }
                },
                open2: false,
            };
            vm.dateOptions = {
                formatYear: 'yy',             
                startingDay: 1
            };
            vm.DatePicker.open = function ($event, type) {
                $event.preventDefault();
                $event.stopPropagation();
                if (!$scope.disableDatepicker == 1) {
                    vm.DatePicker[type] = true;
                }
            };

            vm.DatePicker.ChangeDate = function () {

                if ($scope.dateChange) {
                    $timeout(function () {
                        $scope.$eval($scope.dateChange());
                    });
                }
            };

            initData();

            function initData() {

                if ($scope.modelDate != null) {
                    $scope.$watch('modelDate', function () {                       
                       
                    });
                }
               

                $scope.$watch('isValidate', function () {
                    if ($scope.isValidate == 1) {
                        vm.isRequired = true;
                    }
                    else {
                        vm.isRequired = false;
                    }
                });

            }

        }],
    };

}]);
