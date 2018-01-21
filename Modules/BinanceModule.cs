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

    /// <summary>
    /// General api endpoint address for API calls
    /// //https://api.binance.com
    ///  /api/v1/ticker/allPrices
    ///
    ///  Pretty basic module to pull prices from Binance's public side API. 
    ///  this doesn't require API keys since it doesn't place orders (buy or sell)
    ///
    ///
    /// TODO: Add in price check versus ETH
    ///         Check for more errors that are caused within BTC calls
    ///         Figure out a way to pull back last hour price change
    ///         Change price output to include numbers that end in zero. EX. $1.90, right now it shows $1.9
    /// </summary>
    public class BinanceModule : ModuleBase
    {
        [Name("Coin Search")]
        [Command("b")] //Binance command
        [Summary("Returns the ticker for the specified coin/token from Binance" + " /n" + "Usage: .b eth ")]       
        public async Task BinanceTicker([Remainder] string coin)
        {
            var typing = Context.Message.Channel.EnterTypingState();
            string _coin = coin.ToUpper();
            //error checking to make tell if bitcoin was entered. Default to false.
            Boolean wasBitcoinEntered = false;


            try
            {
                //default input parameter to be paired with BTC
                _coin = _coin + "BTC";
                //if block when searching for BTC just default to BTCUSDT
                //also flip the bitcoin entered flag
                if (_coin == "BTCBTC")
                {
                    _coin = "BTCUSDT";
                    wasBitcoinEntered = true;

                }

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

                client.Dispose();

                //Value to computer XXX coin to USDT values
                //real goal here needs to use USD but....
                double USDTprice = bitcoinObj["lastPrice"];

                //declare the variables that need to be output
                string symbol = binanceObj["symbol"];
                double lastPrice = binanceObj["lastPrice"];
                double twentyfourHourChange = binanceObj["priceChangePercent"];



                //this one requires some logic before we can output the price.
                double lastPriceDollaredOut;
                if (wasBitcoinEntered == false)
                {
                    lastPriceDollaredOut = (USDTprice * lastPrice);

                }
                else
                {
                    lastPriceDollaredOut = (USDTprice);
                }

                var builder = new EmbedBuilder()
                {
                    Color = new Color(114, 137, 218),
                    Description =               

                    "Symbol: " + symbol.Replace("BTC", "") + "\n" + "\n" +

                    "24 Hour Change: " + Math.Round(twentyfourHourChange, 2) + "%" + "\n" +

                    "Last Price USDT: $" + Math.Round(lastPriceDollaredOut, 2) + "\n" +

                    "Price Versus BTC: " + (decimal)lastPrice + "\n"                   

                    
                };

                await ReplyAsync("", false, builder.Build());


            }

            catch (WebException e)
            {
                //output error to console
                Console.WriteLine(e);
                //outout message to user
                await ReplyAsync("Coin not found: " + _coin.Replace("BTC","") + " :thumbsdown:");
                
            }

            typing.Dispose();
        }
      
    }
}
