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
            
            try
            {
                _coin = _coin + "BTC"; //default input parameter to be paired with BTC

                if (_coin == "BTCBTC")
                {   //error checking for BTC pair - Default to BTCUSDT
                    _coin = "BTCUSDT";                    
                }

                var apiStartingAddress = $"https://www.binance.com/api/v1/ticker/24hr?symbol={_coin}";
                var client = new WebClient(); //open web client
                string jsonRawSymbolInput = client.DownloadString(apiStartingAddress);      //raw json data depending on input               
                dynamic binanceObj = JObject.Parse(jsonRawSymbolInput);                     //create JObject containing the data of input compared with BTC          

                client.Dispose(); //close the client connection

                string symbol = binanceObj["symbol"]; //holds the symbol value
                decimal twentyfourHourChange = binanceObj["priceChangePercent"]; //holds 24 hour change value                
                decimal lastPriceOutput;
                decimal lastPrice = binanceObj["lastPrice"];

                if (bitcoinEntered(_coin) == false)
                {
                    symbol = symbol.Replace("BTC", "");
                    lastPriceOutput = lastPriceUSDT(_coin, jsonRawSymbolInput);
                }
                else
                {
                    symbol = "BTC";
                    lastPriceOutput = bitcoinDollarValue();
                   lastPrice = Math.Round(lastPrice, 2);
                }


                var builder = new EmbedBuilder()
                {
                    Color = new Color(114, 137, 218),
                    Description =
                    "Symbol: " + symbol + "\n" + "\n" +
                    "Last Price USDT: $" + Math.Round(lastPriceOutput, 2) + "\n" +
                    "1 Hour Change: " + Math.Round(percentChangePastHour(_coin), 2) + '%' + " (as of " + currentTimestampDatetime() + ")" + "\n" +
                    "24 Hour Change: " + Math.Round(twentyfourHourChange, 2) + "%" + "\n" +
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
       
        public Boolean bitcoinEntered(string _coin)
        {
            Boolean wasBitcoinEntered; //error checking to make tell if bitcoin was entered

            if (_coin == "BTCUSDT")
                wasBitcoinEntered = true;
            else
                wasBitcoinEntered = false;

            return wasBitcoinEntered;
        }

        public decimal bitcoinDollarValue()
        {                        
            var bitcoinDollarValue = $"https://www.binance.com/api/v1/ticker/24hr?symbol=BTCUSDT";
            var client = new WebClient(); //open web client
            
            string BTCUSDT = client.DownloadString(bitcoinDollarValue);                 //BTCUSDT raw data  
            dynamic bitcoinObj = JObject.Parse(BTCUSDT);

            client.Dispose(); //close the client connection
            decimal USDTprice = bitcoinObj["lastPrice"];        

            return USDTprice;
        }

        public decimal lastPriceUSDT(string _coin, string _jsonRawSymbolInput)
        {
            _coin = _coin + "BTC"; //default input parameter to be paired with BTC            

            var client = new WebClient(); //open web client
            dynamic binanceObj = JObject.Parse(_jsonRawSymbolInput);                     //create JObject containing the data of input compared with BTC          

            client.Dispose(); //close the client connection
            decimal lastPrice = binanceObj["lastPrice"]; //holds the last sold price          
            decimal lastPriceDollaredOut = Math.Round((lastPrice * bitcoinDollarValue()), 2);
            
            return lastPriceDollaredOut;
        }

        public decimal lastPriceOfInputCoin(string _coin)
        {
            //_coin = _coin + "BTC"; //default input parameter to be paired with BTC          
            var client = new WebClient(); //open web client

            var apiStartingAddress = $"https://www.binance.com/api/v1/ticker/24hr?symbol={_coin}";
            string jsonRawSymbolInput = client.DownloadString(apiStartingAddress);
            
            dynamic binanceObj = JObject.Parse(jsonRawSymbolInput);                     //create JObject containing the data of input compared with BTC          

            client.Dispose(); //close the client connection

            decimal lastPrice = binanceObj["lastPrice"]; //holds the last sold price

            return lastPrice;
        }

        public decimal percentChangePastHour(string _coin)
        {
            string _inputedCoin = _coin;
            var hourChangeOfInputedSymbol = $"https://www.binance.com/api/v1/klines?symbol={_coin}&interval=1h";

            var client = new WebClient(); //open web client
            string jsonRawHourChange = client.DownloadString(hourChangeOfInputedSymbol);//Hour change data
            dynamic currentHourCloseObj = JsonConvert.DeserializeObject(jsonRawHourChange);

            client.Dispose();
            
            var currentHourData = currentHourCloseObj[498];
            decimal currentHourClose = currentHourData[4];
                       
            decimal oneHourChange = (lastPriceOfInputCoin(_inputedCoin) - currentHourClose) / Math.Abs(currentHourClose) * 100;

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
