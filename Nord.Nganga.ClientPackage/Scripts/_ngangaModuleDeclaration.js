var ngangaUi = angular.module(
  'nganga.ui', [
    'ngResource',
    'ui.utils',
    'ui.router',
    'ngCookies',
    'ui.bootstrap',
    'tableSort',
    'toaster',
    'ngAnimate'
  ])
  .run([
    '$rootScope', '$state', '$stateParams', function ($rootScope, $state, $stateParams)
      {
        $rootScope.$state = $state;
        $rootScope.$stateParams = $stateParams;
      }
  ]);