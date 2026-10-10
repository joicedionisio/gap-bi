namespace GapBi.Api.Models;

public class SyncLog
{
    public int Id { get; set; }
    public DateTime SyncedAt { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}