app.directive('permission', ['authService', function (authService) {
    return {
        restrict: 'A',
        scope: {
            permission: '='
        },

        link: function (scope, elem, attrs) {
            scope.$watch(authService.authentication.isAuth, function () {
                var isAuthorize = false;
                var allowFeatureList = scope.permission;
                var listPermission = authService.authentication.permission != null ? JSON.parse(authService.authentication.permission) : null;
                if (listPermission != null && listPermission.length > 0) {
                    $.each(allowFeatureList, function (i, item) {
                        $.each(listPermission, function (j, itemP) {
                            if (item.trim() == itemP) {
                                isAuthorize = true;
                            }
                        });
                    });
                }
                if (!isAuthorize)
                    elem.remove();
            });
        }
    }
}]);