angular.module('nganga.ui').directive('nordRow', [function()
  {
    var tmpl = '<div class="row"><div class="col-md-12"></div></div>';

    function compile(el, attrs)
      {
        var children = el.children();
        var newEl = $(tmpl);
        newEl.find('.col-md-12').append(children);
        el.append(newEl);
      }

    return {
      restrict: 'E',
      //replace: true,
      compile: compile,
      scope: false,
      transclude: false
    }
  }]);