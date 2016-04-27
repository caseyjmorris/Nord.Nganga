(function()
  {
    var VERBOSE_MODE = false;

    var priority = 2000;

    var uidService;

    var log;

    var commonSelectSources = {};

    var directiveEditStateEvaluator;

    var forms = {};

    function logIfVerbose(obj)
      {
        if (VERBOSE_MODE)
          {
            log.debug(obj);
          }
      }

    function liftCommonSelectSources(scope, uid)
      {
        var sources = commonSelectSources[uid];

        for (var i = 0; i < sources.length; i++)
          {
            var src = sources[i];

            var ancestor = scope.$parent;

            while (!scope[src] && ancestor)
              {
                if (ancestor[src])
                  {
                    scope[src] = ancestor[src];
                    break;
                  }
                ancestor = ancestor.$parent;
              }
          }
      }

    function cacheForm(el, scope)
      {
        var MAX_TRIES = 100;

        var formEl = angular.element(el.closest('form'));

        var formName = formEl.attr('name');

        var targetScope = scope;

        var tries = 0;

        while (!targetScope[formName] && scope.$parent && tries < MAX_TRIES)
          {
            targetScope = scope.$parent;

            tries++;
          }

        if (targetScope[formName])
          {
            forms[formName] = targetScope[formName];
          }
      }

    function dirtyParent(el)
      {
        var formEl = angular.element(el.closest('form'));

        var formName = formEl.attr('name');

        var formCtrl = forms[formName];

        if (!formCtrl)
          {
            return;
          }

        formCtrl.$setDirty();
      }

    function initializeCollection(scope)
      {
        logIfVerbose('entered initializeCollection.  scope follows');

        logIfVerbose(scope);

        if (!scope.parentObject)
          {
            logIfVerbose('scope.parentObject falsy; exiting');

            return;
          }

        function doInit()
          {
            logIfVerbose('initializeCollection:  entered initialization function')

            logIfVerbose('initializing property ' + scope.collectionName);

            scope.parentObject[scope.collectionName] = scope.parentObject[scope.collectionName] || [];

            logIfVerbose('setting scope.collection');

            scope.collection = scope.parentObject[scope.collectionName];

            if (!scope.collection || scope.collection !== scope.parentObject[scope.collectionName])
              {
                throw 'initialization failed';
              }
          }

        if (scope.parentObject.$promise)
          {
            logIfVerbose('initializeCollection:  object is promise; execution will be deferred');

            scope.parentObject.$promise.then(doInit);
          }
        else
          {
            logIfVerbose('initializeCollection:  object is not promise; execution will not be deferred');

            doInit();
          }
      }

    function postLinkCommon(scope, el, attrs, canEditIfCtrl)
      {
        cacheForm(el, scope);

        function initCollection()
          {
            initializeCollection(scope);
          }

        initCollection();

        scope.$watch('parentObject', function()
        {
          initCollection(scope);
          var exp = 'parentObject.' + scope.collectionName;
          for (var i = 0; i < scope.$$watchers.length; i++)
            {
              var watcher = scope.$$watchers[i];
              if (watcher.exp === exp)
                {
                  return;
                }
            }
          scope.$watch(exp, initCollection, false);
        }, false);

        scope.canEdit = function()
          {
            return directiveEditStateEvaluator.evaluateEditState(canEditIfCtrl, attrs);
          };

        scope.deleteItemAt = function(index)
          {
            logIfVerbose(nganga.interpolate('Deleting item at <<r>>', {r: index}));
            scope.collection.splice(index, 1);
            dirtyParent(el);
          };

        scope.anythingToShow = function()
          {
            return scope.collection && scope.collection.length && scope.collection.length > 0;
          }
      }

    function getScopeCommonProperties()
      {
        return {
          parentObject: '=',
          collectionName: '@',
          allowEdit: '@',
          fieldLabelText: '@'
        }
      }

    (function nordMultipleSimpleEditor()
      {
        var transcludables = {};

        var timeoutService;

        function preprocess(el, attrs)
          {
            if (attrs.hasOwnProperty('isPrimitive'))
              {
                var oldModel = nganga.interpolate('[ng-model=<<childFieldName>>]', attrs);

                el.find(oldModel).each(function()
                {
                  $(this)
                    .attr('ng-model', nganga.interpolate('parentObject.<<collectionName>>[$index]', attrs))
                    .attr('twelfths', '12')
                    .attr('field-label-text', '');
                });
              }

            var uid = uidService.getUniqueId();

            el.attr('id', uid);

            transcludables[uid] = el.html();

            commonSelectSources[uid] = [];

            el.find('[common-select-source]').each(function()
            {
              var val = angular.element(this).attr('common-select-source').split('.')[0];

              commonSelectSources[uid].push(val);
            });

            el.html('');
          }

        function postLink(scope, el, attrs)
          {
            postLinkCommon(scope, el, attrs);

            var uid = el.attr('id');

            liftCommonSelectSources(scope, uid);

            scope.addItem = function()
              {
                initializeCollection(scope);
                var pushVal = scope.defaultObjectDefinition ? angular.fromJson(scope.defaultObjectDefinition) : {};
                scope.collection.push(pushVal);

                function transform()
                  {
                    var lines = angular.element('#' + uid + ' li.simple-editor-line-item');

                    logIfVerbose(lines);

                    var lastLine = lines.last();

                    logIfVerbose(lastLine);

                    logIfVerbose(lastLine[0]);

                    var firstInput = lastLine.find('input')
                      .first();

                    logIfVerbose(firstInput);

                    if (firstInput.length > 0)
                      {
                        var el = firstInput[0];

                        logIfVerbose(el);

                        el.focus();

                        el.select();
                      }
                  }

                timeoutService(transform);
              };
          }

        function provider($log, $timeout, _directiveEditStateEvaluator)
          {
            log = $log;
            timeoutService = $timeout;
            directiveEditStateEvaluator = _directiveEditStateEvaluator;

            var scopeObj = getScopeCommonProperties();

            scopeObj.defaultObjectDefinition = '@';

            scopeObj.childFieldName = '@';

            return {
              restrict: 'E',
              priority: priority,
              templateUrl: window.ngangaTemplateLocation + 'nord.multiple.simple.editor.html',
              scope: scopeObj,
              require: '^?canEditIf',
              compile: compile
            }
          }

        function preprocessorProvider(uniqueIdService)
          {
            uidService = uniqueIdService;

            return {
              restrict: 'E',
              priority: priority + 1,
              compile: preprocess
            }
          }

        function compile(el, attrs)
          {
            angular.element(el.find('li.simple-editor-line-item'))
              .attr('ng-repeat', nganga.interpolate('<<childFieldName>> in collection track by $index', attrs));

            angular.element(el.find('button.list-item-delete-button'))
              .attr('ng-click', 'deleteItemAt($index)');

            var uid = el.attr('id');

            el.find('div.pseudotransclude').append(transcludables[uid]);

            return {
              post: postLink
            }
          }

        ngangaUi.directive('nordMultipleSimpleEditor', ['uniqueIdService', preprocessorProvider]);

        ngangaUi.directive('nordMultipleSimpleEditor', ['$log', '$timeout', 'directiveEditStateEvaluator', provider]);
      })();

    (function nordFileUploadCollection()
      {
        function postLink(scope, el, attrs)
          {
            postLinkCommon(scope, el, attrs);

            scope.newDoc = {};

            var uid = el.attr('id');

            scope.uid = uid;

            var uploadControlName = uid + "-upload-control";

            scope.addFiles = function()
              {
                initializeCollection(scope);

                logIfVerbose(scope.collection);

                logIfVerbose(scope.parentObject[scope.collectionName]);

                logIfVerbose('addFiles triggered');

                logIfVerbose('control name:  ' + uploadControlName);

                var control = document.getElementById(uploadControlName);

                logIfVerbose(control);

                function receiveResult(result)
                  {
                    logIfVerbose(result);

                    logIfVerbose(scope.collection);

                    scope.collection.push(result);

                    logIfVerbose(scope.collection);

                    scope.$digest();

                    dirtyParent(el);
                  }

                serializeFiles(control, scope.newDoc.documentTypeId, receiveResult);

                control.value = "";
              };

            scope.showDocumentTypeSelector = function()
              {
                return scope.documentTypeSource && scope.documentTypeSource.length >= 1;
              };

            scope.uploadEnabled = function()
              {
                if (!scope.canEdit())
                  {
                    return false;
                  }

                if (!scope.showDocumentTypeSelector())
                  {
                    return true;
                  }

                return scope.newDoc.documentTypeId !== null && scope.newDoc.documentTypeId !== undefined;
              }
          }

        function compile(el)
          {
            el.attr('id', uidService.getUniqueId());

            return {
              post: postLink
            };
          }

        function serializeFiles(control, documentTypeId, executeOnResultsFn)
          {
            for (var i = 0; i < control.files.length; i++)
              {

                var file = control.files[i];

                var newMember = {
                  fileName: file.name,
                  documentTypeId: documentTypeId,
                  id: 0,
                  mimeType: file.type
                };

                var fileReader = new FileReader();

                fileReader.onloadend = function(e)
                  {
                    newMember.uri = e.target.result;

                    executeOnResultsFn(newMember);
                  };

                fileReader.readAsDataURL(file);
              }
        }

        function provider(uniqueIdService, _directiveEditStateEvaluator)
          {
            uidService = uniqueIdService;

            directiveEditStateEvaluator = _directiveEditStateEvaluator;

            var scopeObj = getScopeCommonProperties();

            scopeObj.documentTypeSource = '=';

            return {
              restrict: 'E',
              scope: scopeObj,
              compile: compile,
              require: '^?canEditIf',
              templateUrl: window.ngangaTemplateLocation + 'nord.file.upload.collection.html'
            }
          }

        ngangaUi.directive('nordFileUploadCollection', ['uniqueIdService', 'directiveEditStateEvaluator', provider]);
      })();

  })
();
