using System;
using System.Collections.Generic;

namespace Nord.AngularUiGen.Engine.JavaScript
{
  public class EndpointViewModel
  {
    public string MethodName { get; set; }
    public IEnumerable<string> Arguments { get; set; }
    public bool HasReturnValue { get; set; }
    public bool ReturnsIEnumerable { get; set; }
    public HttpMethodType HttpMethod { get; set; }
    public Type ReturnType { get; set; }
    public IEnumerable<Type> ArgumentTypes { get; set; }
    public IEnumerable<string> SpecialAuthorization { get; set; }
    public string SectionHeader { get; set; }
    public IEnumerable<string> OnPostSuccessExpressions { get; set; }
    public IEnumerable<string> OnPostFailureExpressions { get; set; }
    public bool ResourceOnly { get; set; }

    public enum HttpMethodType
    {
      Get,
      Post,
    }
  }
}