/// <reference path="F:\01.Project\2016-CFTD-QLGSGT\02. SourceCode\01. Center\TimeAttendance\TimeAttendance\js/thread/underscore-min.js" />
/// <reference path="viewen/FusionChart.js" />
/// <reference path="views/FusionChart.js" />
/// <reference path="views/FusionChart.js" />
'use strict';

/**
 * Config for the router
 */
angular.module('app')
    .config(
    ['$stateProvider', '$urlRouterProvider', 'JQ_CONFIG',
        function ($stateProvider, $urlRouterProvider, JQ_CONFIG) {

            $urlRouterProvider
                .otherwise('/login');

            $stateProvider
                .state('app', {
                    //abstract: true,
                    url: '/app',
                    controller: 'homeController',
                    data: { requireLogin: true },
                    templateUrl: 'app/views/Common/home.html',
                    resolve: {
                        deps: ['$ocLazyLoad',
                            function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'app/controllers/Common/homeController.js',
                                ]);
                            }
                        ]
                    }
                })
                .state('login', {
                    templateUrl: 'app/views/Common/login.html',
                    url: '/login',
                    controller: 'loginController',
                    data: { requireLogin: false },
                    resolve: {
                        deps: ['$ocLazyLoad',
                            function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'app/controllers/Common/loginController.js',
                                ]);
                            }
                        ]
                    }
                })
                //Nhóm người dùng
                .state('app.group-user-list', {
                    url: '/group-user-list',
                    data: { requireLogin: true },
                    templateUrl: 'app/views/NTS0102GroupUser/NTS0102List.html',
                    controller: 'NTS0102ListCtrl',
                    controllerAs: 'vm',
                    resolve: {
                        deps: ['$ocLazyLoad',
                            function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'app/controllers/NTS0102GroupUser/NTS0102ListController.js',
                                    'app/services/NTS0102GroupUser/NTS0102Service.js',
                                     'js/filters/pages-startfrom.js',
                                ]);
                            }
                        ]
                    }
                })
                .state('app.group-user-create', {
                    url: '/group-user-create',
                    data: { requireLogin: true },
                    templateUrl: 'app/views/NTS0102GroupUser/NTS0102Create.html',
                    controller: 'NTS0102CreateCtrl',
                    controllerAs: 'vm',
                    resolve: {
                        deps: ['$ocLazyLoad',
                            function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'app/controllers/NTS0102GroupUser/NTS0102CreateController.js',
                                    'app/services/NTS0102GroupUser/NTS0102Service.js'
                                ]);
                            }
                        ]
                    }
                })
                .state('app.group-user-update', {
                    data: { requireLogin: true },
                    url: '/group-user-update/:id',
                    params: {
                        id: 'id',
                    },
                    templateUrl: 'app/views/NTS0102GroupUser/NTS0102Update.html',
                    controller: 'NTS0102UpdateCtrl',
                    controllerAs: 'vm',
                    resolve: {
                        deps: ['$ocLazyLoad',
                            function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'app/controllers/NTS0102GroupUser/NTS0102UpdateController.js',
                                    'app/services/NTS0102GroupUser/NTS0102Service.js'
                                ]);
                            }
                        ]
                    }
                })

                .state('app.Department', {
                    data: { requireLogin: true },
                    url: '/Department/:id',
                    params: {
                        id: 'id',
                    },
                    templateUrl: 'app/views/Department/Department001.html',
                    controller: 'Department001Ctrl',
                    controllerAs: 'vm',
                    resolve: {
                        deps: ['$ocLazyLoad',
                            function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'app/controllers/Department/Department001Controller.js',
                                    'app/services/Department/DepartmentService.js'
                                ]);
                            }
                        ]
                    }
                })
                .state('app.AttendanceLog', {
                    data: { requireLogin: true },
                    url: '/AttendanceLog/:id',
                    params: {
                        id: 'id',
                    },
                    templateUrl: 'app/views/AttendanceLog/ListAttendanceLog.html',
                    controller: 'AttendanceLogCtrl',
                    controllerAs: 'vm',
                    resolve: {
                        deps: ['$ocLazyLoad',
                            function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'app/controllers/AttendanceLog/AttendanceLogController.js',
                                    'app/services/AttendanceLog/AttendanceLogService.js'
                                ]);
                            }
                        ]
                    }
                })
                                .state('app.NoAttendanceLog', {
                                    data: { requireLogin: true },
                                    url: '/NoAttendanceLog/:id',
                                    params: {
                                        id: 'id',
                                    },
                                    templateUrl: 'app/views/NoAttendanceLog/NoAttendanceLog.html',
                                    controller: 'NoAttendanceLogCtrl',
                                    controllerAs: 'vm',
                                    resolve: {
                                        deps: ['$ocLazyLoad',
                                            function ($ocLazyLoad) {
                                                return $ocLazyLoad.load([
                                                    'app/controllers/NoAttendanceLog/NoAttendanceLogController.js',
                                                    'app/services/AttendanceLog/AttendanceLogService.js',
                                                    'app/services/NTS0301Report/NTS0301Service.js',
                                                ]);
                                            }
                                        ]
                                    }
                                })


                                .state('app.job-title', {
                                    data: { requireLogin: true },
                                    url: '/job-title/:id',
                                    params: {
                                        id: 'id',
                                    },
                                    templateUrl: 'app/views/JobTitle/JobTitle001.html',
                                    controller: 'JobTitle001Ctrl',
                                    controllerAs: 'vm',
                                    resolve: {
                                        deps: ['$ocLazyLoad',
                                            function ($ocLazyLoad) {
                                                return $ocLazyLoad.load([
                                                    'app/controllers/JobTitle/JobTitle001Controller.js',
                                                    'app/services/JobTitle/JobTitle001Service.js'
                                                ]);
                                            }
                                        ]
                                    }
                                })

                //Người dùng
            .state('app.user-list', {
                url: '/user-list',
                data: { requireLogin: true },
                templateUrl: 'app/views/NTS0101User/NTS0101List.html',
                controller: 'NTS0101ListCtrl',
                controllerAs: 'vm',
                resolve: {
                    deps: ['$ocLazyLoad',
                        function ($ocLazyLoad) {
                            return $ocLazyLoad.load([
                                'app/controllers/NTS0101User/NTS0101ListController.js',
                                'app/services/NTS0101User/NTS0101Service.js',
                                 'js/filters/pages-startfrom.js',
                            ]);
                        }
                    ]
                }
            })
                .state('app.user-create', {
                    url: '/user-create',
                    data: { requireLogin: true },
                    templateUrl: 'app/views/NTS0101User/NTS0101Create.html',
                    controller: 'NTS0101CreateCtrl',
                    controllerAs: 'vm',
                    resolve: {
                        deps: ['$ocLazyLoad',
                            function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'app/controllers/NTS0101User/NTS0101CreateController.js',
                                    'app/services/NTS0101User/NTS0101Service.js'
                                ]);
                            }
                        ]
                    }
                })
                .state('app.user-update', {
                    url: '/user-update/:id',
                    data: { requireLogin: true },
                    params: {
                        id: 'id',
                    },
                    templateUrl: 'app/views/NTS0101User/NTS0101Update.html',
                    controller: 'NTS0101UpdateCtrl',
                    controllerAs: 'vm',
                    resolve: {
                        deps: ['$ocLazyLoad',
                            function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'app/controllers/NTS0101User/NTS0101UpdateController.js',
                                    'app/services/NTS0101User/NTS0101Service.js'
                                ]);
                            }
                        ]
                    }
                })
                //ql nhân viên
                .state('app.employee-list', {
                    url: '/employee-list',
                    data: { requireLogin: true },
                    templateUrl: 'app/views/NTS0201Employee/NTS0201List.html',
                    controller: 'NTS0201ListCtrl',
                    controllerAs: 'vm',
                    resolve: {
                        deps: ['$ocLazyLoad',
                            function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'app/controllers/NTS0201Employee/NTS0201ListController.js',
                                    'app/services/NTS0201Employee/NTS0201Service.js',
                                     'js/filters/pages-startfrom.js',
                                ]);
                            }
                        ]
                    }
                })
                 //thống kê
                .state('app.report-list', {
                    url: '/report-list',
                    data: { requireLogin: true },
                    templateUrl: 'app/views/NTS0301Report/NTS0301List.html',
                    controller: 'NTS0301ListCtrl',
                    controllerAs: 'vm',
                    resolve: {
                        deps: ['$ocLazyLoad',
                            function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'app/controllers/NTS0301Report/NTS0301ListController.js',
                                    'app/services/NTS0301Report/NTS0301Service.js',
                                     'js/filters/pages-startfrom.js',
                                ]);
                            }
                        ]
                    }
                })
                        //thống kê
                .state('app.tranlog-list', {
                    url: '/tranlog-list',
                    data: { requireLogin: true },
                    templateUrl: 'app/views/NTS0301Report/NTS0302List.html',
                    controller: 'NTS0302ListCtrl',
                    controllerAs: 'vm',
                    resolve: {
                        deps: ['$ocLazyLoad',
                            function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'app/controllers/NTS0301Report/NTS0302ListController.js',
                                    'app/services/NTS0301Report/NTS0301Service.js',
                                     'js/filters/pages-startfrom.js',
                                ]);
                            }
                        ]
                    }
                })
                  //thống kê
                .state('app.density-list', {
                    url: '/density-list',
                    data: { requireLogin: true },
                    templateUrl: 'app/views/NTS0301Report/NTS0303List.html',
                    controller: 'NTS0303ListCtrl',
                    controllerAs: 'vm',
                    resolve: {
                        deps: ['$ocLazyLoad',
                            function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'app/controllers/NTS0301Report/NTS0303ListController.js',
                                    'app/services/NTS0301Report/NTS0301Service.js',
                                     'js/filters/pages-startfrom.js',
                                ]);
                            }
                        ]
                    }
                })
                //Xem lich sử theo user
            .state('app.log-list', {
                url: '/log-list/:id',
                data: { requireLogin: true },
                params: {
                    id: '',
                },
                templateUrl: 'app/views/NTS0208Log/NTS0208List.html',
                controller: 'NTS0208ListCtrl',
                controllerAs: 'vm',
                resolve: {
                    deps: ['$ocLazyLoad',
                        function ($ocLazyLoad) {
                            return $ocLazyLoad.load([
                                'app/controllers/NTS0208Log/NTS0208ListController.js',
                                'app/services/NTS0208Log/NTS0208Service.js'
                            ]);
                        }
                    ]
                }
                })
                .state('app.dashboard', {
                    url: '/dashboard',
                    data: { requireLogin: true },
                    params: {
                        id: '',
                    },
                    templateUrl: 'app/views/Dashboard/DashboardHome.html',
                    controller: 'DashboardCtrl',
                    controllerAs: 'vm',
                    resolve: {
                        deps: ['$ocLazyLoad',
                            function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'app/controllers/Dashboard/DashboardController.js',
                                    'app/services/Dashboard/DashboardService.js'
                                ]);
                            }
                        ]
                    }
                });
        }
    ]
);
