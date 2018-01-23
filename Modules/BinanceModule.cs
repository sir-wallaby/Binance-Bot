using Discord.Commands;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Binance_Bot.Services;

namespace Binance_Bot.Modules
{   /// <summary>
    /// General api endpoint address for API calls
    /// //https://api.binance.com
    ///  /api/v1/ticker/allPrices
    ///  
    ///  interval URL
    ///  https://www.binance.com/api/v1/klines?symbol=BTCUSDT&interval=1m 
    /// TODO:             
    ///         Top 5 by volume
    /// </summary>
    public class BinanceModule : ModuleBase
    {
        [Name("Coin Search")]
        [Command("b")]
        [Summary("Returns the ticker for the specified coin/token from Binance" + "\n"+
                "Usage: .b eth ")]       
        public async Task BinanceTicker([Remainder] string coin)
        {
            var typing = Context.Message.Channel.EnterTypingState();
            string _coin = coin.ToUpper();
            
            BinancePriceCalls price = new BinancePriceCalls();
            
            try
            {
                _coin = _coin + "BTC"; //default input parameter to be paired with BTC
                
                if (_coin == "BTCBTC")
                {   //error checking for BTC pair - Default to BTCUSDT
                    _coin = "BTCUSDT";
                }                
              
                var apiStartingAddressBTC = $"https://www.binance.com/api/v1/ticker/24hr?symbol={_coin}";
               
                var client = new WebClient(); //open web client
                string jsonRawSymbolInput = client.DownloadString(apiStartingAddressBTC);      //raw json data depending on input
                      //raw json data depending on input  
                dynamic binanceObj = JObject.Parse(jsonRawSymbolInput);                     //create JObject containing the data of input compared with BTC          

                client.Dispose(); //close the client connection

                string symbol = binanceObj["symbol"]; //holds the symbol value
                decimal twentyfourHourChange = binanceObj["priceChangePercent"]; //holds 24 hour change value                
                decimal lastPriceOutput;
                decimal lastPrice = binanceObj["lastPrice"];

                if (price.bitcoinEntered(_coin) == false)
                {
                    symbol = symbol.Replace("BTC", "");
                    lastPriceOutput = price.lastPriceUSDT(jsonRawSymbolInput);
                }
                else
                {
                    symbol = "BTC";
                    lastPriceOutput = price.bitcoinDollarValue();
                   lastPrice = Math.Round(lastPrice, 2);
                }

                var builder = new EmbedBuilder()
                {
                    Color = new Color(114, 137, 218),
                    Description =
                    "Symbol: " + symbol + "\n" + "\n" +
                    "Last Price USDT: $" + Math.Round(lastPriceOutput, 2) + "\n" +
                    "1 Hour Change: " + Math.Round(price.percentChangePastHour(_coin), 2) + "% " + "(as of " + price.currentTimestampDatetime().ToShortTimeString() + " - " +_coin + ": "  + price.closeOfLastHour(_coin) + ")" + "\n" +
                    "24 Hour Change: " + Math.Round(twentyfourHourChange, 2) + "%" + "\n" +
                    "Price Versus BTC: " + lastPrice + "\n" +
                   
                    "Price Versus ETH: " + price.lastPriceETH(_coin)                    
                };

                await ReplyAsync("", false, builder.Build());
            }

            catch (WebException e)
            {                
                Console.WriteLine(e);
                //outout message to user
                await ReplyAsync("Coin not found: " + _coin.Replace("BTC","") + " :thumbsdown:");                
            }

            typing.Dispose();
        }
    }
}
