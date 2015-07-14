$ngAppName$.factory('buildInfoSvc', ['$resource', '$cacheFactory', function ($resource, $cacheFactory) {
  return {
    isDebugBuild: function (handler, successFn, failFn) {
      var rsc = $resource('/$webAPIRoot$/BuildInfoProvider/IsDebugBuild', {}, { 'get': { isArray: false, cache: true } });

      var x = rsc.get({}, {}, successFn, failFn).$promise.then(handler);

      return x;
    }
  };
}]);