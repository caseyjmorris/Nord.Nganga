using System;

namespace Nord.AngularUiGen.Annotations.Attributes.Angular
{
  /// <summary>
  /// Transition to a different UI router state on success.
  /// JavaScript embedded via JavaScriptOnPostCompleteAttribute
  /// </summary>
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
  public class SuccessStateOnPostCompleteAttribute : JavaScriptOnPostCompleteAttribute
  {
    /// <summary>
    /// If specified, will be used as the body of the route parameters passed to the $state service.
    /// </summary>
    public string RouteParameters { get; set; }

    private readonly string destinationState;

    public override string Expression
    {
      get
      {
        return this.RouteParameters != null
          ? string.Format("$state.go('{0}', {{{1}}});", this.destinationState, this.RouteParameters)
          : string.Format("$state.go('{0}');", this.destinationState);
      }
    }

    /// <summary>
    /// Create a new SuccessStateOnPostCompleteAttribute.
    /// </summary>
    /// <param name="destinationState">success destination</param>
    public SuccessStateOnPostCompleteAttribute(string destinationState) : base(null)
    {
      this.destinationState = destinationState;
    }
  }
}