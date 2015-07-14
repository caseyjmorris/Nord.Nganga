referrerPortalNgApp.factory('authService', [
  '$resource', '$cookies', function ($resource, $cookies)
    {
      var service = {};

      service.devHeLogin = function (successFn, failFn)
        {
          var rsc = $resource('/api/Auth/DevHeLogin');
          return rsc.save({}, {}, function (result)
          {
            $cookies.userInfo = JSON.stringify(result);
            if (successFn)
              {
                successFn(result);
              }
          }, failFn);
        };

      service.devPssLogin = function (successFn, failFn)
        {
          var rsc = $resource('/api/Auth/DevPssLogin');
          return rsc.save({}, {}, function (result)
          {
            $cookies.userInfo = JSON.stringify(result);
            if (successFn)
              {
                successFn(result);
              }
          }, failFn);
        };

      service.login = function (model, successFn, failFn)
        {
          var rsc = $resource('/api/Auth/Login');
          return rsc.save({}, model, function (result)
          {
            $cookies.userInfo = JSON.stringify(result);
            if (successFn)
              {
                successFn(result);
              }
          }, failFn);
        };

      service.logout = function (successFn, failFn)
        {
          var rsc = $resource('/api/Auth/Logout');

          return rsc.save({}, {}, function (result)
          {
            if (successFn)
              {
                successFn(result);
              }
          }, failFn);
        };

      return service;
    }
]);