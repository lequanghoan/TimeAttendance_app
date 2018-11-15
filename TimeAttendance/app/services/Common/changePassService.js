(function () {
    'use strict';

    angular.module('app').factory('changPassService', changPassService);

    // Inject some dependencies
    changPassService.$inject = ['$http', '$q', 'ngAuthSettings'];

    function changPassService($http, $q, ngAuthSettings) {
        var serviceBase = ngAuthSettings.apiServiceBaseUri;

        var changPassServiceFactory = {
            ChangePassword: changePassword,
        };

        return changPassServiceFactory;

        // Thay đổi mật khẩu
        function changePassword(userEntity) {
            var deferred = $q.defer();
            var url = serviceBase + 'api/TRC1500_InformationUser/ChangePassword';

            $http.post(url, JSON.stringify(userEntity), { headers: { 'Content-Type': 'application/json' } })
                .success(function (response) {
                    deferred.resolve(response);
                    return response;
                })
                .error(function (errMessage, statusCode) {
                    var result = { isSuccess: false, status: statusCode, message: errMessage };
                    deferred.reject(result);
                    return result;
                });

            return deferred.promise;
        }
    }
})();