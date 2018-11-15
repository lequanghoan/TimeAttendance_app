/**
* <copyright company="nhantinsoft.vn">
* Author: Vũ Văn Văn
* Created Date: 30/09/2017 13:08
* </copyright>
*/
'use strict';
app.controller('DashboardCtrl', ['$scope', '$http', 'DashboardService', '$uibModal',
    function ($scope, $http, DashboardService, $uibModal) {
        var vm = this;
        fnGetDataDashboard();
        function ViewChartWeek() {
            Highcharts.chart('chartWeek', {
                title: {
                    text: 'Combination chart'
                },
                xAxis: {
                    categories: ['Thứ 2', 'Thứ 3', 'Thứ 4', 'Thứ 5', 'Thứ 6', 'Thứ 7', 'Chủ nhật']
                },
                labels: {
                    items: [{
                        style: {
                            left: '50px',
                            top: '18px',
                            color: (Highcharts.theme && Highcharts.theme.textColor) || 'black'
                        }
                    }]
                },
                yAxis: {
                    title: {
                        text: 'Lần'
                    }
                },
                series: [{
                    type: 'column',
                    name: 'Đi muộn',
                    color: '#ef5350',
                    data: vm.Model.DataChartWeek.ListTotalLate
                }, {
                    type: 'column',
                    name: 'Vê sớm',
                    color: '#fdb45d',
                    data: vm.Model.DataChartWeek.ListTotalEarly
                }, {
                    type: 'column',
                    name: 'Nghỉ',
                    color: '#00797c',
                    data: vm.Model.DataChartWeek.ListTotalAbsent
                }]
            });
        }

        function ViewChartMonth() {
            Highcharts.chart('chartMonth', {
                chart: {
                    type: 'line'
                },
                title: {
                    text: 'Monthly Average Temperature'
                },
                subtitle: {
                    text: 'Source: WorldClimate.com'
                },
                xAxis: {
                    categories: vm.Model.DataChartMonth.ListCategories
                },
                yAxis: {
                    title: {
                        text: 'Lần'
                    }
                },
                plotOptions: {
                    line: {
                        dataLabels: {
                            enabled: true
                        },
                        enableMouseTracking: false
                    }
                },
                series: [{
                    name: 'Đi muộn',
                    color: '#ef5350',
                    data: vm.Model.DataChartMonth.ListTotalLate
                }, {
                    name: 'Đi sớm',
                    color: '#fdb45d',
                    data: vm.Model.DataChartMonth.ListTotalEarly
                }, {
                    name: 'Vắng mặt',
                    color: '#00797c',
                    data: vm.Model.DataChartMonth.ListTotalAbsent
                }]
            });
        }

        function ViewChartQuarter() {
            Highcharts.chart('chartQuarter', {
                chart: {
                    type: 'column'
                },
                title: {
                    text: 'Area chart with negative values'
                },
                xAxis: {
                    categories: vm.Model.DataChartQuarter.ListCategories
                },
                credits: {
                    enabled: false
                },
                yAxis: {
                    title: {
                        text: 'Lần'
                    }
                },
                series: [{
                    name: 'Đi muộn',
                    color: '#ef5350',
                    data: vm.Model.DataChartQuarter.ListTotalLate
                }, {
                    name: 'Về sớm',
                    color: '#fdb45d',
                    data: vm.Model.DataChartQuarter.ListTotalEarly
                }, {
                    name: 'Vắng mặt',
                    color: '#00797c',
                    data: vm.Model.DataChartQuarter.ListTotalAbsent
                }]
            });
        }

        //Tìm kiếm
        function fnGetDataDashboard() {
            DashboardService.GetDataDashboard().then(function (data) {
                vm.Model = data;
                ViewChartWeek();
                ViewChartMonth();
                ViewChartQuarter();
            }, function (error) {
                vm.Model = [];
                message.ShowMessage(error.message, 1);
            });
        }
    }
]);