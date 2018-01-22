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

namespace Binance_Bot.Modules
{

    /// <summary>
    /// General api endpoint address for API calls
    /// //https://api.binance.com
    ///  /api/v1/ticker/allPrices
    ///  
    ///  interval URL
    ///  https://www.binance.com/api/v1/klines?symbol=BTCUSDT&interval=1m 
    /// TODO: 
    ///         Add in price check versus ETH
    ///         Check for more errors that are caused within BTC calls --solved 1
    ///         Figure out a way to pull back last hour price change -- related to klines I believe --DONE
    ///         Change price output to include numbers that end in zero. EX. $1.90, right now it shows $1.9 -- DONE?
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
            
            Boolean wasBitcoinEntered = false; //error checking to make tell if bitcoin was entered. Default to false.

            try
            {
                _coin = _coin + "BTC"; //default input parameter to be paired with BTC                
               
                if (_coin == "BTCBTC")
                {   //error checking for BTC pair - Default to BTCUSDT
                    _coin = "BTCUSDT";
                    wasBitcoinEntered = true;
                }

                var apiStartingAddress = $"https://www.binance.com/api/v1/ticker/24hr?symbol={_coin}";
                var bitcoinDollarValue = $"https://www.binance.com/api/v1/ticker/24hr?symbol=BTCUSDT";                

                var client = new WebClient(); //open web client

                string jsonRawSymbolInput = client.DownloadString(apiStartingAddress);      //raw json data depending on input
                string BTCUSDT = client.DownloadString(bitcoinDollarValue);                 //BTCUSDT raw data
                

                dynamic binanceObj = JObject.Parse(jsonRawSymbolInput);
                dynamic bitcoinObj = JObject.Parse(BTCUSDT);
               
                client.Dispose();

                decimal USDTprice = bitcoinObj["lastPrice"];
                           

                //declare the variables that need to be output
                string symbol = binanceObj["symbol"];
                decimal lastPrice = binanceObj["lastPrice"];
                decimal twentyfourHourChange = binanceObj["priceChangePercent"];              
               
                decimal lastPriceDollaredOut;
                if (wasBitcoinEntered == false)
                {
                    lastPriceDollaredOut = (USDTprice * lastPrice);

                }
                else
                {
                    lastPriceDollaredOut = (USDTprice);
                }

                if (symbol == "BTCUSDT")
                {
                    symbol = "BTC";
                    lastPrice = Math.Round(lastPrice, 2);
                }
                else
                {
                    symbol = symbol.Replace("BTC", "");
                }       
               

                var builder = new EmbedBuilder()
                {
                    Color = new Color(114, 137, 218),
                    Description =
                    "Symbol: " + symbol + "\n" + "\n" +
                    "1 Hour Change: " + Math.Round(percentChangePastHour(), 2) + '%' + "(as of " + currentTimestampDatetime() +")" + "\n" +
                    "24 Hour Change: " + Math.Round(twentyfourHourChange, 2) + "%" + "\n" +
                    "Last Price USDT: $" + Math.Round(lastPriceDollaredOut, 2) + "\n" +
                    "Price Versus BTC: " + lastPrice + "\n"           
                    
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

        public decimal percentChangePastHour()
        {
            var hourChangeOfInputedSymbol = $"https://www.binance.com/api/v1/klines?symbol=BTCUSDT&interval=1h";

            var client = new WebClient(); //open web client
            string jsonRawHourChange = client.DownloadString(hourChangeOfInputedSymbol);//Hour change data

            dynamic previousHourCloseObj = JsonConvert.DeserializeObject(jsonRawHourChange);
            dynamic currentHourCloseObj = JsonConvert.DeserializeObject(jsonRawHourChange);

            client.Dispose();
            //close of previous
            //close of current 
            //221.42
            var previousHourData = previousHourCloseObj[498];
            var currentHourData = currentHourCloseObj[499];

            decimal previousHourClose = previousHourData[4];
            decimal currentHourClose = currentHourData[4];


            decimal oneHourChange = (currentHourClose - previousHourClose) / Math.Abs(previousHourClose) * 100;

            return oneHourChange;

        }


        public DateTime previousTimestampDateTime()
        {
            var client = new WebClient(); //open web client

            var hourChangeOfInputedSymbol = $"https://www.binance.com/api/v1/klines?symbol=BTCUSDT&interval=1h";
            string jsonRawHourChange = client.DownloadString(hourChangeOfInputedSymbol);//Hour change data

            dynamic previousHourCloseObj = JsonConvert.DeserializeObject(jsonRawHourChange);
            
            var previousHour = previousHourCloseObj[498];
            
            client.Dispose();

            double timestampPreviousHour = previousHour[0];            

            System.DateTime dateTimePreviousHour = new System.DateTime(1970, 1, 1, 0, 0, 0);           
            dateTimePreviousHour = dateTimePreviousHour.AddMilliseconds(timestampPreviousHour).ToLocalTime(); //Create TimeStamps for  previous Hour

            return dateTimePreviousHour;

        }

        public DateTime currentTimestampDatetime()
        {
            var client = new WebClient(); //open web client

            var hourChangeOfInputedSymbol = $"https://www.binance.com/api/v1/klines?symbol=BTCUSDT&interval=1h";
            string jsonRawHourChange = client.DownloadString(hourChangeOfInputedSymbol);//Hour change data

            dynamic currentHourCloseObj = JsonConvert.DeserializeObject(jsonRawHourChange);

            var currentHour = currentHourCloseObj[499];

            client.Dispose();

            double timestampCurrentHour = currentHour[0];

            System.DateTime dateTimeCurrentHour = new System.DateTime(1970, 1, 1, 0, 0, 0);

            dateTimeCurrentHour = dateTimeCurrentHour.AddMilliseconds(timestampCurrentHour).ToLocalTime(); //Create TimeStamps for  Current Hour

            return dateTimeCurrentHour;

        }

    }
}
