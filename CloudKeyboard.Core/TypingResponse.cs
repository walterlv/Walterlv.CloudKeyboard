namespace Walterlv.CloudTyping
{
    public readonly struct TypingResponse
    {
        public TypingResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public bool Success { get; }

        public string Message { get; }
    }
}
