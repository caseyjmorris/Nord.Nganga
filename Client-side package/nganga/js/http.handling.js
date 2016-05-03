(function()
  {
    var httpMonitorServiceProvider = {};

    httpMonitorServiceProvider.activeHttpCalls = 0;

    httpMonitorServiceProvider.errors = [];

    httpMonitorServiceProvider.isBusy = function()
      {
        return httpMonitorServiceProvider.activeHttpCalls > 0;
      };

    httpMonitorServiceProvider.$get = function()
      {
        var service = {};
        service.activeHttpCalls = function()
          {
            return httpMonitorServiceProvider.activeHttpCalls;
          };
        service.errors = httpMonitorServiceProvider.errors;
        service.isBusy = httpMonitorServiceProvider.isBusy;

        return service;
      };

    angular.module('nganga.ui').provider('httpMonitorService', httpMonitorServiceProvider);

    angular.module('nganga.ui').config([
      '$httpProvider', 'httpMonitorServiceProvider', function($httpProvider, httpMonitorServiceProvider)
        {
          var convertDate = function convertDate(input)
            {
              for (var key in input)
                {
                  if (!input.hasOwnProperty(key))
                    {
                      continue;
                    }

                  if (typeof input[key] === 'object' && !$.isArray(input[key]))
                    {
                      convertDate(input[key]);
                    }

                  else if ($.isArray(input[key]))
                    {
                      for (var i = 0; i < input[key].length; i++)
                        {
                          convertDate(input[key][i]);
                        }
                    }

                  else
                    {
                      if (typeof input[key] === 'string' &&
                        /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d+)?Z?$/.test(input[key]))
                        {
                          var datePart = input[key].split('T');
                          var dateSegments = datePart[0].split('-');
                          input[key] = new Date(parseInt(dateSegments[0], 10), parseInt(dateSegments[1], 10) - 1,
                            parseInt(dateSegments[2], 10));
                        }
                    }
                }

              return input;
            };

          $httpProvider.defaults.transformResponse.push(convertDate);

          $httpProvider.interceptors.push([
            '$q', '$injector', function httpPushInterceptor($q, $injector)
              {
                return {
                  'requestError': function(data)
                    {
                      httpMonitorServiceProvider.activeHttpCalls--;
                      return data;
                    },
                  'responseError': function(rejection)
                    {
                      httpMonitorServiceProvider.activeHttpCalls--;
                      console.log('Response error:  ' + rejection.status);
                      console.log(JSON.stringify(rejection));
                      httpMonitorServiceProvider.errors.push({
                        status: rejection.status,
                        statusText: rejection.statusText,
                        message: rejection.data.message,
                        url: rejection.config.url
                      });

                      if (rejection.status === 401) //unauthorized
                        {
                          if (!window.NGANGA_LOCK_AUTH_MSG)
                            {
                              $injector.get('toasterProxyService').pop({
                                type: 'info',
                                body: 'You are not logged in or not authorized to access this page.' +
                                '  Redirecting to login page.'
                              });

                              window.NGANGA_LOCK_AUTH_MSG = true;

                              var ngangaLockTimeoutMs = 10000;

                              setTimeout(function(){window.NGANGA_LOCK_AUTH_MSG = false}, ngangaLockTimeoutMs);
                            }

                          $injector.get('stateHistoryStack').push();
                          $injector.get('$state').transitionTo('login');
                        }
                      else
                        {
                          var errMsg = 'Error:  ' + rejection.data.message;

                          if (rejection.data.exceptionMessage)
                            {
                              errMsg += '  ' + rejection.data.exceptionMessage;
                            }

                          errMsg += '\r\n\r\n[' +
                            rejection.config.url + ' | ' + rejection.statusText + '(' + rejection.status + ')]';

                          $injector.get('toasterProxyService').pop({type: 'error', body: errMsg, timeout: 60000});
                        }
                      return $q.reject(rejection);
                    },
                  'request': function(config)
                    {
                      httpMonitorServiceProvider.activeHttpCalls++;
                      return config;
                    },
                  'response': function(data)
                    {
                      if (data.config.method === 'POST' && !(window.ngangaNonnotifiableUrls &&
                        window.ngangaNonnotifiableUrls.indexOf(data.config.url) !== -1))
                        {
                          $injector.get('toasterProxyService').pop({type: 'success', body:
                            'Changes saved successfully.', timeout: 5000});
                        }
                      httpMonitorServiceProvider.activeHttpCalls--;
                      return data;
                    }
                }

              }
          ]);
        }
    ]);
  })();
