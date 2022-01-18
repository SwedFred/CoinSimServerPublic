using System;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoinSim.Clients;

namespace CoinSim.Hubs
{
  // Här joinar vi grupper och ser till att skicka ut information till olika klienter, samt manuella force updates.
  // Fundering, hur hanterar jag cachen? Ska jag läsa in till SQLite databas inklusive ha ett in-memory object som sedan uppdateras samtidigt som dbn?
  // Då kan jag göra så att vid start-up så läser jag in DB till minnet innan vi fetchar data från API.
  // En annan fråga är: Ska jag erbjuda att spara användare i DB också så att deras "wallets" sparas? Det kan vara en paid-for feature.
  // Den här klassen är till för att registrera till grupper samt pusha data vid behov.
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
      var coins = coinGecko.GetCurrencies();
      await Clients.Caller.SendAsync("CoinUpdate", coins);
    }

    public async Task GetCurrencies()
    {
      var coins = coinGecko.GetCurrencies();
      var kek = Context.ConnectionId;
      await Clients.Caller.SendAsync("CoinUpdate", coins);
    }
  }
}
