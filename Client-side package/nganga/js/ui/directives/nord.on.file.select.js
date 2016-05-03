(function()
  {
    function postLink(scope, el, attrs)
      {
        function exec()
          {
            scope.$eval(attrs.nordOnFileSelect);
          }

        el.bind('change', exec);
      }

    function provider()
      {
        return {
          restrict: 'A',
          link: postLink
        }
      }

    angular.module('nganga.ui').directive('nordOnFileSelect', [provider]);
  })();