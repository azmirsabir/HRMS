using System.ComponentModel;

namespace Core.Domain.Authentications.Enums;

public enum Roles
{
    [Description("Employee")] Employee = 1,
    [Description("Manager")] Manager = 2,
    [Description("Auditor")] Auditor = 3
}