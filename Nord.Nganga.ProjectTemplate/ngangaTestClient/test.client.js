var ngangaTestClient = angular.module(
  'nganga.test.client', [
    'ngResource',
    'ui.utils',
    'ui.router',
    'ngCookies',
    'ui.bootstrap',
    'tableSort',
    'toaster',
    'ngAnimate',
    'nganga.ui'
  ])
  .run([
    '$rootScope', '$state', '$stateParams', function ($rootScope, $state, $stateParams)
      {
        $rootScope.$state = $state;
        $rootScope.$stateParams = $stateParams;
      }
  ]);