ngangaUi.directive('nordBusySpinner', [
  'httpMonitorService', 'spinner',
  function(httpMonitorService, spinner)
    {
      return {
        restrict: 'E',
        scope: {},
        template: '<div class="spinner-element"></div>',
        compile: function(el, attrs)
          {
            var newEl = $('<div class="spinner-element"></div>');
            var sp = spinner.spin();
            var spinnerEl = $(sp.el);
            spinnerEl.attr('ng-show', 'isBusy()');
            el.replaceWith(newEl.append(sp.el));

            return {
              post: function(scope)
                {
                  scope.isBusy = function()
                    {
                      return httpMonitorService.isBusy();
                    }
                }
            }
          }
      }
    }
]);