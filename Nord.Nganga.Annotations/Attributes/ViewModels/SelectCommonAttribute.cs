using System;

namespace Nord.Nganga.Annotations.Attributes.ViewModels
{
  /// <summary>
  /// Property maps to the key of a common record
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class SelectCommonAttribute : Attribute
  {
    private string providerExpression;

    public string NotResolvedExpression
    {
      get { return string.Format("(!{0}.{1}.$resolved)", this.ObjectName, this.CommonInformationName); }
    }

    public string GetReadOnlyExpression(string indexer = "")
    {
      return string.Format(
        "(!{0}.{1}.$resolved){2}",
        this.ObjectName,
        this.CommonInformationName,
        string.IsNullOrEmpty(indexer)
          ? ""
          : string.Format(" || (!{0}.{1}{2})", this.ObjectName, this.CommonInformationName, indexer)
        );
    }

    public string QualifiedName
    {
      get { return string.Format("{0}.{1}", this.ObjectName, this.CommonInformationName); }
    }

    public string GetIteratorExpression(string elementName = "choice", string indexer = "")
    {
      return string.Format("{0}.id as {0}.name for {0} in {1}{2}", elementName, this.QualifiedName, indexer);
    }

    /// <summary>
    /// Function to resolve the values of this common record
    /// </summary>
    public string ProviderExpression
    {
      get
      {
        if (this.providerExpression != null)
        {
          return this.providerExpression;
        }
        var chrArr = this.CommonInformationName.ToCharArray();
        chrArr[0] = char.ToUpperInvariant(chrArr[0]);
        var st = new string(chrArr);
        return string.Format("commonRecordsService.get{0}(); ", st);
      }
      set { this.providerExpression = value; }
    }

    /// <summary>
    /// The name of the common records
    /// </summary>
    public string CommonInformationName { get; set; }

    /// <summary>
    /// The name of the object attached to the $scope which will be looked up for values.
    /// </summary>
    public string ObjectName { get; set; }

    /// <summary>
    /// If the common record is a dictionary, this field specifies the value that will be used as its key.
    /// </summary>
    public string Index { get; set; }


    /// <summary>
    /// if true, the generated control will be attributed with ng-readonly="(!common.{commonInformationName}.$resolved)"
    /// further, any subordinate controls will be ng-readonly until such time as a value 
    /// is selected on the 'parent' - that is to say that if the Index property is not empty
    /// then the ng-readonly attribute will also contain "|| (!common.{commonInformationName}[{Index}])"
    /// </summary>
    public bool RestrictUntilResolved { get; set; }

    /// <summary>
    /// Generates a new <code>SelectCommonAttribute</code>
    /// </summary>
    /// <param name="commonInformationName">The name of the common records</param>
    public SelectCommonAttribute(string commonInformationName)
    {
      if (string.IsNullOrEmpty(commonInformationName))
      {
        throw new ArgumentException();
      }
      this.CommonInformationName = commonInformationName;
      this.RestrictUntilResolved = true;
      this.ObjectName = "common";
    }
  }
}