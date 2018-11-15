app.directive('datepickerFull', [function () {
    return {
        restrict: 'E',
        scope: {
            'isValidate': '@',
            'textValidate': '@',
            'modelDate': '=',
            'disableDatepicker': '=',
        },
        templateUrl: 'app/shared/Directives/DatePicker/datepicker.html',
        replace: true,
        controller: ['$scope', function ($scope) {
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
            initData();

            function initData() {
                $scope.$watch('modelDate', function () {
                    // Chuyển dạng string về object
                    //if (typeof $scope.modelDate != 'object') {
                    //    $scope.modelDate = new Date($scope.modelDate);
                    //}

                    if ($scope.modelDate != undefined && $scope.modelDate != null) {
                        if (typeof $scope.modelDate != 'string') {
                            $scope.modelDate = addDays($scope.modelDate, 0);
                        }

                        if ($scope.modelDate.indexOf('T') !== -1) {
                            var date = $scope.modelDate;
                            $scope.modelDate = new Date(date.substring(0, 4), parseInt(date.substring(5, 7), 10) - 1, date.substring(8, 10));
                        }
                    } else {
                        if (vm.isValidate == 1) {
                            $scope.modelDate = new Date();
                        }
                    }
                });

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

    function toyyyyMMdd(date) {
        var yyyy = date.getFullYear().toString();
        var mm = (date.getMonth() + 1).toString();
        var dd = date.getDate().toString();
        return yyyy + "-" + (mm[1] ? mm : "0" + mm[0]) + "-" + (dd[1] ? dd : "0" + dd[0]);
    }

    function addDays(theDate, days) {
        var a = new Date(theDate.getTime() + days * 24 * 60 * 60 * 1000);
        return toyyyyMMdd(a);
    }

}]);
