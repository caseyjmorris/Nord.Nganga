ngangaUi.directive('nordRow', [function()
  {
    return {
      restrict: 'E',
      templateUrl: window.ngangaTemplateLocation + 'nord.row.html',
      transclude: true
    }
  }]);