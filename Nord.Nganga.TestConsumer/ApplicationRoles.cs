using System.Collections.Generic;

namespace Nord.Nganga.TestConsumer
{
  /// <summary>
  /// Potential roles for users to have in application
  /// </summary>
  public static class ApplicationRoles
  {
    /// <summary>
    /// Regular MAP staff
    /// </summary>
    public const string PatientAssistanceAssociate = "PatientAssistanceAssociate";

    /// <summary>
    /// Managers with the ability to edit program definitions
    /// </summary>
    public const string PatientAssistanceManager = "PatientAssistanceManager";

    /// <summary>
    /// Automated service to sync financial records
    /// </summary>
    public const string FinanceSyncService = "FinanceSyncService";

    /// <summary>
    /// User with full access to entire system
    /// </summary>
    public const string Administrator = "Administrator";

    /// <summary>
    /// Third-party referrer
    /// </summary>
    public const string Referrer = "Referrer";

    /// <summary>
    /// User in Finance department
    /// </summary>
    public const string FinanceUser = "FinanceUser";

    /// <summary>
    /// Manager in Finance department
    /// </summary>
    public const string FinanceManager = "FinanceManager";

    public static readonly IEnumerable<string> AllRoles = new[]
    {
      PatientAssistanceAssociate,
      PatientAssistanceManager,
      FinanceSyncService,
      Administrator,
      Referrer,
      FinanceManager,
      FinanceUser
    };
  }
}