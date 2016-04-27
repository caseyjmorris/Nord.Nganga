ngangaUi.directive('nordCommonSelectExpansible', ['directiveEditStateEvaluator', function(directiveEditStateEvaluator)
  {
    return {
      restrict: 'E',
      templateUrl: window.ngangaTemplateLocation + 'nord.common.select.expansible.html',
      require: '^?canEditIf',
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
            post: function(scope, el, attrs, canEditIfCtrl)
              {
                scope.isRequired = attrs.hasOwnProperty('required');
                if (!scope.hasOwnProperty('waitResolve'))
                  {
                    scope.waitResolve = true;
                  }

                scope.canEdit = function()
                  {
                    return directiveEditStateEvaluator.evaluateEditState(canEditIfCtrl, attrs);
                  }
              }
          }
        }
    }
  }]);