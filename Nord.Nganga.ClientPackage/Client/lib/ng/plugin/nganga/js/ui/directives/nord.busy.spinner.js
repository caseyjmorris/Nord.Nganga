ngangaUi.directive('nordBusySpinner', [ 'spinner',
  function(spinner)
    {
      return {
        restrict: 'E',
        replace: true,
        transclude: false,
        compile: function(el, attrs)
          {
            var newEl = $('<div class="spinner-element"></div>');
            var sp = spinner.spin();
            el.replaceWith(newEl.append(sp.el));

            return function(scope, element, attributes, controller)
              {
              };
          }
      };
    }
]);