(function()
  {
    var beginning =
      '<div class="row"><div class="col-md-12"><div class="col-md-10"><div class="col-md-{{twelfths}}">' +
        '<label ng-class="{\'required-field-label\': isRequired}">' +
          '{{fieldLabelText}}' +
        '</label>' +

        '<div class="panel panel-default">' +
           '<ul class="list-group">' +
              '<li class="list-group-item" ng-repeat="';

    var end = '"></div>' +
      '<div class="col-md-2">' +
      '<button type="button" class="btn btn-danger" ng-click="deleteItemAt($index)">' +
      '<span class="glyphicon glyphicon-remove"></span> Delete' +
      '</button>' +
      '</div>' +
      '</li></ul></div></div></div></div>';

    function getNgRepeatStatement(childFieldName)
      {
        return childFieldName + ' in collection track by $index';
      }

    function compileTextFn(el, attrs, transclude)
      {
        var children = el.children();

        var childFieldName = attrs.childFieldName;

        var ngRepeatStatement = getNgRepeatStatement(childFieldName);

        var htmlText = beginning + ngRepeatStatement + end;

        var htmlEl = angular.element(htmlText);

        htmlEl.find('li').append(children);

        el.html('').append(htmlEl);
      }

    function postLinkFn(scope, el, attrs, ctrl, transclude)
      {
        scope.$$childHead[scope.childFieldName] = {};

        scope.parentObject[scope.collectionName] = scope.parentObject[scope.collectionName] || [];

        scope.collection = scope.parentObject[scope.collectionName];

        scope.addItem = function()
          {
            scope.collection.push({});
          };

        scope.deleteItemAt = function(index)
          {
            scope.collection.splice(index, 1);
            //set parent form dirty
          };
      }

    ngangaUi.directive('nordMultipleSimpleEditor', [
      function()
        {
          return {
            restrict: 'E',
            transclude: true,
            //templateUrl: window.ngangaTemplateLocation + 'nord.multiple.simple.editor.html',
            scope: {
              parentObject: '=',
              collectionName: '@',
              allowEdit: '@',
              childFieldName: '@',
              fieldLabelText: '@'
            },
            compile: function(element, attributes)
              {
                compileTextFn(element, attributes);
                return {
                  post: postLinkFn
                }
              }
          }
        }
    ]);
  })();
