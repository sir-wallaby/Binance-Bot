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
        public async Task BinanceTicker([Remainder] string coin)
        {
            string _coin = coin.ToUpper();
            //default input parameter to be paired with BTC
            _coin = _coin + "BTC";
            Console.WriteLine(_coin);
            var apiStartingAddress = $"https://www.binance.com/api/v1/ticker/24hr?symbol={_coin}";
            var bitcoinDollarValue = $"https://www.binance.com/api/v1/ticker/24hr?symbol=BTCUSDT";

            //open web client
            var client = new WebClient();
            //download the json into a string
            string jsonRaw = client.DownloadString(apiStartingAddress);
            //download BTC value
            string BTCUSDT = client.DownloadString(bitcoinDollarValue);
            //parse the json into a raw object
            dynamic binanceObj = JObject.Parse(jsonRaw);
            //another json object to computer dollar values
            dynamic bitcoinObj = JObject.Parse(BTCUSDT);

            //Value to computer XXX coin to USDT values
            //real goal here needs to use USD but....
            double USDTprice = bitcoinObj["lastPrice"];

            //declare the variables that need to be output
            string symbol = binanceObj["symbol"];
            double lastPrice = binanceObj["lastPrice"];
            string twentyfourHourChange = binanceObj["priceChange"];
            double lastPriceDollaredOut = (USDTprice * lastPrice);

          


            await ReplyAsync(
                "```" +

                "Symbol: " + symbol + "\n" +

                "Last Price: " + lastPrice + "\n" +

                "24 Hour Change: " + twentyfourHourChange + "\n" +

                "Last Price USDT:" + lastPriceDollaredOut + "\n" 

                //"Symbol" + +

                //"Symbol" + +

                +"```");




        }

  
    }
}
