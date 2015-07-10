ngangaUi.directive('nordMultipleComplexEditor', ['$parse', function($parse)
  {
    return {
      restrict: 'E',
      transclude: true,
      templateUrl: window.ngangaTemplateLocation + 'nord.multiple.complex.editor.html',
      scope: {
        parentObject: '=',
        collectionName: '@',
        allowEdit: '@',
        additionalButtons: '@',
        fieldDefinitions: '@',
        childFieldName: '@',
        panelTopLabel: '@',
        childFormName: '@'
      },
      compile: function(element, attributes)
        {
          return {
            post: function(scope, element, attrs, controller, transclude)
              {
                window.dirScope = scope;
                scope.$$childHead[scope.childFieldName] = {};
                scope.activeItemIndex = null;
                //scope.additionalButtonsValues = $parse(element.additionalButtons).call();
                scope.fieldDefinitionValues = $parse(scope.fieldDefinitions)();
                scope.parentObject[scope.collectionName] = scope.parentObject[scope.collectionName] || [];

                scope.collection = scope.parentObject[scope.collectionName];

                scope.addItem = function()
                  {
                    scope.$$childHead[scope.childFieldName] = {};
                    scope.activeItemIndex = null;
                    //show control
                  };

                scope.editItemAt = function(index)
                  {
                    console.log(index);
                    scope.$$childHead[scope.childFieldName] = {};
                    scope.activeItemIndex = index;
                    $.extend(scope.$$childHead[scope.childFieldName], scope.collection[index]);
                    //show child form
                  };

                scope.deleteItemAt = function(index)
                  {
                    console.log(index);
                    scope.collection.splice(index, 1);
                    //set parent form dirty
                  };

                scope.applyActiveItemChanges = function()
                  {
                    if (scope.activeItemIndex === null)
                      {
                        scope.collection.push(scope.$$childHead[scope.childFieldName]);
                      }
                    else
                      {
                        scope.collection[scope.activeItemIndex] = scope.$$childHead[scope.childFieldName];
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