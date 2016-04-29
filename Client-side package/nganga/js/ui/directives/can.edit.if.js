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

    angular.module('nganga.ui').directive('canEditIf', [provider]);
  })();