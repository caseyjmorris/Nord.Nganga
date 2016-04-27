ngangaUi.directive('nordFormSection', [function()
  {
    return {
      restrict: 'E',
      templateUrl: window.ngangaTemplateLocation + 'nord.row.html',
      transclude: true
    }
  }]);