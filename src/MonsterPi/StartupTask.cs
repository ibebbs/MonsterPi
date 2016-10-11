using Devkoes.Restup.WebServer.Http;
using Devkoes.Restup.WebServer.Rest;
using Windows.ApplicationModel.Background;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace MonsterPi
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;
        private HttpServer _httpServer;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            IoService ioService = IoService.Instance;

            _httpServer = new HttpServer(8800);

            var restRouteHandler = new RestRouteHandler();

            restRouteHandler.RegisterController<MainsController>();

            _httpServer.RegisterRoute("api", restRouteHandler);

            await _httpServer.StartServerAsync();
        }
    }
}
