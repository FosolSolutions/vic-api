using System.Net.Http;

namespace Fosol.Core.Http
{
    /// <summary>
    /// LoggingHandler class, provides a way to debug HTTP request and response made by the HttpClient.
    /// </summary>
    /// <example>
    /// this.Client = new HttpClient(new LoggingHandler(new HttpClientHandler()
    /// {
    ///     ClientCertificateOptions = ClientCertificateOption.Manual,
    ///     ServerCertificateCustomValidationCallback = (sender, cert, chain, errors) => {
    ///         return HttpClientHandler.DangerousAcceptAnyServerCertificateValidator(sender, cert, chain, errors);
    ///     }
    /// }));
    /// 
    /// </example>
    public class LoggingHandler : DelegatingHandler
    {
        public LoggingHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected override async System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            System.Diagnostics.Debug.WriteLine("Request:");
            System.Diagnostics.Debug.WriteLine(request.ToString());
            if (request.Content != null)
            {
                System.Diagnostics.Debug.WriteLine(await request.Content.ReadAsStringAsync());
            }
            System.Diagnostics.Debug.WriteLine("");

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            System.Diagnostics.Debug.WriteLine("Response:");
            System.Diagnostics.Debug.WriteLine(response.ToString());
            if (response.Content != null)
            {
                System.Diagnostics.Debug.WriteLine(await response.Content.ReadAsStringAsync());
            }
            System.Diagnostics.Debug.WriteLine("");

            return response;
        }
    }
}
