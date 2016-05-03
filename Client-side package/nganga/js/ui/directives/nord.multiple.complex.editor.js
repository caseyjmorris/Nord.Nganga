(function()
  {
    var VERBOSE_MODE = false;
    
    var uidService;
    var parseService;
    var log;
    var interpolate;
    var toaster;
    var directiveEditStateEvaluator;
    
    function logIfVerbose(message)
      {
        if (VERBOSE_MODE)
          {
            log.debug(message);
          }
      }

    function liftCommonSelectSources(scope, sources)
      {
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

    function initializeCollection(scope)
      {
        logIfVerbose('initCollection entry => childFieldName:' + scope.childFieldName + ' collectionName:' +
         scope.collectionName);
        logIfVerbose(scope.collection);

        logIfVerbose(scope.parentObject);
        if (scope.$$childHead === undefined)
          {
            logIfVerbose('initCollection early exit - scope.$childHead is undefined => childFieldName:' +
              scope.childFieldName + ' collectionName:' + scope.collectionName);
          }
        if (scope.parentObject === undefined)
          {
            logIfVerbose('initCollection early exit - scope.parentObject is undefined => childFieldName:' +
              scope.childFieldName + ' collectionName:' + scope.collectionName);
            return;
          } // collection provider service has not been invoked yet

        scope.$$childHead[scope.childFieldName] = scope.$$childHead[scope.childFieldName] || {};
        scope.parentObject[scope.collectionName] = scope.parentObject[scope.collectionName] || [];
        scope.collection = scope.parentObject[scope.collectionName];

        logIfVerbose(scope.collection);
        logIfVerbose('initCollection exit => childFieldName:' + scope.childFieldName + ' collectionName:' +
          scope.collectionName);
      }

    function calcSums(scope)
      {
        if (!scope.ledgerSumProperty)
          {
            return;
          }
        var total = 0;
        for (var i = 0; i < scope.collection.length; i++)
          {
            var member = scope.collection[i];

            var val = member[scope.ledgerSumProperty];

            var current = val || 0;

            total += current;

            scope.sums[i] = total;
          }
      }


    function postLink(scope, element, attrs, canEditIfCtrl)
      {
        scope.sums = [];

        var initCollection = function()
          {
            initializeCollection(scope);
          };

        scope.editDisplayed = false;

        scope.activeItemIndex = null;
        scope.fieldDefinitionValues = parseService(scope.fieldDefinitions)();

        scope.additionalButtonValues = attrs.additionalButtons
          ? parseService(scope.additionalButtons)()
          : {};

        function calcSumsWatch()
          {
            if (!scope.parentObject.$promise)
              {
                calcSums(scope);
              }
            else
              {
                scope.parentObject.$promise.then(function()
                {
                  calcSums(scope);
                })
              }
          }

        var commonSelectSources = [];

        for (var i = 0; i < scope.fieldDefinitionValues.length; i++)
          {
            var fd = scope.fieldDefinitionValues[i];

            if (fd.clientType == 'selectcommon')
              {
                var commonSource = fd.filterArguments.split('.')[0];

                commonSelectSources.push(commonSource);
              }
          }

        var additionalButtonCategories = Object.keys(scope.additionalButtonValues);

        for (i = 0; i < additionalButtonCategories.length; i++)
          {
            var members = scope.additionalButtonValues[additionalButtonCategories[i]];

            for (var j = 0 ; j < members.length ; j++)
              {
                var member = members [j];

                if (member.attributeName != 'ng-click')
                  {
                    continue;
                  }

                commonSelectSources.push(member.attributeValue.split('(')[0]);
              }
          }

        liftCommonSelectSources(scope, commonSelectSources);

        for (i = 0 ; i < scope.fieldDefinitionValues.length; i++)
          {
            fd = scope.fieldDefinitionValues[i];

            if (fd.filterArguments)
              {
                fd.filterArguments = scope.$eval(fd.filterArguments) || fd.filterArguments;
              }
          }

        scope.nonActionHeaders = _(Object.keys(scope.additionalButtonValues)).filter(function(x)
          {
            return x !== 'Actions';
          }).value();

        initCollection(scope);

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

        scope.$watch('parentObject', calcSumsWatch, true);

        scope.uid = uidService.getUniqueId();

        scope.canEdit = function()
          {
            return directiveEditStateEvaluator.evaluateEditState(canEditIfCtrl, attrs);
          };

        scope.showActionsColumn = function()
          {
            return scope.additionalButtonValues.hasOwnProperty('Actions') ||
            (scope.canEdit() && !scope.ledgerSumProperty);
          };

        scope.addItem = function()
          {
            initCollection(scope);
            scope.$$childHead[scope.childFieldName] = scope.defaultObjectDefinition
              ? angular.fromJson(scope.defaultObjectDefinition)
              : {};
            scope.activeItemIndex = null;
            scope.editDisplayed = true;
          };

        scope.editItemAt = function(index)
          {
            console.log(index);
            scope.$$childHead[scope.childFieldName] = {};
            scope.activeItemIndex = index;
            $.extend(scope.$$childHead[scope.childFieldName], scope.collection[index]);
            scope.editDisplayed = true;
          };

        scope.deleteItemAt = function(index)
          {
            console.log(index);
            scope.collection.splice(index, 1);
            //set parent form dirty
            if (scope.childForm)
              {
                scope.childForm.$parent.$setDirty();
              }
          };

        scope.applyActiveItemChanges = function()
          {
            initCollection();
            if (scope.activeItemIndex === null)
              {
                scope.collection.push(scope.$$childHead[scope.childFieldName]);
              }
            else
              {
                scope.collection[scope.activeItemIndex] = scope.$$childHead[scope.childFieldName];
              }
            scope.activeItemIndex = null;
            scope.editDisplayed = false;
            if (scope.childForm)
              {
                scope.childForm.$setPristine();
                scope.childForm.$parent.$setDirty();
              }
            //set parent form dirty

            if (scope.ledgerSumProperty)
              {
                calcSums(scope);
              }
          };

        scope.discardActiveItemChanges = function()
          {
            scope.activeItemIndex = null;
            scope.editDisplayed = false;
            if (scope.childForm)
              {
                scope.childForm.$setPristine();
              }
          };

        scope.invokeOnParent = function(functionName, item, confirmationTemplate)
          {
            var execute = true;

            if (confirmationTemplate)
              {
                var msgExpression = interpolate(confirmationTemplate);
                var msg = msgExpression({ 'item': item });
                execute = (confirm(msg) === true);
              }

            if (execute)
              {
                scope.$parent[functionName](item);
              }
            else
              {
                toaster.pop({ body: "Operation cancelled.", type: 'info' });
              }
          };
      }

    function setOpenAndCloseTags(btn)
      {
        btn.openTag = btn.attributeName === 'ng-click' ? 'button type="button"' : 'a';
        btn.closeTag = btn.openTag.split(' ')[0];
      }

    function compile(element, attrs)
      {
        var additionalButtonValues = attrs.additionalButtons
          ? parseService(attrs.additionalButtons)()
          : {};

        var tpl = '\\<<<openTag>> class="<<cssClass>>" <<attributeName>>="<<attributeValue>>"\>' +
          '<span class="<<glyphicon>>"></span> <<actionLabel>>' +
          '</<<closeTag>>>';

        var actionsSection = element.find('span.additionalButtonActionSpan');

        if (additionalButtonValues.Actions)
          {
            for (var i = 0; i < additionalButtonValues.Actions.length; i++)
              {
                var btn = additionalButtonValues.Actions[i];
                setOpenAndCloseTags(btn);
                actionsSection.append(nganga.interpolate(tpl, btn));
              }
          }

        for (var sect in additionalButtonValues)
          {
            if (sect == 'Actions')
              {
                continue;
              }

            var headerBody = nganga.interpolate('<th><<header>></th>', { header: sect });

            //element.find('tr.membersHeaderRow').append(headerBody);
            element.find('tr.membersHeaderRow').children('[ng-repeat]').after(headerBody);


            var newCell = $('<td></td>');

            //element.find('tr.membersBodyRow').append(newCell);
            element.find('tr.membersBodyRow').children('[ng-repeat]').after(newCell);

            for (var i = 0; i < additionalButtonValues[sect].length; i++)
              {
                var btn = additionalButtonValues[sect][i];
                setOpenAndCloseTags(btn);
                newCell.append(nganga.interpolate(tpl, btn));
              }
          }

        return {
          post: postLink
        }
      }

    function provider($parse, $log, $interpolate, toasterProxyService, uniqueIdService, _directiveEditStateEvaluator)
      {
        parseService = $parse;
        log = $log;
        uidService = uniqueIdService;
        interpolate = $interpolate;
        toaster = toasterProxyService;
        directiveEditStateEvaluator = _directiveEditStateEvaluator;
        return {
          restrict: 'E',
          require: '^?canEditIf',
          transclude: true,
          templateUrl: window.ngangaTemplateLocation + 'nord.multiple.complex.editor.html',
          scope: {
            parentObject: '=',
            collectionName: '@',
            allowEdit: '@',
            additionalButtons: '@',
            fieldDefinitions: '@',
            childFieldName: '@',
            panelTopLabel: '@',
            ledgerSumProperty: '@',
            defaultObjectDefinition: '@'
          },
          compile: compile
        }
      }

    angular.module('nganga.ui').directive('nordMultipleComplexEditor', ['$parse', '$log', '$interpolate', 'toasterProxyService',
      'uniqueIdService', 'directiveEditStateEvaluator', provider]);
  })();