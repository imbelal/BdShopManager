using System.Text;

namespace Common.NlogExtentions
{
    public interface IExceptionRendererService
    {
        /// <summary>
        /// Render additional exception information for a given exception if a [IExceptionRenderer] is registered.
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <param name="exception"></param>
        void RenderAdditionalExceptionInformation(
            StringBuilder stringBuilder,
            Exception exception
        );

        /// <summary>
        /// Return a message object for a exception if a [IExceptionRendererWithMessageObject] is registered.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        object GetMessageObject(
            Exception exception
        );
    }
}
