namespace Walterlv.CloudTyping
{
    public readonly struct TypingResponse
    {
        public TypingResponse(bool success)
        {
            Success = success;
        }

        public bool Success { get; }
    }
}
