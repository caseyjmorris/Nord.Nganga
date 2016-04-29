angular.module('nganga.ui').directive('nordRowSimple', [function()
  {
    return {
      restrict: 'E',
      templateUrl: window.ngangaTemplateLocation + 'nord.row.simple.html',
      transclude: true
    }
  }]);
