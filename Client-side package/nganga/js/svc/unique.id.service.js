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

    function provider()
      {
        return {
          getUniqueId: function()
            {
              return getUid();
            }
        }
      }

    ngangaUi.factory('uniqueIdService', [provider])
  })();