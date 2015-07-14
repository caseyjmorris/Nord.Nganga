ngangaTestClient.controller('sampleCtrl', ['$scope', function($scope)
  {
    $scope.obj = {
      childCollection: [
        {field1: 'value1', field2: 'value2'},
        {field1: 'value3', field2: 'value4'}
      ]
    };
  }]);