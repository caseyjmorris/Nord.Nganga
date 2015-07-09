ngangaUi.directive('nordMultipleComplexEditor', ['$parse', function($parse)
  {
    return {
      restrict: 'E',
      transclude: false,
      templateUrl: window.ngangaTemplateLocation + 'nord.multiple.complex.editor.html',
      scope: {
        parentObject: '=',
        collectionName: '@',
        allowEdit: '@',
        additionalButtons: '@',
        fieldDefinitions: '@'
      },
      post: function(scope, element)
        {
          alert(JSON.stringify(scope.fieldDefinitions));
          scope.activeItem = {};
          scope.activeItemIndex = null;
          scope.additionalButtonsValues = $parse(element.additionalButtons);
          scope.fieldDefinitionValues = $parse(element.fieldDefinitions);
          element.parentObject[element.collectionName] = element.parentObject[element.collectionName] || [];

          scope.collection = element.parentObject[element.collectionName];

          scope.addItem = function()
            {
              scope.activeItem = {};
              scope.activeItemIndex = null;
              //show control
            };

          scope.editItemAt = function(index)
            {
              scope.activeItem = {};
              scope.activeItemIndex = index;
              $.extend(scope.activeItem, scope.collection[index]);
              //show child form
            };

          scope.deleteItemAt = function(index)
            {
              scope.collection.splice(index, 1);
              //set parent form dirty
            };

          scope.applyActiveItemChanges = function()
            {
              if (scope.activeItemIndex === null)
                {
                  scope.collection.push(scope.activeItem);
                }
              else
                {
                  scope.collection[scope.activeItemIndex] = scope.activeItem;
                }
              //close child form
              //set child form pristine
              //set parent form dirty
            }
        }
    }
  }]);