using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Binance_Bot.Services
{

    public class BinancePriceCalls
    {


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

        public decimal closeOfLastHour(string _coin)
        {
            string _inputedCoin = _coin;
            var hourChangeOfInputedSymbol = $"https://www.binance.com/api/v1/klines?symbol={_coin}&interval=1h";

            var client = new WebClient(); //open web client
            string jsonRawHourChange = client.DownloadString(hourChangeOfInputedSymbol);//Hour change data
            dynamic currentHourCloseObj = JsonConvert.DeserializeObject(jsonRawHourChange);

            client.Dispose();

            var currentHourData = currentHourCloseObj[498];
            decimal currentHourClose = currentHourData[4];

            return currentHourClose;

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
