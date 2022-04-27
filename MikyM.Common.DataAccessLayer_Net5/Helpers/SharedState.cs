namespace MikyM.Common.DataAccessLayer_Net5.Helpers
{
    internal static class SharedState
    {
        internal static bool DisableOnBeforeSaveChanges { get; set; } = false;
    }
}