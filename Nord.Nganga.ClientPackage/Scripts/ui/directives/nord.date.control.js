ngangaUi.directive('nordDateControl', [function()
  {
    return {
      restrict: 'E',
      templateUrl: window.ngangaTemplateLocation + 'nord.date.control.html',
      scope: {
        ngModel: '=',
        twelfths: '@',
        endCap: '@',
        startCap: '@',
        fieldLabelText: '@'
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