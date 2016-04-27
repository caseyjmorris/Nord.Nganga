(function()
  {
    function provider(uniqueIdService, directiveEditStateEvaluator)
      {
        return {
          restrict: 'E',
          require: '^?canEditIf',
          templateUrl: window.ngangaTemplateLocation + 'nord.bool.control.html',
          scope: {
            ngModel: '=',
            twelfths: '@',
            fieldLabelText: '@',
            yesLabelText: '@',
            noLabelText: '@'
          },
          compile: function(el, attrs)
            {
              return {
                post: function(scope, el, attrs, canEditIfCtrl)
                  {
                    scope.uid = uniqueIdService.getUniqueId();
                    scope.isRequired = attrs.hasOwnProperty('required');
                    scope.yesLabelText = scope.yesLabelText || 'Yes';
                    scope.noLabelText = scope.noLabelText || 'No';
                    
                    scope.canEdit = function()
                      {
                        return directiveEditStateEvaluator.evaluateEditState(canEditIfCtrl, attrs);
                      }
                  }
              }
            }
        }
      }

    ngangaUi.directive('nordBoolControl', [ 'uniqueIdService', 'directiveEditStateEvaluator', provider]);
  })();