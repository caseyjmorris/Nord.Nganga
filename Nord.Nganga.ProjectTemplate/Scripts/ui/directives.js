referrerPortalNgApp.directive('nordBusySpinner', [
  function()
    {
      return {
        restrict: 'E',
        replace: true,
        transclude: false,
        compile: function(el, attrs)
          {
            var newEl = $('<div class="spinner-element"></div>');
            var sp = new Spinner().spin();
            el.replaceWith(newEl.append(sp.el));

            return function(scope, element, attributes, controller)
              {
              };
          }
      };
    }
]);

referrerPortalNgApp.directive('nordModal', [
  function()
  {
      return {
        restrict: 'E',
        templateUrl: '/ui-views/templates/modal.html',
        transclude: true,
        scope: {
          nordModalTitle: '@nordModalTitle',
          nordOnSubmit: '&nordOnSubmit',
          nordOnCancel: '&nordOnCancel',
          nordSubmitCaption: '@nordSubmitCaption',
          nordCloseCaption: '@nordCloseCaption',
          nordModalTarget: '@nordModalTarget',
          nordSubmitDisabled: '&nordSubmitDisabled'
        },
        link: function(scope, element, attrs)
          {
            // Funky code!  I'm injecting the form into the parent scope so that validation based on it will work.
            var form = element.find('form').eq(0);
            if (form != null)
              {
                scope.$parent[form.attr('name')] = form.controller('form');
            }
            //if (scope.nordSubmitDisabled) {
            //  scope.$watch(
            //    function() {
            //      var nsd = scope.nordSubmitDisabled();
            //      return nsd;
            //    },
            //    function(newVal) {
            //      element.prop('submitDisabled', newVal);
            //    }
            //  );
            //}
          }
      };
    }
]);