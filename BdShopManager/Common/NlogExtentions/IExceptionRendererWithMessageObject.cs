namespace Common.NlogExtentions
{
    /// <summary>
    /// Return a message object for a specific type of exception.
    /// </summary>
    public interface IExceptionRendererWithMessageObject : IExceptionRenderer
    {
        object GetMessageObject(Exception exception);
    }
}
