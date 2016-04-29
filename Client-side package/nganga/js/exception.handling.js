angular.module('nganga.ui').provider('$exceptionHandler', {
  '$get': ['exceptionLoggingService', function(exceptionLoggingService)
    {
      return (exceptionLoggingService);
    }]
});

angular.module('nganga.ui').factory('exceptionLoggingService', ['$log', '$window', function($log, $window)
  {
    return function(exception, cause)
      {
        // Preserves default trace behavior.
        $log.error.apply($log, arguments);

        try
          {
            var errorMessage = exception.toString();

            if (cause)
              {
                errorMessage += '\r\n\r\n [Cause:  ' + cause + ']'
              }

            var stackTrace = $window.printStackTrace({e: exception}).join('\r\n');

            var url = $window.location.host + $window.location.pathname + $window.location.hash;

            $.ajax({
              type: 'POST',
              url: '/api/AngularException/RaiseException',
              contentType: 'application/json',
              data: angular.toJson({
                clientLocation: url,
                message: errorMessage,
                stackTrace: stackTrace
              })
            });
          }
        catch (loggingError)
          {
            $log.warn("Server-side logging was unsuccessful.");
            $log.log(loggingError);
          }
      };
  }]);