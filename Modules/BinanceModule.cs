using Discord.Commands;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;

namespace Bittrex_Bot.Modules
{
    public class BinanceModule : ModuleBase
    {
        //https://api.binance.com
        ///api/v1/ticker/allPrices
        [Command("b")] //Binance command
        [Summary("Returns the ticker for the specified coin/token from Binance")]
        public async Task BinanceTicker() //[Remainder] string coin)
        {
            //string _coin = coin;
            var apiStartingValue = $"https://api.binance.com/api/v1/ticker/allPrices";

            var client = new WebClient();

            string jsonRaw = client.DownloadString(apiStartingValue);

            JArray array = JArray.Parse(jsonRaw);

            foreach (JObject o in array.Children<JObject>())
            {
                foreach (JProperty p in o.Properties())
                {
                    string name = p.Name;
                    string value = (string)p.Value;
                    await ReplyAsync(
                               "```" + (name + " -- " + value)
                               + "```");
                    //Console.WriteLine(name + " -- " + value);
                }
            }

           
        }

        //[Command("b")] //Binance command
        //[Summary("Returns the ticker for the specified coin/token from Binance")]        
        //public async Task BinanceTicker([Remainder] string coin)
        //{
        //    string _coin = coin;
        //    var api = new BinanceApi();

        //    var book = await api.GetOrderBookAsync(Symbol.BTC_USDT);


        //    Console.WriteLine(book);

        //    await ReplyAsync(_coin);
        //}

    }
}
