'use strict';

app.factory('ntsHubProxy', ['$rootScope', 'ngAuthSettings', 'authService',
  function ($rootScope, ngAuthSettings, authService) {
      var connection;
      var proxy;
      function backendFactory(hubName, startOptions) {
          if (connection && connection.id) {
              connection.stop();
          }
          connection = $.hubConnection(ngAuthSettings.apiServiceBaseUri);
          proxy = connection.createHubProxy(hubName);
          return {
              on: function (eventName, callback) {
                  proxy.on(eventName, function (result) {
                      $rootScope.$apply(function () {
                          if (callback) {
                              callback(result);
                          }
                      });
                  });
              },
              off: function (eventName, callback) {
                  proxy.off(eventName, function (result) {
                      $rootScope.$apply(function () {
                          if (callback) {
                              callback(result);
                          }
                      });
                  });
              },
              invoke: function (methodName, callback) {
                  proxy.invoke(methodName)
                      .done(function (result) {
                          $rootScope.$apply(function () {
                              if (callback) {
                                  callback(result);
                              }
                          });
                      });
              },
              start: function () {
                  connection.start().done(function () {
                      console.log('Start ok' + connection.id);
                  }).fail(function () { console.log('Could not connect'); });

              }
          };
      };

      return backendFactory;
  }]);