using System;

namespace SendNotificaton.DataContext;

public partial class Record
{
    public int Id { get; set; }

    public string RecordId { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string UserEmail { get; set; } = string.Empty;

    public string DataValue { get; set; } = string.Empty;

    public bool NotificationFlag { get; set; }

    public DateTime? CreatedTime { get; set; }
}
