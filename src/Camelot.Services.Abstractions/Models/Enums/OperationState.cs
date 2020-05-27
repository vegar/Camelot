namespace Camelot.Services.Abstractions.Models.Enums
{
    public enum OperationState : byte
    {
        NotStarted,
        InProgress,
        Paused,
        Blocked,
        Finished,
        Cancelling,
        Cancelled,
        Failed
    }
}