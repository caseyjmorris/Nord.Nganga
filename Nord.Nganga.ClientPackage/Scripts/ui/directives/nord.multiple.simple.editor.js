(function()
  {
    var template =
      '<div class="col-md-{{twelfths}}"><label ng-class="{\'required-field-label\': isRequired}">' +
      '{{fieldLabelText}}</label><div class="panel panel-default">  <ul class="list-group"> ' +
      ' <li class="list-group-item"> ' +
      ' <div class="row"> ' +
      ' <div class="col-md-10 pseudotransclude">  </div>  ' +
      '<div class="col-md-2">  ' +
      '<button type="button" class="btn btn-danger drop-item-button">' +
      '<span class="glyphicon glyphicon-remove"></span> Remove</button>' +
      '</div>' +
      '</div>' +
      '</li></ul></div></div>';

    function getNgRepeatStatement(childFieldName, parentObj, childCollectionName)
      {
        return childFieldName + ' in ' + parentObj + '.' + childCollectionName + ' track by $index';
      }

    function getDeleteFunction(parentObj, childCollectionName)
      {
        return parentObj + '.' + childCollectionName + '.splice($index, 1)';
      }

    function compileTextFn(el, attrs, transclude)
      {
        var children = el.children();

        var templateObj = angular.element(template);

        var repeatStatement = getNgRepeatStatement(attrs.childFieldName, attrs.parentObject, attrs.collectionName);

        templateObj.find('li.list-group-item').attr('ng-repeat', repeatStatement);

        templateObj.find('div.pseudotransclude').append(children);

        templateObj.find('button.drop-item-button').attr('ng-click', getDeleteFunction(attrs.parentObject, attrs.collectionName));

        el.html('').append(templateObj);
      }

    ngangaUi.directive('nordMultipleSimpleEditor', [
      function()
        {
          return {
            restrict: 'E',
            priority: 2000,
            compile: function(element, attributes)
              {
                compileTextFn(element, attributes);
                return {}
              }
          }
        }
    ]);
  })();
