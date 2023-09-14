using System;

namespace SendNotificaton.DataContext;

public partial class ExecutionLog
{
    public int LogId { get; set; }

    public string ProcedureName { get; set; } = string.Empty;

    public DateTime? LastExecutionTime { get; set; }
}
