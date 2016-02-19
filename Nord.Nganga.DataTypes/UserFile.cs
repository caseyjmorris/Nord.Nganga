using System;

namespace Nord.Nganga.DataTypes
{
  /// <summary>
  /// Represents a file uploaded by a user
  /// </summary>
  public class UserFile
  {
    /// <summary>
    /// ID of file in database
    /// </summary>
    public int Id { get; set; }

/*    /// <summary>
    /// Uri to view existing file.  For new file, data URI representing file body
    /// </summary>
    public Uri Uri { get; set; }*/

    /// <summary>
    /// MIME type of file
    /// </summary>
    public string MimeType { get; set; }

    /// <summary>
    /// Original name of file
    /// </summary>
    public string FileName { get; set; }

    public byte[] Content { get; set; }

    /// <summary>
    /// Document type ID, if any
    /// </summary>
    public int? DocumentTypeId { get; set; }
  }
}