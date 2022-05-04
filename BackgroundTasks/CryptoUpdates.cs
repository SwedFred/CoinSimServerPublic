using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoinSim.Clients;
using CoinSim.Hubs;

namespace CoinSim.BackgroundTasks
{
    //  Background task to update the CoinGecko cache every 5 minutes
    public class CryptoUpdates : IHostedService
    {
        private CoinGecko coinGecko;
        private IHubContext<CryptoHub> hub;
        private Timer _timer;

        public CryptoUpdates(IHubContext<CryptoHub> _hub, CoinGecko _coinGecko)
        {
            coinGecko = _coinGecko;
            hub = _hub;
        }

        /// <summary>
        /// Periodically refreshes the application cache
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(RefreshCacheAndPush, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
            RefreshCacheAndPush(null);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Pauses the timer indefinately, but does not destroy it
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Refreshes the cache in CoinGecko and pushes the new data to the clients
        /// </summary>
        /// <param name="state"></param>
        private async void RefreshCacheAndPush(object state)
        {
            try
            {
                var coins = await coinGecko.RefreshCurrencies();
                if (coins != null && coins.Any())
                    await hub.Clients.Group("cryptocoins").SendAsync("CoinUpdate", coins);
                else
                    await hub.Clients.Group("cryptocoins").SendAsync("CoinUpdateFail");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
