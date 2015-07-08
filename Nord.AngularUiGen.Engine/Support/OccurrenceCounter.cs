using System;
using System.Collections.Generic;

namespace Nord.AngularUiGen.Engine.Support
{
  /// <summary>
  /// Keep track of the number of times you have used a value.
  /// </summary>
  /// <typeparam name="T">An equatable object</typeparam>
  public class OccurrenceCounter<T> where T : IEquatable<T>
  {
    private readonly Dictionary<T, int> _dict = new Dictionary<T, int>();

    private readonly int _offset;

    /// <summary>
    /// Create a new occurence counter.
    /// </summary>
    /// <param name="offset">All counts will start here.</param>
    public OccurrenceCounter(int offset = 0)
    {
      this._offset = offset;
    }

    /// <summary>
    /// Get the count.
    /// </summary>
    /// <param name="input">Value to look up</param>
    /// <returns>Previous references plus current count</returns>
    public int Count(T input)
    {
      if (this._dict.ContainsKey(input))
      {
        return ++this._dict[input];
      }
      this._dict[input] = this._offset;
      return this._offset;
    }

    /// <summary>
    /// View count without incrementing
    /// </summary>
    /// <param name="input">Value to look up</param>
    /// <returns>Previous references</returns>
    /// <remarks>
    /// If the value is not in the collection it will return one less than the offset.
    /// </remarks>
    public int Peek(T input)
    {
      int val;
      if (this._dict.TryGetValue(input, out val))
      {
        return val;
      }
      return this._offset - 1;
    }
  }
}