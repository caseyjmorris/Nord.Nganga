(function()
  {
    function provider()
      {
        return {
          restrict: 'A',
          scope: {canEditIf: '='},
          controller: ['$scope', function($scope)
            {
              this.canEditIf = $scope.canEditIf;
            }]
        };
      }

    ngangaUi.directive('canEditIf', [provider]);
  })();