using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Binance_Bot.Services
{
    public class CoinMarketCapService
    {
        public string returnCoinFullName(string _symbol)
        {
            var cmcApiWebAddress = $"https://api.coinmarketcap.com/v1/ticker/?limit=0";

            var client = new WebClient();
            string cmcRawInput = client.DownloadString(cmcApiWebAddress);

            dynamic cmcObject = JsonConvert.DeserializeObject(cmcRawInput);
            client.Dispose();
            int length = cmcObject.Count; //since JArray doesn't have a Length function

            string fullNameOfSmybol = "";

            for (int i = 0; i < length; i++)
            {
                if(cmcObject[i]["symbol"] == _symbol)
                {
                    fullNameOfSmybol = cmcObject[i]["name"];
                    break;
                }               
                
            }            

            return fullNameOfSmybol;
        }
        
    }
}
