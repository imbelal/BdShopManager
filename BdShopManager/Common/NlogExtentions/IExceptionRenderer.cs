using System.Text;

namespace Common.NlogExtentions
{
    /// <summary>
    /// Render additional information for a specific type of exception.
    /// </summary>
    public interface IExceptionRenderer
    {
        bool DoesMatchException(Exception exception);

        void RenderException(StringBuilder stringBuilder, Exception exception);
    }
}
