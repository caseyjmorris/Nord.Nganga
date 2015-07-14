referrerPortalNgApp.factory('userInfoService', ['$cookies', '$state', function ($cookies, $state) {

  var avatarName = 'Guest';
  var getParsedUserInfo = function (service) {
    var userInfoCookie = $cookies.userInfo;
    if (!userInfoCookie) {
      $state.go('login');
      return { roles: [], groups: [], avatarName: avatarName }
    } else {
      var parsed = JSON.parse(userInfoCookie);
      service.user = parsed;
      service.avatarName = parsed == null ? avatarName : parsed.firstName;
      return parsed;
    }
  };

  var service = {
    user: null,
    avatarName: avatarName,
    getUserInfo: function() {
      return getParsedUserInfo(this);
    },
    userIsInRole: function (role) {
      var parsed = getParsedUserInfo(this);
      return parsed.roles && parsed.roles.indexOf(role) !== -1;
    },
    userIsInGroup: function (group) {
      var parsed = getParsedUserInfo(this);
      return parsed.groups && parsed.groups.indexOf(group) !== -1;
    },
    getRoles: function () {
      var parsed = getParsedUserInfo(this);
      return parsed.roles;
    },
    getGroups: function () {
      var parsed = getParsedUserInfo(this);
      return parsed.groups;
    }
  };

  return service;

}]);