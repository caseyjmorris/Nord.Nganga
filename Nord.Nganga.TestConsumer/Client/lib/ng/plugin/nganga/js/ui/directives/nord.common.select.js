ngangaUi.directive('nordCommonSelect', [function()
  {
    return {
      restrict: 'E',
      templateUrl: window.ngangaTemplateLocation + 'nord.date.control.html',
      scope: {
        ngModel: '=',
        commonSelectSource: '=',
        twelfths: '@',
        endCap: '@',
        startCap: '@',
        fieldLabelText: '@',
        waitResolve: '@'
      },
      compile: function(el, attrs)
        {
          return {
            post: function(scope, el, attrs)
              {
                scope.isRequired = attrs.hasOwnProperty('required');
                if (!scope.hasOwnProperty('waitResolve'))
                  {
                    scope.waitResolve = true;
                  }
              }
          }
        }
    }
  }]);