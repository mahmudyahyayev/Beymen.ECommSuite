namespace BuildingBlocks.Core.Exception
{
    public enum Severity : byte
    {
        SystemError = 0,
        Error = 1,
        AuditFailure = 2,
        AccessDenied = 3,
        AssigneeNotSpecified = 4,
        ShowConfirmation = 5,
        WorkflowException = 6,
        IgnorableErrorForTransaction = 7,
        ValidationError = 8,
        UnhandledException = 9
    }
}
