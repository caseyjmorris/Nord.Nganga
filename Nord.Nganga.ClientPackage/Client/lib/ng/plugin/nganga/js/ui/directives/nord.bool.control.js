(function()
  {
    function getUid()
      {
        var S4 = function()
          {
            return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
          };
        return "el" + (S4() + S4() + S4() + S4() + S4() + S4() + S4() + S4()) + Date.now().toString();
      }

    ngangaUi.directive('nordBoolControl', [
      function()
        {
          return {
            restrict: 'E',
            templateUrl: window.ngangaTemplateLocation + 'nord.bool.control.html',
            scope: {
              ngModel: '=',
              twelfths: '@',
              fieldLabelText: '@',
              yesLabelText: '@',
              noLabelText: '@'
            },
            compile: function(el, attrs)
              {
                return {
                  post: function(scope, el, attrs)
                    {
                      scope.uid = getUid();
                      scope.isRequired = attrs.hasOwnProperty('required');
                      scope.yesLabelText = scope.yesLabelText || 'Yes';
                      scope.noLabelText = scope.noLabelText || 'No';
                    }
                }
              }
          }
        }
    ]);
  })();