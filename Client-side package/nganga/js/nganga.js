var ngangaUi = angular.module(
  'nganga.ui', [
    'ngResource',
    'ui.utils',
    'ui.router',
    'ngCookies',
    'ui.bootstrap',
    'tableSort',
    'toaster',
    'ngAnimate',
    'LocalStorageModule'
  ])
  .config(['$compileProvider', function($compileProvider)
    {
      $compileProvider.aHrefSanitizationWhitelist(/^\s*(https?|ftp|mailto|data):/);
    }])
  .run([
    '$rootScope', '$state', '$stateParams', function ($rootScope, $state, $stateParams)
      {
        $rootScope.$state = $state;
        $rootScope.$stateParams = $stateParams;
      }
  ]);

window.ngangaTemplateLocation = '/client/lib/ng/plugin/nganga/js/templates/';