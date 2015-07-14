ngangaUi.directive('nordNumberControl', [function()
  {
    return {
      restrict: 'E',
      templateUrl: window.ngangaTemplateLocation + 'nord.number.control.html',
      scope: {
        ngModel: '=',
        twelfths: '@',
        endCap: '@',
        startCap: '@',
        fieldLabelText: '@',
        max: '@',
        min: '@',
        step: '@'
      },
      compile: function(el, attrs)
        {
          return {
            post: function(scope, el, attrs)
              {
                scope.isRequired = attrs.hasOwnProperty('required');
              }
          }
        }
    }
  }]);