(function()
  {
    var $resource;
    var localStorageService;
    var stateHistoryStack;
    var $state;
    var toaster;
    var userInfoService;
    var $cacheFactory;
    var $rootScope;

    var logoutMsg = {
      timeOut: 5000,
      body: 'You have been logged out.  Thank you for using RareCare.',
      type: 'info'
    };

    function scrubCache()
      {
        var cache;
        var cacheInfo = $cacheFactory.info();
        for (cache in cacheInfo)
          {
            if (!cacheInfo.hasOwnProperty(cache) || cache === 'templates')
              {
                continue;
              }
            $cacheFactory.get(cacheInfo[cache].id).removeAll();
          }
      }

    function returnToPreviousIfExists()
      {
        var previous = stateHistoryStack.pop();

        if (previous)
          {
            $state.go(previous.name, previous.params);
          }
        else
          {
            // force back to whatever $urlRouteProvider.otherwise is.
            window.location = window.location.pathname;
          }
      }

    function login(model, successFn, failFn)
    {
      localStorageService.remove('userInfo');

        var rsc = $resource('/api/Auth/Login');

        return rsc.save({}, model, function(result)
        {

          localStorageService.set('userInfo', angular.toJson(result));

          $rootScope.$broadcast('loginComplete');
          
          if (successFn)
            {
              successFn(result);
            }

          returnToPreviousIfExists();
        }, failFn);
      }

    function postLogout()
      {
        localStorageService.remove('userInfo');
        toaster.pop(logoutMsg);
        scrubCache();
        stateHistoryStack.reset();
        userInfoService.reset();
        $state.go('login');
      }

    function logout(successFn, failFn)
      {
        var rsc = $resource('/api/Auth/Logout');

        return rsc.save({}, {}, function(result)
        {
          postLogout();

          $rootScope.$broadcast('logoutComplete');

          if (successFn)
            {
              successFn(result);
            }

        }, failFn);
      }

    function provider($rootScope_, $resource_, localStorageService_, stateHistoryStack_, $state_, toasterProxyService, userInfoService_, $cacheFactory_)
      {
        $resource = $resource_;
        localStorageService = localStorageService_;
        stateHistoryStack = stateHistoryStack_;
        $state = $state_;
        toaster = toasterProxyService;
        userInfoService = userInfoService_;
        $cacheFactory = $cacheFactory_;
        $rootScope = $rootScope_;

        return {
          login: login,
          logout: logout
        };
      }

    angular.module('nganga.ui').factory('authService', [
      '$rootScope', '$resource', 'localStorageService', 'stateHistoryStack', '$state', 'toasterProxyService', 'userInfoService',
      '$cacheFactory', provider
    ]);
  })();