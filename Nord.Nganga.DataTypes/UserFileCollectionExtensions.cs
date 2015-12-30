using System.Collections.Generic;

namespace Nord.Nganga.DataTypes
{
  /// <summary>
  /// Extensions related to the <see cref="UserFileCollection"/> class
  /// </summary>
  public static class UserFileCollectionExtensions
  {
    /// <summary>
    /// Convert the collection to a <see cref="UserFileCollection"/>
    /// </summary>
    /// <param name="source">Elements to be added to the collection</param>
    /// <returns><see cref="UserFileCollection"/></returns>
    public static UserFileCollection ToUserFileCollection(this IEnumerable<UserFile> source)
    {
      return new UserFileCollection(source);
    }
  }
}