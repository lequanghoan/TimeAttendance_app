'use strict';
app.controller('loginController', ['$scope', '$location', 'authService', 'ngAuthSettings', function ($scope, $location, authService, ngAuthSettings) {
    

    $scope.loginData = {
        userName: "",
        password: "",
        role: "",
        imageLink: "",
        version:"",
    };

    $scope.message = "";

    $scope.login = function () {

        authService.login($scope.loginData).then(function (response) {
            $location.path(response.homePage);
        },
        function (err) {
            if (err !== null && err !== undefined)
                $scope.message = err.error_description;
        });
    };

    //Get version phần mềm từ fie version text
    $.get('version.txt', function (data) {
        $scope.loginData.version = data;
    }, 'text').fail(function () {
        $scope.loginData.version = "16.0.1";
    });
}]);
