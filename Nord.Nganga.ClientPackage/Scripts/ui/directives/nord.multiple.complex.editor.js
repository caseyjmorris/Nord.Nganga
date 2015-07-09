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
      compile: function(element, attributes)
        {
          return {
            post: function(scope, element)
              {
                window.parser = $parse;
                scope.activeItem = {};
                scope.activeItemIndex = null;
                //scope.additionalButtonsValues = $parse(element.additionalButtons).call();
                scope.fieldDefinitionValues = $parse(scope.fieldDefinitions)();
                scope.parentObject[scope.collectionName] = scope.parentObject[scope.collectionName] || [];

                scope.collection = scope.parentObject[scope.collectionName];

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
        }

    }
  }]);