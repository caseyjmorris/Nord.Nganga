(function()
  {
    function provider(toaster)
      {
        var service = {};

        function createGuid()
          {
            return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c)
            {
              var r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
              return v.toString(16);
            });
          }

        function getter()
          {
            var localData = angular.fromJson(localStorage.getItem('toast.history'));
            return localData || [];
          }

        function putter(history)
          {
            localStorage.setItem('toast.history', angular.toJson(history));
          }

        service.pop = function(args)
          {
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

        service.clearHistory = function()
          {
            putter([]);
          };


        service.saveHistory = function(history)
          {
            putter(history);
          };

        service.getToastHistory = function()
          {

            return getter();

          };

        return service;
      }

    var dependencies = ['toaster', provider];

    angular.module('nganga.ui').factory('toasterProxyService', dependencies);
  })();