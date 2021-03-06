/* NGANGA - GENERATED CODE - BEGIN HEADER  
    Generated on 10/6/2015 at 6:41 PM
    Context: Controller      
    Controller type name: Nord.Nganga.TestConsumer.Controllers.Api.SponsorsController    
    Company: 
    Copyright: Copyright ©  2015
    Templates directory: C:\Users\mlloyd\Source\Repos\Nord.Nganga\Nord.Nganga.StEngine\templates
    Master Template version: 1.1.0 
    Body Template version: 1.1.0 
    Output signature: a5c3cccce6f0dc83fcf45e26a7e171c2    
    NGANGA - GENERATED CODE - END HEADER  
    */
rareCareNgApp.controller('sponsorsCtrl', 
  ['$scope', '$state', '$stateParams', 'commonRecordsService', 'userInfoService', 'sponsorsService',
  function($scope, $state, $stateParams, commonRecordsService, userInfoService, sponsorsService)
    {
      // Injected JavaScript at context Beginning 
      alert('Hello world!'); 
      // End injected JavaScript
      var programPeriodId = $stateParams.programPeriodId;
      $scope.common = $scope.common || {};


      $scope.common.someOtherCommonRec = commonRecordsService.getSomeOtherCommonRec(); 
      $scope.common.drugs = commonRecordsService.getDrugs(); 
      $scope.common.phoneNumberClasses = commonRecordsService.getPhoneNumberClasses(); 
      $scope.common.someCommonRecord = commonRecordsService.getSomeCommonRecord(); 


      if (programPeriodId !== null)
        {
          $scope.sponsorProgramPeriodDetailCollection = sponsorsService.getProgramPeriodSponsors(programPeriodId);

        }
      else
        {

          $scope.sponsorProgramPeriodDetailCollection = {testProperty: 'testValue'};
        }

      $scope.saveChangesToSponsorProgramPeriodDetailCollection = function()
        {
          sponsorsService.saveChangesToProgramPeriodSponsors($scope.sponsorProgramPeriodDetailCollection, function(result)
            {
              $scope.sponsorProgramPeriodDetailCollectionForm.$setPristine();

              $scope.sponsorProgramPeriodDetailCollection = result;

            })
        }


      // Injected JavaScript at context End 
      alert('Isn\'t this really annoying?'); 
      // End injected JavaScript
    }]);