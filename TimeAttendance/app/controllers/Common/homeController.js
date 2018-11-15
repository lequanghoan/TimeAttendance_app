'use strict';
app.controller('homeController', ['$scope', '$location', 'authService', '$uibModal', 'notify', 'ngAuthSettings', 'common',
    function ($scope, $location, authService, $uibModal, notify, ngAuthSettings, common) {
        $scope.fnLogOut = function () {
            authService.logOut();
            $location.path('/login');
        }
        $scope.serviceBase = ngAuthSettings.apiServiceBaseUri;
        $scope.authentication = authService.authentication;
        $scope.NoImage = common.NoImage;
        $scope.NoAvatar = common.NoAvatar;

        $scope.FnChangePassword = fnChangePassword;
        $scope.FnUserProfiles = fnUserProfiles;
        $scope.FnShowModalCommon = fnShowModalCommon;

        $scope.FnShowConfig = fnShowConfig;

        //$scope.CheckPermisstion = fnCheckPermisstion;

        function fnShowConfig() {
            var modalInstance = $uibModal.open({
                templateUrl: 'app/views/Common/config.html',
                controller: 'ConfigCtrl',
                controllerAs: 'vmconfig',
                windowClass: 'app-modal',
                resolve: {
                    deps: ['$ocLazyLoad',
                        function ($ocLazyLoad) {
                            return $ocLazyLoad.load([
                                'app/controllers/Common/ConfigController.js',
                                'app/services/Common/ConfigService.js',
                            ])
                        }
                    ]
                }
            });
            modalInstance.result.then(function (rs) {
                if (rs) {
                    notify('Thay đổi mật khẩu thành công');
                    authService.logOut();
                    $location.path('/login');
                }
            }, function () {
            });
        }

        function fnChangePassword() {
            var modalInstance = $uibModal.open({
                templateUrl: 'app/views/PCTP0205ChangePassword/PCTP0205Change.html',
                controller: 'PCTP0205ChangeCtrl',
                controllerAs: 'vm',
                windowClass: 'app-modal',
                resolve: {
                    deps: ['$ocLazyLoad',
                        function ($ocLazyLoad) {
                            return $ocLazyLoad.load([
                                'app/controllers/PCTP0205ChangePassword/PCTP0205ChangeController.js',
                                'app/services/PCTP0205ChangePassword/PCTP0205Service.js',
                            ])
                        }
                    ]
                }
            });
            modalInstance.result.then(function (rs) {
                if (rs) {
                    notify('Thay đổi mật khẩu thành công');
                    authService.logOut();
                    $location.path('/login');
                }
            }, function () {
            });
        }
        function fnUserProfiles() {
            var modalInstance = $uibModal.open({
                templateUrl: 'app/views/PCTP0207UpdateUserInfo/PCTP0207Update.html',
                controller: 'PCTP0207UpdateCtrl',
                controllerAs: 'vm',
                windowClass: 'app-modal',
                resolve: {
                    deps: ['$ocLazyLoad',
                        function ($ocLazyLoad) {
                            return $ocLazyLoad.load([
                                'app/controllers/PCTP0207UpdateUserInfo/PCTP0207UpdateController.js',
                                'app/services/PCTP0207UpdateUserInfo/PCTP0207Service.js',
                            ]);
                        }
                    ], items: function () {
                        return {};
                    }
                }
            });
            modalInstance.result.then(function (rs) {
                if (rs)
                    notify('Thay đổi thông tin cá nhân thành công');
            }, function () {
            });
        }

        //function fnCheckPermisstion(allowFeature) {
        //    var isAuthorize = false;
        //    var allowFeatureList = allowFeature.split(';');
        //    var listPermission = authService.authentication.permission != null ? JSON.parse(authService.authentication.permission) : null;
        //    if (listPermission != null && listPermission.length > 0) {
        //        $.each(allowFeatureList, function (i, item) {
        //            $.each(listPermission, function (j, itemP) {
        //                if (item.trim() == itemP) {
        //                    isAuthorize = true;
        //                }
        //            });
        //        });
        //    }
        //    return isAuthorize;
        //};

        function fnCheckObjectEmpty(obj) {
            for (var prop in obj) {
                if (obj.hasOwnProperty(prop))
                    return false;
            }

            return JSON.stringify(obj) === JSON.stringify({});
        }

        //Show modal
        function fnShowModalCommon(folder, filename) {
            var modalInstance = $uibModal.open({
                templateUrl: 'app/views/' + folder + '/' + filename + '.html',
                //controller: 'PCTP0103CreateCtrl',
                controllerAs: 'vm',
                windowClass: 'app-modal',
                resolve: {
                    deps: ['$ocLazyLoad',
                        function ($ocLazyLoad) {
                            return $ocLazyLoad.load([
                                //'app/controllers/PCTP0103Unit/PCTP0103CreateController.js',
                                //'app/services/PCTP0103Unit/PCTP0103Service.js',
                            ]);
                        }
                    ]
                }
            });
        }
    }]);

