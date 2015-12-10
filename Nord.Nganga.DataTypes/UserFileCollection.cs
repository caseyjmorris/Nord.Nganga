using System.Collections.Generic;

namespace Nord.Nganga.DataTypes
{
  /// <summary>
  /// Collection of user files
  /// </summary>
  public class UserFileCollection : List<UserFile>
  {
    /// <summary>
    /// Create a new instance of <see cref="UserFileCollection"/>
    /// </summary>
    /// <param name="source">Elements to be added to the collection</param>
    public UserFileCollection(IEnumerable<UserFile> source) : base(source)
    {
    }

    /// <summary>
    /// Create a new instance of <see cref="UserFileCollection"/>
    /// </summary>
    public UserFileCollection()
    {
    }
  }
}