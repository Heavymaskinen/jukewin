using System.Threading;

namespace Juke.UI
{
    public class LoaderCancellationTokenProvider
    {
        private static CancellationTokenSource tokenSource;
        public static CancellationToken Token
        {
            get 
            {
                if (tokenSource == null)
                {
                    tokenSource = new CancellationTokenSource();
                }

                return tokenSource.Token;
            }
        }

        public static void Dispose()
        {
            tokenSource?.Dispose();
            tokenSource = null;
        }
    }
}
