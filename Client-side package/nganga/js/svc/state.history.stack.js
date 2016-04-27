(function()
  {
    var _state;
    var _log;

    var arr = [];

    function format(stateObj)
      {
        return stateObj.name + '(' + angular.toJson(stateObj.params) + ')';
      }

    function getCallerName(fn)
      {
        return (fn.caller.name || 'anonymous fn')
      }

    function push()
      {
        if (_state.includes('login'))
          {
            _log.debug('rejecting push of login from ' + getCallerName(push));
            return;
          }
        var stateObj = {name: _state.current.name, params: _state.params};
        _log.debug('pushing state ' + format(stateObj) + ' from ' + getCallerName(push));
        arr.push(stateObj);
      }

    function pop()
      {
        if (arr.length === 0)
          {
            _log.debug('call to stateHistoryStack.pop from ' + getCallerName(pop) + ' but no history to pop');
            return;
          }
        var result = arr.pop();
        _log.debug('popped off ' + format(result) + 'from ' + getCallerName(pop));
        return result;
      }

    function popAndGo()
      {
        var result = pop();
        if (!result)
          {
            return;
          }
        _log.debug('popAndGo redirecting to ' + format(result) + 'from ' + getCallerName(popAndGo));
        _state.go(result.name, result.params);
      }

    function reset()
      {
        arr = [];
      }

    var svc = {
      push: push,
      pop: pop,
      popAndGo: popAndGo,
      reset: reset
    };

    function provider($state, $log)
      {
        _state = $state;
        _log = $log;
        return svc;
      }

    ngangaUi.factory('stateHistoryStack', ['$state', '$log', provider]);
  })();