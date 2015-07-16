ngangaUi.directive('nordModal', [
  function()
    {
      return {
        restrict: 'E',
        templateUrl: window.ngangaTemplateLocation + 'modal.html',
        transclude: true,
        scope: {
          nordModalTitle: '@nordModalTitle',
          nordOnSubmit: '&nordOnSubmit',
          nordOnCancel: '&nordOnCancel',
          nordSubmitCaption: '@nordSubmitCaption',
          nordCloseCaption: '@nordCloseCaption',
          nordModalTarget: '@nordModalTarget',
          nordSubmitDisabled: '&nordSubmitDisabled',
        },
        link: function(scope, element, attrs)
          {
            // Funky code!  I'm injecting the form into the parent scope so that validation based on it will work.
            var form = element.find('form').eq(0);
            if (form != null)
              {
                scope.$parent[form.attr('name')] = form.controller('form');
              }
          }
      };
    }
]);