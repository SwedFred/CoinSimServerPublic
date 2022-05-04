using System;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoinSim.Clients;

namespace CoinSim.Hubs
{
  public class CryptoHub : Hub
  {
    private readonly CoinGecko coinGecko;
    public CryptoHub(CoinGecko coinGecko)
    {
      this.coinGecko = coinGecko;
    }

    public async Task JoinCryptoUpdateGroup()
    {
      await Groups.AddToGroupAsync(Context.ConnectionId, "cryptocoins");
      var coins = await coinGecko.GetCurrencies();
      await Clients.Caller.SendAsync("CoinUpdate", coins);
    }

    public async Task GetCurrencies()
    {
      var coins = await coinGecko.GetCurrencies();
      await Clients.Caller.SendAsync("CoinUpdate", coins);
    }
  }
}
