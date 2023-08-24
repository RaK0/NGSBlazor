using NGSBlazor.Client.Interfaces.Authentication;
using NGSBlazor.Client.Interfaces.Services;
using NGSBlazor.Client.LocalItems;
using NGSBlazor.Client.Services;
using NGSBlazor.Shared.Wrapper.Result;
using System.Net.Http.Headers;
using System.Net.Http;
using MediatR;

namespace NGSBlazor.Client.Authentication
{
    internal class AuthetnicateDelegatingHandler : DelegatingHandler
    {
        readonly IAuthenticationManager _authenticationManager;

        public AuthetnicateDelegatingHandler(IAuthenticationManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
        }
        
        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Task.Run(()  => {

                SetTokenIfTrue(_authenticationManager.CheckTokenNeedRefresh(cancellationToken), request); 

            }, cancellationToken);
            return base.Send(request, cancellationToken);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            SetTokenIfTrue(_authenticationManager.CheckTokenNeedRefresh(cancellationToken), request);

            return base.SendAsync(request, cancellationToken);
        }

        async void SetTokenIfTrue(Task<IResult<BearerLocalItem>> resultTask, HttpRequestMessage request)
        {
            IResult<BearerLocalItem> result = await resultTask;
            if(result?.Succeeded == true) 
            {
                BearerLocalItem? bearer = result.Data;
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearer?.Token);

            }
        }
    }
}
