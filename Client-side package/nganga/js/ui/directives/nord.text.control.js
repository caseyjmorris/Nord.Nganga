(function()
  {
    var directiveEditStateEvaluator;

    var scopeObj = {
      ngModel: '=',
      twelfths: '@',
      endCap: '@',
      startCap: '@',
      fieldLabelText: '@',
      uiMask: '@'
    };

    function postLink(scope, el, attrs, canEditIfCtrl)
      {
        scope.isRequired = attrs.hasOwnProperty('required');

        scope.canEdit = function()
          {
            return directiveEditStateEvaluator.evaluateEditState(canEditIfCtrl, attrs);
          }
      }

    function compile(el, attrs)
      {
        return {
          post: postLink
        }
      }


    function provider(dve)
      {
        directiveEditStateEvaluator = dve;

        return {
          restrict: 'E',
          templateUrl: window.ngangaTemplateLocation + 'nord.text.control.html',
          scope: scopeObj,
          require: '^?canEditIf',
          compile: compile
        }
      }

    ngangaUi.directive('nordTextControl', ['directiveEditStateEvaluator', provider]);
  })();