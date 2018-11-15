app.directive("sort", [function () {
    return {
        restrict: 'A',
        transclude: true,
        replace: true,
        scope: {
            order: '=',
            by: '=',
            reverse: '=',
            someCtrlFn: '&callbackFn',
        },
        templateUrl: "app/Shared/Directives/TableSort/tablesort.html",
        controller: ['$scope', function ($scope) {
            $scope.orderClick = function () {
                $scope.reverse = !$scope.reverse;
                $scope.someCtrlFn();
            };
        }]
    };
}]);

$(document).ready(function () {
    //sắp xếp trong table
    $('body').on('click', 'table.table-sorting th', function (e) {
        var currentth = $(e.currentTarget);
        var idtable = currentth.closest('table').attr('id');
        var indexth = currentth.index();

        var checksort = currentth.find('i.fa');
        var currentsort = checksort.attr('class');
        var asc = (currentsort == "fa fa-sort" || currentsort == "fa fa-sort-desc") ? 1 : -1;
        currentth.closest('table').find('thead tr th i').removeClass("fa-sort-desc").removeClass("fa-sort-asc").addClass("fa-sort");
        $(checksort).attr('class', "").attr('class', (asc == -1 ? "fa fa-sort-desc" : "fa fa-sort-asc"));

        $($("table[data-id='" + idtable + "']>thead th")[indexth]).click();
    });
});