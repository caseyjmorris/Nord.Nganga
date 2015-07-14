$ngAppName$.factory('toasterProxyService', ['toaster', function (toaster) {

  var service = {};

  var createGuid = function () {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
      var r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }

    var getter = function() {
      var localData = angular.fromJson(localStorage.getItem('toast.history'));
      return localData || [];
    };

    var putter = function(history) {
      localStorage.setItem('toast.history', angular.toJson(history));
    };

    service.pop = function(args) {
      toaster.pop(args);

      var history = getter();

      history.push(
      {
        timeStamp: new Date(),
        title: args.title,
        body: args.body,
        type: args.type,
        logId: createGuid()
      });

      putter(history);
    };

    service.clearHistory = function () {
      putter([]);
    };


    service.saveHistory = function(history) {
      putter(history);
    };

    service.getToastHistory = function () {

      return getter();
      
    }
    
    return service;
  }
])