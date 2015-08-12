using System;

namespace Nord.Nganga.Annotations.Attributes.Angular
{
  /// <summary>
  /// When placed on a WebAPI controller, allows the injection of raw JavaScript into
  /// the generated AngularJS controller.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public class InjectJavaScriptAttribute : Attribute
  {
    /// <summary>
    /// The JavaScript statement to be executed.
    /// </summary>
    public string Content { get; private set; }

    /// <summary>
    /// The position in which to inject the JavaScript statement
    /// </summary>
    public Position ControllerPosition { get; set; }

    /// <summary>
    /// Creates a new InjectJavaScript attribute
    /// </summary>
    /// <param name="content">The statement body</param>
    public InjectJavaScriptAttribute(string content)
    {
      this.Content = content;
      this.ControllerPosition = Position.End;
    }

    /// <summary>
    /// The position in which to inject the JavaScript statement.  If not specified, the end position is used.
    /// </summary>
    public enum Position
    {
      /// <summary>
      /// Right after the function signature declaration
      /// </summary>
      Beginning,

      /// <summary>
      /// After all initialization is complete but before any server endpoints are called.
      /// </summary>
      AfterInitialization,

      /// <summary>
      /// After all GET methods to retrieve data on the current page are called but before methods referring to the 
      /// POST endpoints are declared.
      /// </summary>
      AfterGet,

      /// <summary>
      /// At the end of the function
      /// </summary>
      End,
    }
  }
}