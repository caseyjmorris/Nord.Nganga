(function()
  {
    var body = {
      restrict: 'E',
      scope: true,
      compile: compile,
      templateUrl: window.ngangaTemplateLocation + 'nord.modal.simple.html',
      transclude: true
    };

    function postLink(scope, el, attrs)
      {
        scope.directiveInfo = {title: attrs.title,
        id: attrs.name};
      }

    function compile()
      {
        return {post: postLink};
      }

    function provider()
      {
        return body;
      }

    ngangaUi.directive('nordModalSimple', [provider]);
  })();