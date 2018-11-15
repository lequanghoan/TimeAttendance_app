'use strict';

// Declare app level module which depends on views, and components
var app = angular.module('app', [
    'ngAnimate',
//    'ngCookies',
//    'ngResource',
    'ngSanitize',
    'ngTouch',
//    'ngStorage',
    'ui.router',
    'ui.bootstrap',
    'ui.utils',
    'ui.load',
    'ui.jq',
    'oc.lazyLoad',
    'perfect_scrollbar',
    'angular-inview',
    'angular-loading-bar',
    //'cgNotify',
'ckeditor'
]);

var serviceBase = "http://localhost:2031/";
app.constant('ngAuthSettings', {
    apiServiceBaseUri: serviceBase,
    clientId: 'TimeAttendance'
});
// Directive cho fileUpload
app.directive('fileModel', ['$parse', function ($parse) {
    return {
        restrict: 'A',
        //require: 'ngModel',
        link: function (scope, element, attrs, ngModel) {
            element.require = true;
            var model = $parse(attrs.fileModel);
            var modelSetter = model.assign;

            element.bind('change', function () {
                scope.$apply(function () {
                    modelSetter(scope, element[0].files[0]);
                    //ngModel.$setViewValue(element.val());
                    //ngModel.$render();
                });

            });
        }
    };
}]);

app.directive('changeFile', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var onChangeFunc = scope.$eval(attrs.changeFile);
            element.bind('change', onChangeFunc);
        }
    };
});

app.directive("formatDate", function () {
    return {
        require: 'ngModel',
        link: function (scope, elem, attr, modelCtrl) {
            modelCtrl.$formatters.push(function (modelValue) {
                return new Date(modelValue);
            })
        }
    }
})

app.directive('datepickerLocalDate', ['$parse', function ($parse) {
    var directive = {
        restrict: 'A',
        require: ['ngModel'],
        link: link
    };
    return directive;

    function link(scope, element, attr, ctrls) {
        var ngModelController = ctrls[0];

        // called with a JavaScript Date object when picked from the datepicker
        ngModelController.$parsers.push(function (viewValue) {
            // undo the timezone adjustment we did during the formatting

            if (viewValue == null) return null;

            viewValue.setMinutes(viewValue.getMinutes() - viewValue.getTimezoneOffset());

            // we just want a local date in ISO format
            return viewValue.toISOString().substring(0, 10);
            //return viewValue;
        });

        // called with a 'yyyy-mm-dd' string to format
        ngModelController.$formatters.push(function (modelValue) {
            if (!modelValue) {
                return undefined;
            }
            // date constructor will apply timezone deviations from UTC (i.e. if locale is behind UTC 'dt' will be one day behind)
            var dt = new Date(modelValue);

            // 'undo' the timezone offset again (so we end up on the original date again)          
            dt.setMinutes(dt.getMinutes() - dt.getTimezoneOffset());
            return dt.toISOString().substring(0, 10);
            // return dt.ToddMMyyyy();
        });
    }
}]);
