app.directive('selectAuto', [function () {
    return {
        restrict: 'E',
        scope: {
            'modelValue': '=',
            'listValue': '=',
            'id': '@',
            'name': '@',
            'isEmpty': '@',
            'onChange': '&',
            'isDisabled': '='
        },
        require: ['uiSelect'],
        templateUrl: 'app/shared/SelectAuto/selectAuto.html',
        replace: true,
        controller: ['$scope', '$timeout', function ($scope, $timeout) {
            var vm = $scope;
            vm.SelectName = "";

           // console.log("test: " + vm.isDisabled);

            vm.modelChoice = null;
            vm.choose = function (data) {
                vm.modelValue = data.selected[vm.id];
                if (vm.onChange != undefined) {
                    $timeout(function () {
                        vm.onChange();
                    }, 50);
                }
                setName(data.selected[vm.name]);
            };

            if (vm.listValue == undefined) {
                vm.listValue = [];
            }

            $scope.$watch('listValue', function () {
                if (vm.listValue.length != 0) {

                    if (vm.isEmpty == '1') {
                        var emptyObj = {};
                        emptyObj[vm.id] = "";
                        emptyObj[vm.name] = "<Tất cả>";
                        vm.listValue.unshift(emptyObj);
                    }

                    if (vm.modelValue == null || vm.modelValue == undefined) {
                        vm.modelValue = "";
                    }

                    vm.modelChoice = vm.listValue.filter(function (item) {
                        return item[vm.id] == vm.modelValue;
                    })[0];

                    if (vm.modelChoice != undefined) {
                        setName(vm.modelChoice[vm.name]);
                    }

                }
            })

            $scope.$watch('modelValue', function () {
                vm.modelChoice = vm.listValue.filter(function (item) {
                    return item[vm.id] == vm.modelValue;
                })[0];
                if (vm.modelChoice != undefined) {
                    setName(vm.modelChoice[vm.name]);
                }
            })

            function setName(name) {
                if (name != "<<Tất cả>>" && name.indexOf(">") > -1) {
                    vm.SelectName = name.substring(name.indexOf(">") + 1, name.length);
                }
                else {
                    vm.SelectName = name;
                }

            }



        }],
    };

}]);