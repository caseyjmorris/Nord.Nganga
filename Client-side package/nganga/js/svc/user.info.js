ngangaUi.factory('userInfoService', [
  'localStorageService', 'stateHistoryStack', '$state',
  function(localStorageService, stateHistoryStack, $state)
    {
      var parsed;
      var defaultUser = {roles: [], groups: [], firstName: 'Guest', isDefault: true, userName: 'guest'};
      var blockedUrls = ['/resetPassword/', '/setPassword/'];

      function initParsed()
        {
          if (parsed)
            {
              return;
            }

          var userInfoCookie = localStorageService.get('userInfo');

          parsed = angular.fromJson(userInfoCookie);

          if (!parsed)
            {
              var urlPrefix = $state.$current.url.prefix;

              if (blockedUrls.indexOf(urlPrefix) !== -1)
                {
                  return;
                }

              stateHistoryStack.push();

              $state.go('login');

              return;
            }

          parsed.isDefault = false;
        }

      function getUserOrDefault()
        {
          initParsed();
          return parsed ? parsed : defaultUser;
        }

      function reset()
        {
          parsed = undefined;
        }


      return {
        getUserOrDefault: getUserOrDefault,
        userIsInRole: function(role)
          {
            initParsed();
            return parsed && parsed.roles && parsed.roles.indexOf(role) !== -1;
          },
        userIsInGroup: function(group)
          {
            initParsed();
            return parsed && parsed.groups && parsed.groups.indexOf(group) !== -1;
          },
        getRoles: function()
          {
            initParsed();
            return parsed ? parsed.roles : [];
          },
        getGroups: function()
          {
            initParsed();
            return parsed ? parsed.groups : [];
          },
        getUserInfo: function()
          {
            initParsed();
            return parsed;
          },
        reset: reset
      };
    }
]);