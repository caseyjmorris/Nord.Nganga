(function ()
  {
    var _state;

    var sidebarScopeBody = {
      twelfths: '@',
      tabs: '='
    };

    var topScopeBody = {
      tabs: '=',
      tabType: '@'
    };

    function evaluatePropertyOrFn(prop)
      {
        var t = typeof prop;
        if (t === 'function')
          {
            return prop();
          }
        else if (t === 'boolean')
          {
            return prop;
          }
        else
          {
            //I guess??

            return prop == true;
          }
      }

    function isActive(tab)
      {
        if (tab.hasOwnProperty('isActive'))
          {
            return evaluatePropertyOrFn(tab.isActive);
          }
        return _state.includes(tab.parentSref || tab.sref);
      }

    function isDisabled(tab)
      {
        if (tab.hasOwnProperty('isDisabled'))
          {
            return evaluatePropertyOrFn(tab.isDisabled);
          }
        return false;
      }

    function isHidden(tab)
      {
        if (tab.hasOwnProperty('isHidden'))
          {
            return evaluatePropertyOrFn(tab.isHidden);
          }
        return false;
      }

    function postLink(scope)
      {
        scope.isActive = isActive;
        scope.isDisabled = isDisabled;
        scope.isHidden = isHidden;

        scope.getWidth = function()
          {
            return scope.twelfths || 2;
          }
      }

    var directiveBodyCommon = {
      restrict: 'E',
      compile: function ()
        {
          return {post: postLink};
        }
    };

    var directiveBodySidebar = angular.copy(directiveBodyCommon);

    directiveBodySidebar.scope = sidebarScopeBody;
    directiveBodySidebar.templateUrl = window.ngangaTemplateLocation + 'nord.nav.sidebar.html';

    function providerSidebar($state)
      {
        _state = $state;
        return directiveBodySidebar;
      }

    var directiveBodyTop = angular.copy(directiveBodyCommon);

    directiveBodyTop.scope = topScopeBody;
    directiveBodyTop.templateUrl = window.ngangaTemplateLocation + 'nord.nav.top.html';

    function providerTop($state)
      {
        _state = $state;
        return directiveBodyTop;
      }

    angular.module('nganga.ui').directive('nordNavSidebar', [
      '$state', providerSidebar
    ]);

    angular.module('nganga.ui').directive('nordNavTop', ['$state', providerTop]);
  })();