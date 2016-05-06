angular.module('nganga.ui').directive('nordDateControl', [
  'directiveEditStateEvaluator',
  function(directiveEditStateEvaluator)
    {
      return {
        restrict: 'E',
        require: '^?canEditIf',
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
              post: function(scope, el, attrs, canEditIfCtrl)
                {
                  scope.isRequired = attrs.hasOwnProperty('required');
                  scope.$watch('ngModel', function()
                  {
                    if (typeof scope.ngModel === 'string')
                      {
                        scope.ngModel = new Date(scope.ngModel);
                      }
                  });
                  scope.canEdit = function()
                    {
                      return directiveEditStateEvaluator.evaluateEditState(canEditIfCtrl, attrs);
                    }
                }
            }
          }
      }
    }
]);