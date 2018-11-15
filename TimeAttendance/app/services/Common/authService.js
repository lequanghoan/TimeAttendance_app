'use strict';
app.factory('authService', ['$http', '$q', '$state', 'localStorageService', 'ngAuthSettings', '$location', function ($http, $q, $state, localStorageService, ngAuthSettings, $location) {

    var serviceBase = ngAuthSettings.apiServiceBaseUri;
    var authServiceFactory = {
        logOut: _logOut,
    };

    var _authentication = {
        isAuth: false,
        userName: "",
        userFullName: "",
        useRefreshTokens: true,
        userid: "",
        authorizestring: "",
        authorizeitemsstring: "",
        permission: "",
        imageLink: "",
       // id: "",
      //  code: "",
      //  unitid: "",
        type:"",
       // level: "",
        groupid: "",
       // agency: "",
       // IsAdmin: "",
        securityKey: "",
    };

    var _login = function (loginData) {
        var data = "grant_type=password&username=" + loginData.userName + "&password=" + loginData.password + "&client_id=" + ngAuthSettings.clientId;

        _logOut();
        // Tao moi mot doi tuong deferred, bao gom cac phuong thuc resolve(), reject(), notify()
        // Defferred co mot thuong tinh promise 
        var deferred = $q.defer();

        $http.post(serviceBase + 'token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(function (response) {


            localStorageService.set('authorizationData',
                {
                    token: response.access_token,
                    userName: loginData.userName,
                    userid: response.userid,
                    userFullName: loginData.FullName,
                    refreshToken: response.refresh_token,
                    useRefreshTokens: true,
                    authorizestring: response.authorizestring,
                    authorizeitemsstring: response.authorizeitemsstring,
                    permission: response.permission,
                    imageLink: response.imageLink,
                  //  id: response.userid,
                   // code: response.code,
                   // unitid: response.unitid,
                    type: response.type,
                   // level: response.level,
                    groupid: response.groupid,
                   // agency: response.agency,
                    //IsAdmin:response.IsAdmin,
                    securityKey:response.securityKey,
                    
                });

            _authentication.isAuth = true;
            _authentication.userName = loginData.userName;
            _authentication.useRefreshTokens = true;
            _authentication.permission = response.permission;
            _authentication.imageLink = response.imageLink;
           // _authentication.id = response.userid;
          //  _authentication.code = response.code;
            _authentication.userid = response.userid;
          //  _authentication.unitid = response.unitid;
            _authentication.type = response.type;
            // _authentication.IsAdmin = response.IsAdmin;
            _authentication.securityKey = response.securityKey;
            
           // _authentication.level = response.level;
            _authentication.groupid = response.groupid,
            _authentication.userFullName = response.userfullname;
           // _authentication.agency = response.agency;

            deferred.resolve(response);

        }).error(function (err, status) {
            _logOut();
            deferred.reject(err);
        });

        return deferred.promise;

    };

    var _logOut = function () {
        localStorage.removeItem('firstLogin');

        localStorageService.remove('authorizationData');
        _authentication.isAuth = false;
        _authentication.userName = "";
        _authentication.userid = "";
        _authentication.permission = "";
        _authentication.userFullName = "";
        _authentication.useRefreshTokens = true;
        _authentication.authorizestring = "";
        _authentication.authorizeitemsstring = "";
       // _authentication.id = "";
       // _authentication.code = "";
      //  _authentication.unitid = "";
        _authentication.type = "";
        //   _authentication.IsAdmin = "";
        _authentication.securityKey = "";
   //     _authentication.level = "";
        _authentication.groupid = "",
       // _authentication.agency = "",
        $state.go('login');
    };

    var _fillAuthData = function () {

        var authData = localStorageService.get('authorizationData');
        if (authData) {
            _authentication.isAuth = true;
            _authentication.userName = authData.userName;
            _authentication.permission = authData.permission;
            _authentication.imageLink = authData.imageLink;
         //   _authentication.id = authData.id;
          //  _authentication.code = authData.code;
            _authentication.userFullName = authData.userFullName;
            _authentication.useRefreshTokens = authData.useRefreshTokens;
            _authentication.userid = authData.userid;
            _authentication.authorizestring = authData.authorizestring;
            _authentication.authorizeitemsstring = authData.authorizeitemsstring;
          //  _authentication.unitid = authData.unitid;
            _authentication.type = authData.type;
            //  _authentication.IsAdmin = authData.IsAdmin;
            _authentication.securityKey = authData.securityKey;
            
          //  _authentication.level = authData.level;
            _authentication.groupid = authData.groupid;
          //  _authentication.agency = authData.agency;

            _authentication.userFullName = authData.userfullname;
        }

    };

    authServiceFactory.login = _login;
    authServiceFactory.logOut = _logOut;
    authServiceFactory.fillAuthData = _fillAuthData;
    authServiceFactory.authentication = _authentication;

    return authServiceFactory;
}]);