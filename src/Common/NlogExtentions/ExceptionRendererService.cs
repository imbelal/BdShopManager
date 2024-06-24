using System.Text;

namespace Common.NlogExtentions
{
    public class ExceptionRendererService : IExceptionRendererService
    {

        private readonly IEnumerable<IExceptionRenderer> _exceptionRenderers;

        public ExceptionRendererService(
            IEnumerable<IExceptionRenderer> exceptionRenderers
        )
        {
            this._exceptionRenderers = exceptionRenderers;
        }

        public void RenderAdditionalExceptionInformation(
            StringBuilder stringBuilder,
            Exception exception
        )
        {
            if (this._exceptionRenderers.AnyMatchException(exception))
            {
                this._exceptionRenderers
                    .FirstMatchException(exception)
                    .RenderException(
                        stringBuilder: stringBuilder,
                        exception: exception
                    );
            }
        }

        public object GetMessageObject(
            Exception exception
        )
        {
            if (this._exceptionRenderers.OfType<IExceptionRendererWithMessageObject>().AnyMatchException(exception))
            {
                return ((IExceptionRendererWithMessageObject)this._exceptionRenderers
                    .OfType<IExceptionRendererWithMessageObject>()
                    .FirstMatchException(exception))
                    .GetMessageObject(
                        exception: exception
                    );
            }
            return null;
        }
    }
}
