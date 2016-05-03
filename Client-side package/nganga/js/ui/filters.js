angular.module('nganga.ui').filter('complexEditorMemberField', [
  'dateFilter', 'yesNoFilter', 'commonRecordFilter', function(dateFilter, yesNoFilter, commonRecordFilter)
    {

      return function(input, clientType, optionalFilterArgs)
        {

          if (clientType === 'bool')
            {
              return yesNoFilter(input);
            }

          if (clientType === 'date')
            {
              return dateFilter(input, optionalFilterArgs || 'shortDate');
            }

          if (clientType === 'selectcommon')
            {
              return commonRecordFilter(input, optionalFilterArgs);
            }

          return input;
        }
    }
]);


angular.module('nganga.ui').filter('yesNo', function()
{
  return function(input)
    {
      return input ? 'Yes' : 'No';
    }
});

angular.module('nganga.ui').filter('mustOrMustNotHave', function()
{
  return function(input)
    {
      return input ? 'Must have' : 'Must not have';
    }
});


angular.module('nganga.ui').filter('unknownAsInProcessIf', function()
{
  return function(input, condition)
    {
      return condition ? (input === 'Unknown' ? 'In-process' : input) : input;
    }
});


angular.module('nganga.ui').filter('commonRecord', function()
{
  return function(input, list)
    {
      var result = _(list).find(function(o)
      {
        return o.id == input;
      });
      if (result)
        {
          return result.name;
        }
      else
        {
          return "(Not selected)";
        }
    }
});

angular.module('nganga.ui').filter('valueOrDefault', function()
{
  return function(input, defaultText)
    {
      return input || defaultText;
    }
});

angular.module('nganga.ui').filter('meta', [
  '$filter', function($filter)
    {
      return function()
        {
          var filterName = [].splice.call(arguments, 1, 1)[0] || "filter";
          var filter = filterName.split(":");
          if (filter.length > 1)
            {
              filterName = filter[0];
              for (var i = 1, k = filter.length; i < k; i++)
                {
                  [].push.call(arguments, filter[i]);
                }
            }
          return $filter(filterName).apply(null, arguments);
        };
    }
]);

angular.module('nganga.ui').filter('iif', function()
{
  return function(input, trueValue, falseValue)
    {
      return input ? trueValue : falseValue;
    };
});