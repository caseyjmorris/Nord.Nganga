using System;

namespace Nord.AngularUiGen.Annotations.Attributes.Angular
{
  /// <summary>
  /// Allows the embedding of javascript into generated controllers.
  /// ContextType determines if the code should be injected into the success or failure handlers.
  /// </summary>
  /// <remarks>
  /// Only supported for HTTP POST methods.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
  public class JavaScriptOnPostCompleteAttribute : Attribute
  {
    /// <summary>
    /// The body of the JavaScript statement to be run.
    /// </summary>
    public virtual string Expression { get; private set; }

    /// <summary>
    /// Determines whether this statement should be injected into the success or failure handler.
    /// </summary>
    public ContextType Context { get; set; }

    /// <summary>
    /// Create a new JavaScriptOnPostCompleteAttribute.
    /// </summary>
    /// <param name="expression">Body of statement.</param>
    public JavaScriptOnPostCompleteAttribute(string expression)
    {
      this.Expression = expression;
      this.Context = ContextType.Success;
    }

    /// <summary>
    /// Determines which handler the statement will be inserted into.
    /// </summary>
    public enum ContextType
    {
      /// <summary>
      /// The on success handler
      /// </summary>
      Success,

      /// <summary>
      /// The on failure handler
      /// </summary>
      Failure
    }
  }
}