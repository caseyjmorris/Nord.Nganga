using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Humanizer;
using Nord.AngularUiGen.Annotations.Attributes.ViewModels;
using Nord.AngularUiGen.Engine.Extensions;
using Nord.CoreLib;
using Nord.CoreLib.Collections;
using Nord.CoreLib.Mvc;
using Nord.CoreLib.Text;
using Nord.AngularUiGen.Engine.Extensions.Reflection;
using Nord.AngularUiGen.Engine.Extensions.Text;
using Nord.AngularUiGen.Engine.Mapping;
using Nord.AngularUiGen.Engine.Support;

namespace Nord.AngularUiGen.Engine.Html
{
  public class HtmlGenerator
  {
    private readonly OccurrenceCounter<string> idCounter;


    public HtmlGenerator(OccurrenceCounter<string> idCounter)
    {
      this.idCounter = idCounter;
    }

    internal string PrettyPrint(string html)
    {
      var indented = XDocument.Parse(html).ToString();
      var invalidTagsStripped = HtmlVoidElements.InvalidClosingTagRegex.Replace(indented, String.Empty);
      return invalidTagsStripped;
    }

    internal TreeTagBuilder GenerateHtmlFormTag(ViewModelViewModel model, bool withEdit,
      ICollection<TreeTagBuilder> childForms,
      string postAction = "saveChanges",
      IList<string> ancestors = null,
      string enclosingModal = null)
    {
      if (model.IsViewOnly)
      {
        withEdit = false;
      }

      ancestors = (ancestors ?? new List<string>()).ToList();

      var ancestorAppendString = AncestorAppendStringFormatter.GetAncestorAppendString(ancestors);
      ancestors.Add(model.Name);
      var formName = string.Format("{0}{1}Form", ancestorAppendString, model.Name);

      var formMaster = new TreeTagBuilder("form")
        .WithOptionalAttributeIf("name", formName, () => withEdit)
        .WithOptionalAttributeIf("id", formName, () => withEdit)
        .WithOptionalAttributeIf("data-ng-submit",
          string.Format("{0}To{1}()", postAction, model.Name.WithFirstLetterUppercased()),
          () => withEdit && enclosingModal == null)
        .WithOptionalCssClassIf("user-edit-form", () => withEdit)
        .WithOptionalCssClassIf("user-view-form", () => !withEdit)
        .WithCssClass("form-horizontal");

      var conjoinedFields = model.Scalars
        .Where(s => !s.IsHidden)
        .Select(s => new FormFieldWrapper
        {
          IsCollection = false,
          Model = s,
          Section = s.Section ?? string.Empty,
        })
        .ToList();

      conjoinedFields.AddRange(model.PrimitiveCollections
        .Where(pc => !pc.IsHidden)
        .Select(pc => new FormFieldWrapper
        {
          IsCollection = true,
          Model = pc,
          Section = pc.Section ?? string.Empty
        }));

      var keysOrdered =
        conjoinedFields.GroupBy(cf => cf.Section)
          .Select(g => g.Key)
          .Concat(model.ComplexCollections.Select(cc => cc.Section))
          .DistinctBy(x => x) //Distinct() does not guarantee order
          .ToList();

      var conjoinedFieldsDict = new Dictionary<string, IList<FormFieldWrapper>>();

      foreach (var cf in conjoinedFields)
      {
        IList<FormFieldWrapper> sectionMembers;
        if (!conjoinedFieldsDict.TryGetValue(cf.Section, out sectionMembers))
        {
          conjoinedFieldsDict[cf.Section] = new List<FormFieldWrapper>();
          sectionMembers = conjoinedFieldsDict[cf.Section];
        }
        sectionMembers.Add(cf);
      }

      var complexItemsDict = new Dictionary<string, IList<SubordinateViewModelWrapper>>();

      foreach (var ci in model.ComplexCollections)
      {
        IList<SubordinateViewModelWrapper> sectionMembers;
        if (!complexItemsDict.TryGetValue(ci.Section, out sectionMembers))
        {
          complexItemsDict[ci.Section] = new List<SubordinateViewModelWrapper>();
          sectionMembers = complexItemsDict[ci.Section];
        }
        sectionMembers.Add(ci);
      }

      foreach (var section in keysOrdered)
      {
        var sectionMaster = formMaster
          .AppendChild("div")
          .WithCssClass("form-section");

        if (!string.IsNullOrWhiteSpace(section))
        {
          Dictionary<string, string> htmlAttrs;
          if (model.SectionHtmlAttributeValues.TryGetValue(section, out htmlAttrs))
          {
            sectionMaster.WithAttributes(htmlAttrs, replaceExisting: true);
          }

          sectionMaster.AppendChild("div")
            .WithCssClass("row form-section-header")
            .AppendChild("div")
            .WithCssClass("col-md-12")
            .AppendChild("h3")
            .WithInnerText(section);
        }
        TreeTagBuilder currentRow = null;
        var i = 0;
        foreach (
          var fld in conjoinedFieldsDict.ContainsKey(section) ? conjoinedFieldsDict[section] : new FormFieldWrapper[0])
        {
          var field = fld;
          if (i%3 == 0)
          {
            currentRow = sectionMaster.AppendChild("div")
              .WithCssClass("row");
          }
          i++;
          Debug.Assert(currentRow != null, "Current row should not be null due to modulo");
          var readOnlyField = !withEdit || field.Model.IsViewOnly;
          var fieldName = field.Model.FieldName.ToCamelCase();
          var col = currentRow.AppendChild("div")
            .WithCssClass("col-md-4");
          col.AppendChild("label")
            .WithOptionalCssClassIf("required-field-label", () => field.Model.IsRequired)
            .WithOptionalAttributeIf("for",
              string.Format("{0}FormControl{1}", fieldName, this.idCounter.Count(fieldName)),
              () =>
                !readOnlyField && !field.IsCollection &&
                field.Model.DataType.GetNonNullableType() != typeof (bool))
            .WithInnerText(field.Model.DisplayName);

          if (!readOnlyField && field.Model.SelectCommon != null && !field.IsCollection)
          {
            this.AppendSelectList(col, field.Model, model, withEdit: true, ancestorAppendString: ancestorAppendString);
          }
          else if (readOnlyField && field.Model.SelectCommon != null && !field.IsCollection)
          {
            this.AppendReadOnlyCommonRecordValue(col, field.Model, model, ancestorAppendString);
          }
          else if (readOnlyField && !field.IsCollection)
          {
            col.AppendChild("p")
              .WithCssClass("form-readonly-entry")
              .WithInnerText(this.GetFieldDisplayCode(model, field.Model, ancestorAppendString));
          }
          else if (readOnlyField && field.IsCollection)
          {
            this.AppendReadOnlyList(col, field.Model, model, ancestorAppendString);
          }
          else if (!readOnlyField && field.IsCollection)
          {
            this.AppendEditingList(col, field.Model, model, ancestorAppendString);
          }
          else if (!readOnlyField && !field.IsCollection)
          {
            this.AppendEditControlForScalar(col, field.Model, model, ancestorAppendString);
          }
        }

        foreach (
          var complex in
            complexItemsDict.ContainsKey(section) ? complexItemsDict[section] : new SubordinateViewModelWrapper[0])
        {
          sectionMaster
            .AppendFullWidthBootStrapRow()
            .AppendChild("h4")
            .WithInnerText(complex.DisplayName);

          sectionMaster
            .AppendFullWidthBootStrapRow()
            .AppendChild(
              this.GenerateHtmlDisplayTableTag(
                complex.Model, 
                withEdit, 
                childForms, 
                prefix: model.Name,
                ancestors: ancestors, 
                isLedger: complex.IsLedger, 
                ledgerSumProperty: complex.LedgerSumProperty,
                itemActionAttribute: complex.ItemActionAttribute));
        }
      }

      if (withEdit && enclosingModal == null)
      {
        formMaster.AppendChild("div")
          .WithCssClass("row")
          .AppendChild("div")
          .WithCssClass("col-md-12")
          .AppendChild("button")
          .WithCssClass("btn btn-primary pull-right")
          .WithInnerText("Save changes to " + model.Name.ToSpaced().ToLower())
          .WithAttribute("data-ng-disabled", string.Format("{0}.$invalid || {0}.$pristine", formName))
          .WithAttribute("type", "submit");
      }
      if (enclosingModal == null)
      {
        return formMaster;
      }
      else
      {
        var modal = new TreeTagBuilder("nord-modal")
          .WithAttributes(
            new
            {
              nord_modal_title = model.Name.ToSpaced().WithFirstLetterUppercased(),
              nord_modal_target = enclosingModal,
              nord_submit_caption = "Apply changes to " + model.Name.ToSpaced().ToLower(),
              nord_submit_disabled = enclosingModal + ".$invalid",
              nord_on_submit = "save" + enclosingModal.WithFirstLetterUppercased() + "()",
            });

        return formMaster.AppendToParent(modal);
      }
    }

    public string GenerateHtmlForm(Type type, bool withEdit, string postAction = "saveChanges")
    {
      var model = ViewModelMapper.GetViewModelViewModel(type);

      var childForms = new List<TreeTagBuilder>();

      var formMaster = this.GenerateHtmlFormTag(model, withEdit, childForms, postAction);

      var topDiv = new TreeTagBuilder("div")
        .WithAttribute("id",
          string.Format("{0}-{1}-section", model.Name.ToSpaced().ToLowerInvariant().Replace(" ", "-"),
            withEdit ? "edit" : "view"));

      topDiv.AppendChild(formMaster);

      foreach (var childForm in childForms)
      {
        topDiv.AppendChild(childForm);
      }

      return this.PrettyPrint(topDiv.ToString());
    }

    private void AppendReadOnlyList(TreeTagBuilder tag, ViewModelViewModel.FieldViewModel field,
      ViewModelViewModel model, string ancestorAppendString)
    {
      var ngModelRepeatString = string.Format("{0}{1}.{2}", ancestorAppendString, model.Name, field.FieldName);
      var ul = tag.AppendChild("ul")
        .WithCssClass("viewing-list")
        .AppendChild("li")
        .WithAttribute("data-ng-repeat", string.Format("item in {0}", ngModelRepeatString));
      if (field.SelectCommon == null)
      {
        ul
          .WithInnerText("{{item}}");
      }
      else
      {
        ul
          .AppendChild("select")
          .WithCssClass("form-control")
          .WithAttribute("disabled", "disabled")
          .WithAttribute("data-ng-model", string.Format("{0}[$index]", ngModelRepeatString))
          .WithAttribute("data-ng-options",field.SelectCommon.GetIteratorExpression())
          .AppendChild("option")
          .WithAttribute("value", string.Empty)
          .WithInnerText("---");
      }
    }

    private void AppendEditingList(TreeTagBuilder tag, ViewModelViewModel.FieldViewModel field,
      ViewModelViewModel model, string ancestorAppendString)
    {
      var li = tag.AppendChild("ul")
        .WithCssClass("editing-list")
        .AppendChild("li")
        .WithAttribute("data-ng-repeat",
          string.Format("item in {0}{1}.{2} track by $index", ancestorAppendString, model.Name, field.FieldName));

      var inputGroup = li
        .AppendChild("div")
        .WithCssClass("input-group");

      var fieldControlType = "text";
      const string pushVal = "null";
      var type = field.DataType.GetNonNullableType();
      if (type == typeof (DateTime))
      {
        fieldControlType = "date";
      }
      if (type == typeof (int) || type == typeof (decimal) || type == typeof (double))
      {
        fieldControlType = "number";
      }

      if (field.SelectCommon == null)
      {
        inputGroup.AppendChild("input")
          .WithAttribute("type", fieldControlType)
          .WithCssClass("form-control")
          .WithOptionalAttributeIf("required", "required", () => field.IsRequired)
          .WithOptionalAttributeIf("step", "1", () => type == typeof (int))
          .WithOptionalAttributeIf("min",
            NullablePropertyHelper.ResolvePropertyOrDefault(() => field.Minimum.ToString()),
            () => field.Minimum != null)
          .WithOptionalAttributeIf("max",
            NullablePropertyHelper.ResolvePropertyOrDefault(() => field.Minimum.ToString()),
            () => field.Maximum != null)
          .WithAttribute("data-ng-model", string.Format("{0}.{1}[$index]", model.Name, field.FieldName));
      }

      else
      {
        inputGroup.AppendChild("select")
          .WithCssClass("form-control")
          .WithAttribute("data-ng-model", string.Format("{0}.{1}[$index]", model.Name, field.FieldName))
          .WithOptionalAttributeIf("ng-readonly", field.SelectCommon.NotResolvedExpression, () => field.SelectCommon.RestrictUntilResolved)
          .WithAttribute("data-ng-options", field.SelectCommon.GetIteratorExpression())
          .AppendChild("option")
          .WithAttribute("value", string.Empty)
          .WithInnerText("---");
      }

      // This is another instance of using the == instead of && to avoid HTML escaping in TagBuilder.  The effect is the same; the expressions on both sides are evaluated and the return value of the comparison is ignored.

      inputGroup
        .AppendChild("span")
        .WithCssClass("input-group-btn")
        .AppendChild("button")
        .WithAttribute("type", "button")
        .WithCssClass("btn btn-danger")
        .WithAttribute("data-ng-click",
          string.Format("({0}.{1}.splice($index, 1)) == ({2}{0}Form.$setDirty())", model.Name, field.FieldName,
            ancestorAppendString))
        .WithInnerText("Delete");

      // yes, using && instead of == would probably make the intent of the push code clearer.  But TagBuilder wants to convert that into &amp;&amp;.

      tag
        .AppendChild("div")
        .WithCssClass("row")
        .AppendChild("div")
        .WithCssClass("col-md-12")
        .AppendChild("button")
        .WithAttribute("type", "button")
        .WithCssClass("btn btn-default pull-right")
        .WithOptionalAttributeIf("ng-disabled",field.SelectCommon.NotResolvedExpression,
          () => field.SelectCommon != null && field.SelectCommon.RestrictUntilResolved)
        .WithAttribute("data-ng-click",
          string.Format("({0}.{1} = {0}.{1} || []) == {0}.{1}.push({2})", model.Name, field.FieldName, pushVal))
        .WithInnerText("Add");
    }

    private void AppendReadOnlyCommonRecordValue(TreeTagBuilder tag, ViewModelViewModel.FieldViewModel field,
      ViewModelViewModel model, string ancestorAppendString)
    {
      tag
        .AppendChild("p")
        .WithCssClass("form-readonly-entry")
        .WithInnerText(this.GetReadOnlyCommonRecordValueText(field, model, ancestorAppendString));
    }

    private static string GetCommonRecordsIndexer(ViewModelViewModel.FieldViewModel field, ViewModelViewModel model,
      string ancestorAppendString)
    {
      if (field.SelectCommon == null )
      {
        return string.Empty;
      }
      return field.SelectCommon.Index == null ? string.Empty : string.Format(@"[{0}{1}.{2}]", ancestorAppendString, model.Name, field.SelectCommon.Index);
    }

    private string GetReadOnlyCommonRecordValueText(ViewModelViewModel.FieldViewModel field,
      ViewModelViewModel model, string ancestorAppendString = "")
    {
      var commonRecordsIndexer = GetCommonRecordsIndexer(field, model, ancestorAppendString);
      return string.Format(@"{{{{{0}{1}.{2} | commonRecord: {3}.{4}{5}}}}}",
        ancestorAppendString,
        model.Name,
        field.FieldName,
        field.SelectCommon.ObjectName,
        field.SelectCommon.CommonInformationName,
        commonRecordsIndexer);
    }

    private void AppendSelectList(TreeTagBuilder tag, ViewModelViewModel.FieldViewModel field,
      ViewModelViewModel model, bool withEdit, string ancestorAppendString)
    {
      var commonRecordsIndexer = GetCommonRecordsIndexer(field, model, ancestorAppendString);
      tag.AppendChild("select")
        .WithCssClass("form-control")
        .WithOptionalAttributeIf("ng-readonly", field.SelectCommon.GetReadOnlyExpression(commonRecordsIndexer),
          () => field.SelectCommon !=null && field.SelectCommon.RestrictUntilResolved )
        .WithOptionalAttributeIf("required", "required", () => field.IsRequired)
        .WithOptionalAttributeIf("id",
          string.Format("{0}FormControl{1}", field.FieldName, this.idCounter.Peek(field.FieldName)), () => withEdit)
        .WithAttribute("data-ng-model", string.Format("{0}{1}.{2}", ancestorAppendString, model.Name, field.FieldName))
        .WithAttribute("data-ng-options",field.SelectCommon.GetIteratorExpression("item", commonRecordsIndexer))
        .WithOptionalAttributeIf("disabled", "disabled", () => !withEdit)
        .AppendChild("option")
        .WithAttribute("value", string.Empty)
        .WithInnerText("---");
    }

    private void AppendEditControlForScalar(TreeTagBuilder tag, ViewModelViewModel.FieldViewModel field,
      ViewModelViewModel model, string ancestorAppendString)
    {
      var ngModelStr = string.Format("{0}{1}.{2}", ancestorAppendString, model.Name, field.FieldName);
      var fieldControlType = "text";
      var type = field.DataType.GetNonNullableType();
      if (type == typeof (bool))
      {
        var masterDiv = tag.AppendChild("div")
          .WithCssClass("radio-yes-no");

        var trueRadioId = string.Format("{0}-{1}-true-radio", model.Name, field.FieldName);

        var occ = this.idCounter.Count(trueRadioId);

        if (occ > 0)
        {
          trueRadioId += occ;
        }

        var falseRadioId = string.Format("{0}-{1}-false-radio", model.Name, field.FieldName);

        if (occ > 0)
        {
          falseRadioId += occ;
        }

        masterDiv.AppendChild("input")
          .WithAttributes(new
          {
            type = "radio",
            data_ng_model = ngModelStr,
            id = trueRadioId,
            data_ng_value = "true",
          });

        masterDiv.AppendChild("label")
          .WithAttribute("for", trueRadioId)
          .WithInnerText("Yes");

        masterDiv.AppendChild("input")
          .WithAttributes(new
          {
            type = "radio",
            data_ng_model = ngModelStr,
            id = falseRadioId,
            data_ng_value = "false",
          });

        masterDiv.AppendChild("label")
          .WithAttribute("for", falseRadioId)
          .WithInnerText("No");

        return;
      }
      if (type == typeof (DateTime))
      {
        fieldControlType = "date";
      }

      if (type == typeof (decimal))
      {
        tag = tag
          .AppendChild("div")
          .WithCssClass("input-group");

        tag.AppendChild("span")
          .WithCssClass("input-group-addon")
          .WithInnerText("$");
      }

      if (type == typeof (int) || type == typeof (decimal) || type == typeof (double))
      {
        fieldControlType = "number";
      }

      tag.AppendChild("input")
        .WithAttribute("type", fieldControlType)
        .WithOptionalAttributeIf("ui-mask", field.InputMask, () => field.InputMask != null)
        .WithCssClass("form-control")
        .WithOptionalAttributeIf("required", "required", () => field.IsRequired)
        .WithOptionalAttributeIf("step", "1", () => type == typeof (int))
        .WithOptionalAttributeIf("step", ".01", () => type == typeof (decimal))
        .WithOptionalAttributeIf("step", "any", () => type == typeof (double))
        .WithAttribute("id", string.Format("{0}FormControl{1}", field.FieldName, this.idCounter.Peek(field.FieldName)))
        .WithAttribute("data-ng-model", ngModelStr);
    }

    internal TreeTagBuilder GenerateHtmlDisplayTableTag(
      ViewModelViewModel model, 
      bool withEdit,
      ICollection<TreeTagBuilder> childForms,
      string deleteAction = "delete", 
      string editAction = "edit", 
      string addAction = "add", 
      string prefix = null,
      IList<string> ancestors = null, 
      bool isLedger = false, 
      string ledgerSumProperty = null,
      SubordinateItemActionAttribute itemActionAttribute = null)
    {
      if (model.IsViewOnly)
      {
        withEdit = false;
      }

      ancestors = (ancestors ?? new List<string>()).ToList();
      var ancestorAppendString = AncestorAppendStringFormatter.GetAncestorAppendString(ancestors);
      var prefixString = string.Empty;
      if (!string.IsNullOrWhiteSpace(ancestorAppendString))
      {
        prefixString = ancestorAppendString.TrimEnd('_') + ".";
      }
      else if (!string.IsNullOrWhiteSpace(prefix))
      {
        prefixString = prefix + ".";
      }
      var masterDiv = new TreeTagBuilder("div")
        .WithCssClass("multiple-item-display-table-region");
      var table = masterDiv.AppendChild("table")
        .WithAttribute("ts-wrapper", "ts-wrapper")
        .WithCssClass("table table-bordered table-condensed table-striped table-hover table-responsive");
      var thead = table.AppendChild("thead");
      var fields = model.Scalars.Where(f => !f.IsHidden).ToList();
      var collections = model.PrimitiveCollections.Where(pc => !pc.IsHidden).ToList();
      var headRow = thead.AppendChild("tr");
      foreach (var field in fields.Concat(collections))
      {
        var f = field;
        headRow.AppendChild("th")
          .WithInnerText(field.DisplayName)
          .WithAttribute("ts-criteria", field.FieldName.ToCamelCase() + this.GetTsSortCriterion(field.DataType))
          .WithOptionalAttributeIf("ts-default", "ts-default", () => f.IsDefaultSort);
      }

      if (withEdit && !isLedger)
      {
        headRow.AppendChild("th").WithInnerText("Actions");
      }
      else if (itemActionAttribute != null)
      {
        headRow.AppendChild("th").WithInnerText(itemActionAttribute.ColumnHeadingText);
      }

      if (isLedger)
      {
        headRow.AppendChild("th").WithInnerText("Balance");
      }
      var tbody = table.AppendChild("tbody");

      var ccName = model.Name.ToCamelCase();

      var bodyRow = tbody.AppendChild("tr")
        .WithAttribute("data-ng-repeat", string.Format("{0} in {1}{2}", ccName, prefixString, ccName.Pluralize()))
        .WithAttribute("ts-repeat", "ts-repeat");

      foreach (var field in fields)
      {
        var td = bodyRow.AppendChild("td");
        td.WithInnerText(field.SelectCommon == null
          ? this.GetFieldDisplayCode(model, field)
          : this.GetReadOnlyCommonRecordValueText(field, model));
      }

      foreach (var field in collections)
      {
        bodyRow
          .AppendChild("td")
          .AppendChild("ul")
          .AppendChild("li")
          .WithAttribute("data-ng-repeat", string.Format("item in {0}.{1}", model.Name, field.FieldName))
          .WithInnerText("{{item}}");
      }

      if (itemActionAttribute != null)
      {
        bodyRow
          .AppendChild("td")
          .AppendChild("a")
          .WithAttribute(itemActionAttribute.AttributeName, itemActionAttribute.AttributeValue)
          .WithCssClass(itemActionAttribute.AreaClass)
          .AppendChild("span")
          .WithCssClass(itemActionAttribute.ActionClass)
          //.AppendChild("a")
          .WithInnerText(itemActionAttribute.ActionText);
      }

      var target = string.Concat(ancestorAppendString, ccName).WithFirstLetterUppercased();

      if (withEdit && !isLedger)
      {
        var btnGroup = bodyRow.AppendChild("td")
          .AppendChild("div")
          .WithCssClass("btn-group")
          .WithAttribute("role", "group");

        btnGroup.AppendChild("button")
          .WithAttribute("type", "button")
          .WithCssClass("btn btn-default")
          .WithAttribute("data-ng-click",
            string.Format("{0}{1}At($index)", editAction, target))
          .WithInnerText("Edit/view details");

        btnGroup.AppendChild("button")
          .WithAttribute("type", "button")
          .WithCssClass("btn btn-danger")
          .WithAttribute("data-ng-click",
            string.Format("{0}{1}At($index)", deleteAction, target))
          .WithInnerText("Delete");
      }

      if (isLedger)
      {
        bodyRow.AppendChild("td")
          .WithInnerText(string.Format("{{{{_(_({0}{1}).slice(0, $index + 1).value()).sum('{2}') | currency}}}}",
            prefixString, ccName.Pluralize(), ledgerSumProperty));
      }

      if (withEdit)
      {
        table
          .AppendChild("tfoot")
          .AppendChild("tr")
          .AppendChild("td")
          .WithAttribute("colspan", (fields.Count + 1).ToString("D"))
          .AppendChild("button")
          .WithAttribute("type", "button")
          .WithCssClass("btn")
          .WithCssClass(prefix != null ? "btn-default" : "btn-primary")
          .WithCssClass("pull-right")
          .WithAttribute("data-ng-click",
            string.Format("{0}{1}()", addAction, target))
          .WithInnerText("Add new");

        childForms.Add(this.GenerateHtmlFormTag(model, withEdit, childForms,
          enclosingModal: ancestorAppendString + model.Name.ToCamelCase() + "Modal", ancestors: ancestors));
      }

      return masterDiv;
    }

    private string GetFieldDisplayCode(ViewModelViewModel vmVm, ViewModelViewModel.FieldViewModel fieldVm,
      string ancestorAppendString = "")
    {
      var type = fieldVm.DataType.GetNonNullableType();

      var append = string.Empty;

      if (type == typeof (DateTime))
      {
        append = " | date: 'shortDate'";
      }
      if (type == typeof (decimal))
      {
        append = " | currency";
      }

      if (type == typeof (bool))
      {
        append = " | yesNo";
      }

      var sb = new StringBuilder("{{")
        .Append(ancestorAppendString)
        .Append(vmVm.Name.ToCamelCase())
        .Append(".")
        .Append(fieldVm.FieldName.ToCamelCase())
        .Append(append)
        .Append("}}");

      return sb.ToString();
    }

    private string GetTsSortCriterion(Type type)
    {
      type = type.GetNonNullableType();
      if (type == typeof (string))
      {
        return " | lowercase";
      }
      if (type == typeof (int) || type == typeof (long))
      {
        return " | parseInt";
      }
      if (type == typeof (float) || type == typeof (double) || type == typeof (decimal))
      {
        return " | parseFloat";
      }
      if (type == typeof (DateTime))
      {
        return " | parseDate"; //TODO:  I have to implement this custom code
      }
      return string.Empty;
    }
  }
}