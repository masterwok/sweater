namespace Sweater.Core.Models
{
    public struct ErrorResponse
    {
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Error { get; }

        public ErrorResponse(string message)
        {
            Error = message;
        }
    }
}