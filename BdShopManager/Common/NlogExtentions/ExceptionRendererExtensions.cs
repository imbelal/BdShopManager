namespace Common.NlogExtentions
{
    public static class ExceptionRendererExtensions
    {
        /// <summary>
        /// Does any exception renderer match with the given exception?
        /// </summary>
        /// <param name="exceptionRenderers"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static bool AnyMatchException(
            this IEnumerable<IExceptionRenderer> exceptionRenderers,
            Exception exception
        )
        {
            return exceptionRenderers.Any(renderer => renderer.DoesMatchException(exception));
        }

        /// <summary>
        /// Returns the first exception renderer which matches with the exception.
        /// </summary>
        /// <param name="exceptionRenderers"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static IExceptionRenderer FirstMatchException(
            this IEnumerable<IExceptionRenderer> exceptionRenderers,
            Exception exception
        )
        {
            return exceptionRenderers.First(renderer => renderer.DoesMatchException(exception));
        }
    }
}
