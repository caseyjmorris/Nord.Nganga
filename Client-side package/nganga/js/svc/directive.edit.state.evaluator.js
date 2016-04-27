(function()
  {
    function evaluatePropertyOrFn(prop)
      {
        var t = typeof prop;
        if (t === 'function')
          {
            return prop();
          }
        else if (t === 'boolean')
          {
            return prop;
          }
        else
          {
            //I guess??

            return prop == true;
          }
      }

    function evaluateEditState(canEditIfCtrl, attrs)
      {
        if (!attrs.hasOwnProperty('viewOnly') && (!canEditIfCtrl || canEditIfCtrl.canEditIf === undefined))
          {
            return true;
          }

        else if (attrs.hasOwnProperty('viewOnly'))
          {
            return false;
          }

        else
          {
            return evaluatePropertyOrFn(canEditIfCtrl.canEditIf);
          }
      }

    function provider()
      {
        return {
          evaluateEditState: evaluateEditState
        }
      }

    ngangaUi.factory('directiveEditStateEvaluator', [provider]);
  })();