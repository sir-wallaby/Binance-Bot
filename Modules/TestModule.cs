using Discord.Commands;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using CoinMarketCap;

namespace Bittrex_Bot.Modules
{    public class TestModule : ModuleBase
    {
        [Command("test")] //how to actually call the bot to do something
        [Remarks("blah blah, testing bot code")]
        public async Task test()
        {
            try
            {
                await ReplyAsync(DefineAnewMethod());
            }
            catch (Exception e)
            {
                await ReplyAsync(e.ToString());
            }
        }

        public string DefineAnewMethod()
        {

            String someText;
            someText = " Hi, I am a  method, I just want to be represented";

            return someText;

        }




        //public string  CoinMarketCap()
        //{

        //    string CnMktCap;
        //    CnMktCap = "https://api.coinmarketcap.com/v1/ticker/bitcoin";

        //    JObject coinMarketCapTop50 = JObject.Parse(CnMktCap);
        //    IList<JToken> results = coinMarketCapTop50[0].Children().ToList();

        //    IList<SearchResult> searchResults = new List<SearchResult>();
        //    foreach(JToken result in results)
        //    {
        //        SearchResult searchResult = result.ToObject<SearchResult>();
        //        searchResults.Add(searchResult);
        //    }
        //   return searchResults.Where(t => t.Symbol == "BTC").ToString();
        //}

        //public async Task CoinMarketCap()
        //{
        //    var client = CoinMarketCapClient.GetInstance();
        //    var tickerList = await client.GetTickerListAsync();

        //    var value = tickerList.First().PercentChange1h;

        //    //Console.WriteLine(value);
        //}


        [Command("$")] //how to actually call the bot to do something
        [Remarks("testing a ticker from Coin MarketCap")]
        public async Task CoinMarketCapOutput(string _coin)
        {
            var client = CoinMarketCapClient.GetInstance();
            //var tickerList = await client.GetTickerListAsync();
            var ticker = await client.GetTickerAsync(_coin);
            var LastHour = ticker.PercentChange1h;
            var USDValue = ticker.PriceUsd;



            await ReplyAsync(
                "```" + "Last Hour" + LastHour.ToString() + "\n" +
                "USD Value: $" + USDValue.ToString() + 






                "```"
                );
           
        }


    }
}
