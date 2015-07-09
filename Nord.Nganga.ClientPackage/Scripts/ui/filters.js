referrerPortalNgApp.filter('yesNo', function ()
{
  return function (input)
    {
      return input ? 'Yes' : 'No';
    }
});

referrerPortalNgApp.filter('mustOrMustNotHave', function ()
{
  return function (input)
    {
      return input ? 'Must have' : 'Must not have';
    }
});


referrerPortalNgApp.filter('unknownAsInProcessIf', function () {
  return function (input, condition) {
    return condition ? (input === 'Unknown' ? 'In-process' : input) : input;
  }
});


referrerPortalNgApp.filter('commonRecord', function ()
{
  return function (input, list)
    {
      var result = _(list).find(function (o)
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