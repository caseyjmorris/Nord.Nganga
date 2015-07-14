ngangaUi.directive('nordBasicScalar', [function()
  {
    return {
      restrict: 'E',
      transclude: true,
      templateUrl: window.ngangaTemplateLocation + 'nord.basic.scalar.html',
      scope: {
        twelfths: '@',
        fieldLabelText: '@'
      },
      compile: function(el, attrs) {
        return {
          post: function(scope, el, attrs)
            {
              scope.isRequired = attrs.hasOwnProperty('required');
            }
        }
      }
    }
  }]);