using System.Net;
using System.Text;

namespace ACB_Prueba_Tecnica.Tests.Mocks
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _response;

        public MockHttpMessageHandler(string response = null)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent(_response ?? "[]", Encoding.UTF8, "application/json")
            });
        }
    }
}
