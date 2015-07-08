using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Humanizer;
using Nord.AngularUiGen.Annotations.Attributes.Angular;
using Nord.AngularUiGen.Engine.Extensions.Reflection;
using Nord.AngularUiGen.Engine.Extensions.Text;
using Nord.AngularUiGen.Engine.Html;
using Nord.AngularUiGen.Engine.Mapping;
using Nord.AngularUiGen.Engine.Support;
using Nord.CoreLib.Collections;
using Nord.CoreLib.Text;

namespace Nord.AngularUiGen.Engine.JavaScript
{
  /// <summary>
  /// Generate an Angular controller from a WebAPI controller
  /// </summary>
  /// <remarks>
  /// This is going to blow up if you try to use it with view models that have cycles in their object graphs.
  /// </remarks>
  public class ControllerGenerator
  {
    private int _indentLevel = 1;

    private readonly object _lockObj = new object();

    public string GenerateController(Type controller)
    {
      if (!controller.HasAttribute<AngularRouteIdParameterAttribute>())
      {
        throw new Exception("Please attribute your controller with AngularRouteIdParameterAttribute.");
      }

      if (!controller.HasAttribute<AngularModuleNameAttribute>())
      {
        throw new Exception("Please attribute your controller with AngularModuleNameAttribute.");
      }

      var idParameter = controller.GetAttribute<AngularRouteIdParameterAttribute>().ParameterName;

      var idParameterIsNullable =
        controller.GetAttributePropertyValueOrDefault<AngularRouteIdParameterAttribute, bool>(a => a.IsNullable);

      var applicationName = controller.GetAttribute<AngularModuleNameAttribute>().ModuleName;

      lock (this._lockObj)
      {
        return this.GenerateControllerInternal(controller, idParameter, applicationName, idParameterIsNullable);
      }
    }

    private string GetAncestorAppendString(IList<ViewModelViewModel> chain)
    {
      var str =
        AncestorAppendStringFormatter.GetAncestorAppendString(
          (chain.Select(c => c.Name)).ToList());

      return str;
    }

    private string GenerateControllerInternal(Type controller, string idParameter,
      string applicationName, bool idParameterIsNullable)
    {
      var endpoints = EndpointMapper.GetEnpoints(controller).Where(e => !e.ResourceOnly).ToList();

      var viewModels = endpoints.Where(e => e.HasReturnValue).Select(e => e.ReturnType).ToList();
      var vmHash = new HashSet<Type>(viewModels);

      var postEndpointTypes =
        endpoints.Where(
          e =>
            e.HttpMethod == EndpointViewModel.HttpMethodType.Post && e.ArgumentTypes.Any() &&
            !vmHash.Contains(e.ArgumentTypes.First()))
          .Select(e => e.ArgumentTypes.First())
          .Where(t => t.Name.EndsWith("ViewModel"));

      viewModels = viewModels.Concat(postEndpointTypes).Distinct().ToList();

      var vmVms = viewModels.Select(ViewModelMapper.GetViewModelViewModel).ToList();

      var privilegedRoleList =
        endpoints.Where(e => e.HttpMethod == EndpointViewModel.HttpMethodType.Post)
          .Select(e => e.SpecialAuthorization)
          .FirstOrDefault(a => a != null);

      var subordinateCollections = vmVms.SelectMany(m => this.GetAllComplexSubordinates(m)).ToList();

      var stringBuilder = new StringBuilder(
        "// GENERATED CODE --  " + DateTime.Now + " -- Changes to this file may be lost if the code is regenerated.\r\n")
        .Append(applicationName)
        .Append(".controller('")
        .Append(controller.Name.ToCamelCase().Replace("Controller", "Ctrl"))
        .Append("',\r\n")
        .Indent(this._indentLevel)
        .AppendLine("[")
        .Indent(++this._indentLevel);

      var ctrlSvc = controller.Name.Replace("Controller", string.Empty).ToCamelCase() + "Service";

      this.AppendHeader(stringBuilder, ctrlSvc,
        controller.GetAttributePropertyValueOrDefault<InjectAngularServicesAttribute, IEnumerable<string>>(
          a => a.Services));

      stringBuilder
        .Indent(++this._indentLevel);

      if (idParameter != null)
      {
        stringBuilder
          .AppendFormat("var {0} = $stateParams.{0};", idParameter)
          .SkipLines(2)
          .Indent(this._indentLevel);
      }

      if (privilegedRoleList != null)
      {
        this.AppendAuthorizationInformation(stringBuilder, privilegedRoleList.ToList());
      }

      this.AppendCommonRecords(stringBuilder, vmVms);

      if (idParameterIsNullable)
      {
        stringBuilder.AppendFormat("if({0} != null)", idParameter);
        this.AppendInBlock(stringBuilder,
          builder =>
          {
            this.AppendControllerRetrievals(builder, controller, ctrlSvc, idParameter);
            builder.Remove(builder.Length - (10 + this._indentLevel), 10 + this._indentLevel);
          });

        stringBuilder.SkipLinesAndIndent(this._indentLevel, 2);
      }
      else
      {
        this.AppendControllerRetrievals(stringBuilder, controller, ctrlSvc, idParameter);
      }

      if (!controller.HasAttribute<PresentAsViewOnlyDataAttribute>())
      {
        if (subordinateCollections.Any())
        {
          this.AppendExistingElementsObject(stringBuilder, subordinateCollections.ToList());

          this.AppendModalControlMethods(stringBuilder, subordinateCollections);
        }

        this.AppendPostMethods(stringBuilder,
          EndpointMapper.GetEnpoints(controller)
            .Where(e => e.HttpMethod == EndpointViewModel.HttpMethodType.Post)
            .Where(e => !e.ResourceOnly),
          ctrlSvc);
      }

      stringBuilder
        .Remove(stringBuilder.Length - (this._indentLevel*2 + 2), (this._indentLevel*2 + 2))
        .Indent(--this._indentLevel)
        .Append("}")
        .SkipLineAndIndent(--this._indentLevel)
        .Append("]);");

      return stringBuilder.ToString();
    }

    private void AppendHeader(StringBuilder stringBuilder, string controllerService,
      IEnumerable<string> additionalServices)
    {
      var commonSvcs = new[]
      {
        "$state", "$scope", "$stateParams", "modalManagerService", "commonRecordsService", "userInfoService",
        //"selectionsService",  TODO:  I have no idea what this is but there's no provider for it so it prevents pages from compiling.
        controllerService
      }.Concat(additionalServices ?? new string[0])
        .ToList();

      foreach (var commonSvc in commonSvcs)
      {
        stringBuilder.AppendFormat("'{0}', ", commonSvc);
      }

      stringBuilder
        .SkipLines()
        .Indent(this._indentLevel)
        .Append("function(");

      foreach (var commonSvc in commonSvcs)
      {
        stringBuilder.Append(commonSvc).Append(", ");
      }

      stringBuilder.Remove(stringBuilder.Length - 2, 2)
        .Append(")")
        .SkipLines()
        .Indent(++this._indentLevel)
        .Append("{")
        .SkipLines();
    }

    private void AppendAuthorizationInformation(StringBuilder stringBuilder, IList<string> roles)
    {
      stringBuilder.AppendFormat("$scope.canEdit = userInfoService.userIsInRole('{0}')", roles[0]);
      for (var i = 1; i < roles.Count; i++)
      {
        stringBuilder.AppendFormat(" || userInfoService.userIsInRole('{0}')", roles[i]);
      }
      stringBuilder.Append(";")
        .SkipLinesAndIndent(this._indentLevel, 2);
    }

    private void AppendCommonRecords(StringBuilder stringBuilder, ICollection<ViewModelViewModel> viewModels)
    {
      var commonRecords =
        viewModels.SelectMany(v => this.GetCommonRecordsWithResolvers(v))
          .DistinctBy(r => r.Key)
          .ToDictionary(x => x.Key, x => x.Value);

      var scopeCommonRecordObjects = commonRecords.Keys.Select(cr => cr.Split('.')[0]).Distinct();

      foreach (var scopeCommonRecordObject in scopeCommonRecordObjects)
      {
        stringBuilder.AppendFormat("$scope.{0} = $scope.{0} || {{}};", scopeCommonRecordObject)
          .SkipLinesAndIndent(this._indentLevel, 2);
      }

      foreach (var commonRecord in commonRecords)
      {
        stringBuilder.AppendFormat("$scope.{0} = {1}", commonRecord.Key,
          commonRecord.Value)
          .SkipLinesAndIndent(this._indentLevel, 2);
      }
    }

    private void AppendControllerRetrievals(StringBuilder stringBuilder, Type controller, string controllerService,
      string idParameter)
    {
      var getMethods =
        EndpointMapper.GetEnpoints(controller)
          .Where(m => m.HttpMethod == EndpointViewModel.HttpMethodType.Get)
          .Where(m => m.HasReturnValue)
          .Where(m => !m.ResourceOnly)
          .ToList();

      foreach (var getMethod in getMethods)
      {
        // Important:  I'm assuming the lone argument to all of these is the ID.  
        // If that's not true you'll need to edit the output.
        var returnValue = getMethod.ReturnType.Name.Replace("ViewModel", string.Empty).ToCamelCase();
        if (getMethod.ReturnsIEnumerable)
        {
          returnValue = returnValue.Pluralize();
        }

        var callString = string.Format(@"$scope.{0} = {1}.{2}({3});", returnValue, controllerService,
          getMethod.MethodName.ToCamelCase(), idParameter);

        stringBuilder.AppendFormat(callString)
          .SkipLines(2)
          .Indent(this._indentLevel);
      }
    }

    private void AppendExistingElementsObject(StringBuilder stringBuilder,
      IList<KeyValuePair<ViewModelViewModel, IList<ViewModelViewModel>>> subordinateObjects)
    {
      stringBuilder.AppendLine("var existingElementIds = {")
        .Indent(++this._indentLevel);

      for (var i = 0; i < subordinateObjects.Count; i++)
      {
        stringBuilder.AppendFormat("{0}{1}: null",
          this.GetAncestorAppendString(subordinateObjects[i].Value), subordinateObjects[i].Key.Name);
        if (i + 1 < subordinateObjects.Count)
        {
          stringBuilder.Append(",\r\n")
            .Indent(this._indentLevel);
        }
      }

      stringBuilder.SkipLines()
        .Indent(--this._indentLevel)
        .Append("};")
        .SkipLines(2)
        .Indent(this._indentLevel);
    }

    private void AppendModalControlMethods(StringBuilder stringBuilder,
      IEnumerable<KeyValuePair<ViewModelViewModel, IList<ViewModelViewModel>>> subordinateObjects)
    {
      foreach (var subordinateObject in subordinateObjects)
      {
        this.AppendModalControlMethod(stringBuilder, subordinateObject.Key, subordinateObject.Value);
      }
    }

    private void AppendModalControlMethod(StringBuilder stringBuilder, ViewModelViewModel subordinateObject,
      IList<ViewModelViewModel> masterObjectsChain)
    {
      //TODO:  this could be cleaner since I wrote the helper methods partway through.

      var ancestorAppendString = this.GetAncestorAppendString(masterObjectsChain);
      var propertyString = ancestorAppendString + subordinateObject.Name;
      var ancestorsCleanString = string.IsNullOrEmpty(ancestorAppendString)
        ? masterObjectsChain[0].Name
        : ancestorAppendString.TrimEnd('_');
      var pluralNameOfType = subordinateObject.Name.Pluralize();


      stringBuilder.AppendFormat("$scope.add{0} = function()", propertyString.WithFirstLetterUppercased())
        .SkipLines()
        .Indent(++this._indentLevel)
        .AppendLine("{")
        .Indent(++this._indentLevel)
        .AppendFormat("$scope.{0}.{1} = $scope.{0}.{1} || [];", ancestorsCleanString,
          pluralNameOfType)
        .SkipLineAndIndent(this._indentLevel)
        .AppendFormat("$scope.{0} = {{}};", propertyString)
        .SkipLines()
        .Indent(this._indentLevel)
        .AppendFormat("existingElementIds.{0} = null;", propertyString)
        .SkipLines()
        .Indent(this._indentLevel)
        .AppendFormat("modalManagerService.showModal('{0}Modal');", propertyString)
        .SkipLines()
        .Indent(--this._indentLevel)
        .Append("};")
        .SkipLines(2)
        .Indent(--this._indentLevel);

      stringBuilder.AppendFormat("$scope.edit{0}At = function(index)",
        propertyString.WithFirstLetterUppercased())
        .SkipLineAndIndent(++this._indentLevel)
        .Append("{")
        .SkipLineAndIndent(++this._indentLevel)
        .AppendFormat("$scope.{0} = {{}};", propertyString)
        .SkipLineAndIndent(this._indentLevel)
        .AppendFormat("existingElementIds.{0} = index;", propertyString)
        .SkipLineAndIndent(this._indentLevel)
        .AppendFormat("$.extend($scope.{0}, $scope.{1}.{2}[index]);", propertyString,
          ancestorsCleanString, pluralNameOfType)
        .SkipLineAndIndent(this._indentLevel)
        .AppendFormat("modalManagerService.showModal('{0}Modal');", propertyString)
        .SkipLineAndIndent(--this._indentLevel)
        .Append("};")
        .SkipLines(2)
        .Indent(--this._indentLevel);

      stringBuilder.AppendFormat("$scope.save{0}Modal = function()", propertyString.WithFirstLetterUppercased())
        .SkipLineAndIndent(++this._indentLevel)
        .Append("{")
        .SkipLineAndIndent(++this._indentLevel)
        .AppendFormat("if (existingElementIds.{0} === null)", propertyString)
        .SkipLineAndIndent(++this._indentLevel)
        .Append("{")
        .SkipLineAndIndent(++this._indentLevel)
        .AppendFormat("$scope.{0}.{1}.push($scope.{2});", ancestorsCleanString, pluralNameOfType,
          propertyString)
        .SkipLineAndIndent(--this._indentLevel)
        .Append("}")
        .SkipLineAndIndent(--this._indentLevel)
        .Append("else")
        .SkipLineAndIndent(++this._indentLevel)
        .Append("{")
        .SkipLineAndIndent(++this._indentLevel)
        .AppendFormat("$scope.{0}.{1}[existingElementIds.{2}] = $scope.{2};", ancestorsCleanString,
          pluralNameOfType, propertyString)
        .SkipLineAndIndent(--this._indentLevel)
        .Append("}")
        .SkipLineAndIndent(--this._indentLevel)
        .AppendFormat("modalManagerService.closeModal('{0}Modal');", propertyString)
        .SkipLineAndIndent(this._indentLevel)
        .AppendFormat("$scope.{0}Form.$setPristine();", propertyString)
        .SkipLineAndIndent(this._indentLevel)
        .AppendFormat("$scope.{0}Form.$setDirty();", ancestorsCleanString)
        .SkipLineAndIndent(--this._indentLevel)
        .Append("};")
        .SkipLinesAndIndent(--this._indentLevel, 2);

      stringBuilder.AppendFormat("$scope.delete{0}At = function(index)",
        propertyString.WithFirstLetterUppercased());

      this.AppendInBlock(stringBuilder, s =>
      {
        s.AppendFormat("$scope.{0}.{1}.splice(index, 1);", ancestorsCleanString, pluralNameOfType)
          .SkipLineAndIndent(this._indentLevel)
          .AppendFormat("$scope.{0}Form.$setDirty();", ancestorsCleanString);
      });

      stringBuilder.Append(";")
        .SkipLinesAndIndent(this._indentLevel, 2);
    }

    private void AppendPostMethods(StringBuilder stringBuilder, IEnumerable<EndpointViewModel> endpoints,
      string serviceName)
    {
      foreach (var e in endpoints.Where(ep=>ep.ArgumentTypes.Any()))
      {
        var endpoint = e;

        var model = endpoint.ArgumentTypes.First();

        var modelStr = model.IsGenericType && typeof(IEnumerable).IsAssignableFrom(model) && model != typeof(string)
          ? model.GetGenericArguments()[0].Name.ToCamelCase().Replace("ViewModel", string.Empty).Pluralize()
          : model.Name.ToCamelCase().Replace("ViewModel", string.Empty);

        stringBuilder.AppendFormat("$scope.saveChangesTo{0} = function()", modelStr.WithFirstLetterUppercased());

        string ctrlReturnTypeStr = null;

        if (endpoint.HasReturnValue)
        {
          var endpointReturnVmVm = ViewModelMapper.GetViewModelViewModel(endpoint.ReturnType);

          ctrlReturnTypeStr = endpoint.ReturnsIEnumerable
            ? endpointReturnVmVm.Name.Pluralize()
            : endpointReturnVmVm.Name;
        }

        this.AppendInBlock(stringBuilder, s =>
        {
          s.AppendFormat("{0}.{1}($scope.{2}, function(result)", serviceName, endpoint.MethodName.ToCamelCase(),
            modelStr);
          this.AppendInBlock(s,
            s1 =>
            {
              s1.AppendIf(string.Format("$scope.{0} = result;\r\n", ctrlReturnTypeStr), endpoint.HasReturnValue)
                .IndentIf(this._indentLevel, endpoint.HasReturnValue)
                .AppendFormat("$scope.{0}Form.$setPristine();", modelStr);

              foreach (var expression in endpoint.OnPostSuccessExpressions)
              {
                s1.SkipLineAndIndent(this._indentLevel).Append(expression);
              }
            });

          s.Append(");");
        });

        stringBuilder
          .Append(";")
          .SkipLinesAndIndent(this._indentLevel, 2);
      }
    }

    private void AppendInBlock(StringBuilder stringBuilder, Action<StringBuilder> action)
    {
      stringBuilder.SkipLineAndIndent(++this._indentLevel)
        .Append("{")
        .SkipLineAndIndent(++this._indentLevel);

      action(stringBuilder);

      stringBuilder.SkipLineAndIndent(--this._indentLevel)
        .Append("}");

      this._indentLevel--;
    }

    private IList<KeyValuePair<ViewModelViewModel, IList<ViewModelViewModel>>> GetAllComplexSubordinates(
      ViewModelViewModel viewModel, IList<ViewModelViewModel> soFar = null)
    {
      soFar = soFar == null ? new List<ViewModelViewModel>() : soFar.ToList();
      soFar.Add(viewModel);
      var list =
        viewModel.ComplexCollections
          .Where(vm => !vm.Model.IsViewOnly)
          .Select(
            cc => new KeyValuePair<ViewModelViewModel, IList<ViewModelViewModel>>(cc.Model, soFar)).ToList();

      foreach (var cc in viewModel.ComplexCollections)
      {
        list.AddRange(this.GetAllComplexSubordinates(cc.Model, soFar));
      }

      return list;
    }

    private Dictionary<string, string> GetCommonRecordsWithResolvers(ViewModelViewModel vmVm,
      Dictionary<string, string> dict = null)
    {
      dict = dict ?? new Dictionary<string, string>();

      Func<ViewModelViewModel.FieldViewModel, Tuple<string, string>> getCrName =
        model =>
          Tuple.Create(model.SelectCommon.QualifiedName, model.SelectCommon.ProviderExpression);

      var commonRecordStrings =
        vmVm.Scalars.Where(s => s.SelectCommon != null)
          .Concat(vmVm.PrimitiveCollections.Where(pc => pc.SelectCommon != null))
          .Select(getCrName);

      foreach (var crStr in commonRecordStrings)
      {
        if (dict.ContainsKey(crStr.Item1))
        {
          continue;
        }
        dict[crStr.Item1] = crStr.Item2;
      }

      foreach (var complex in vmVm.ComplexCollections)
      {
        this.GetCommonRecordsWithResolvers(complex.Model, dict);
      }

      return dict;
    }
  }
}