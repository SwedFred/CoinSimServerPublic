using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoinSim.Clients.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace CoinSim.Clients
{
    // Singleton service which caches the top 250 coins
    public class CoinGecko
    {
        private List<Cryptocoin> coinCache = new List<Cryptocoin>();
        private string Url { get; set; } = "";

        public CoinGecko(IConfiguration configuration)
        {
            Url = configuration.GetSection("ClientSettings").GetSection("CoingeckoUrl").Value;
        }

        public async Task<List<Cryptocoin>> RefreshCurrencies()
        {
            using (HttpClient client = new HttpClient())
            {
                // TODO: För stunden är allt hårdkodat. Känn dig välkommen att fixa detta om du har mer fritid än du vet vad du ska göra av!
                try
                {
                    string fullUrl = Url + "coins/markets?vs_currency=usd&order=market_cap_desc&per_page=250&page=1&sparkline=true&price_change_percentage=1h%2C24h";
                    var message = client.GetAsync(fullUrl);
                    var result = message.Result;
                    if (!result.IsSuccessStatusCode)
                        return null;
                    string responsebody = await result.Content.ReadAsStringAsync();
                    List<Cryptocoin> coins = JsonConvert.DeserializeObject<List<Cryptocoin>>(responsebody);
                    coinCache = coins;
                    return coins;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                return null;
            }
        }

        public async Task<List<Cryptocoin>> GetCurrencies()
        {
            if (coinCache.Any())
                return await Task.FromResult(coinCache);
            else
                return null;
        }
    }
}
