(function()
  {
    var $timeout;

    var directiveEditStateEvaluator;

    var uniqueIdService;

    var scopeObj = {
      ngModel: '=',
      commonSelectSource: '=',
      twelfths: '@',
      endCap: '@',
      startCap: '@',
      fieldLabelText: '@',
      waitResolve: '@'
    };

    function postLink(scope, el, attrs, canEditIfCtrl)
      {
        var displaySelector = false;

        scope.uid = uniqueIdService.getUniqueId();

        scope.isRequired = attrs.hasOwnProperty('required');
        if (!scope.hasOwnProperty('waitResolve'))
          {
            scope.waitResolve = true;
          }

        scope.canEdit = function()
          {
            return directiveEditStateEvaluator.evaluateEditState(canEditIfCtrl, attrs);
          };

        scope.useFiltered = function()
          {
            return attrs.hasOwnProperty('nordFiltered');
          };

        scope.displaySelector = function()
          {
            return scope.useFiltered() && displaySelector;
          };

        function moveCursorToFilter()
          {
            if (displaySelector)
              {
                var control = el.find('input.filtered-common-select-search-box')[0];

                console.log(control);

                control.focus();

                control.select();
              }
          }

        scope.toggleSelectorDisplay = function()
          {
            displaySelector = !displaySelector;
            $timeout(moveCursorToFilter);
          };

        scope.setSelected = function(item)
          {
            scope.toggleSelectorDisplay();
            scope.ngModel = item.id;
          };
      }

    function compile(el, attrs)
      {
        return {
          post: postLink
        }
      }

    function provider(_timeout, _directiveEditStateEvaluator, _uniqueIdService)
      {
        $timeout = _timeout;
        directiveEditStateEvaluator = _directiveEditStateEvaluator;
        uniqueIdService = _uniqueIdService;

        return {
          restrict: 'E',
          templateUrl: window.ngangaTemplateLocation + 'nord.common.select.html',
          scope: scopeObj,
          require: '^?canEditIf',
          compile: compile
        }
      }

    angular.module('nganga.ui').directive('nordCommonSelect', ['$timeout', 'directiveEditStateEvaluator', 'uniqueIdService', provider]);
  })();