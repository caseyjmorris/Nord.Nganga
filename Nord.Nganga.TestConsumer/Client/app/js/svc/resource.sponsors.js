/* NGANGA - GENERATED CODE - BEGIN HEADER  
    Generated on 9/2/2015 at 7:21 PM
    Context: Resource      
    Controller type name: Nord.Nganga.TestConsumer.Controllers.Api.SponsorsController    
    Company: 
    Copyright: Copyright ©  2015
    Templates directory: C:\Users\mlloyd\Source\Repos\Nord.Nganga\Nord.Nganga.StEngine\templates
    Master Template version: 1.1.0 
    Body Template version: 1.1.0 
    Output signature: 692615212d0f5e4b4ec67f81b416281d    
    NGANGA - GENERATED CODE - END HEADER  
    */
rareCareNgApp.factory('sponsorsService', ['$resource', function($resource)
  {return {
      saveChangesToProgramPeriodSponsors: function(model, successFn, failFn)
        {
          var rsc = $resource('/api/Sponsors/SaveChangesToProgramPeriodSponsors', {}, {'save': {isArray: false, method: 'POST'}});

          rsc.save({}, model,  successFn, failFn);
        },
      getProgramPeriodSponsors: function( id , successFn, failFn) 
        {
          var rsc = $resource('/api/Sponsors/GetProgramPeriodSponsors/:id', {}, {'get' : { isArray: false  }});

          return rsc.get({ id: id }, {}, successFn, failFn);
        }
    };
  }]);
