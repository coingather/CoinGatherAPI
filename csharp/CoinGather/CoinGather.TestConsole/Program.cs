using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinGather.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // get these from https://www.coingather.com/account/edit
            var publicKey = "Your PUBLIC API key";
            var privateKey = "Your PRIVATE API key";


            var client = new CoinGather.Client.APIClient(publicKey, privateKey);
            
            // show LIMX market
            TestMarketOrders(client);

            // create very small BUY and SELL orders for LIMX
            // this will actually create orders, use with caution!
            TestCreateOrder(client);
            TestMarketOrders(client);

            Console.WriteLine("Done");
            Console.Read();
        }

        private static void TestCreateOrder(Client.APIClient client)
        {
            var marketid = 55;

            var buyresponse = client.createorder(marketid, "BUY", price: .00000001m, quantity: 1m);
            if (buyresponse.Property("success").ToObject<int>() == 1)
            {
                var buyorderid = buyresponse
                    .Property("return").Value.ToObject<JObject>()
                    .Property("orderid").Value.ToObject<int>();
                Console.WriteLine();
                Console.WriteLine(String.Format("BUY Order ID: {0}", buyorderid));
            }
            else
            {
                Console.WriteLine("BUY CreateOrder Error: {0}", buyresponse.Property("error").ToObject<string>());
            }


            var sellresponse = client.createorder(marketid, "SELL", price: 1m, quantity: .1m);
            if (sellresponse.Property("success").ToObject<int>() == 1)
            {
                var sellorderid = sellresponse
                    .Property("return").Value.ToObject<JObject>()
                    .Property("orderid").Value.ToObject<int>();
                Console.WriteLine();
                Console.WriteLine(String.Format("SELL Order ID: {0}", sellorderid));
            }
            else
            {
                Console.WriteLine("SELL CreateOrder Error: {0}", sellresponse.Property("error").ToObject<string>());
            }

            
        }

        private static void TestMarketOrders(Client.APIClient client)
        {
            var marketid = 55;

            var response = client.marketorders(marketid);

            if (response.Property("success").ToObject<int>() == 1)
            {

                var buyorders = response.Property("return").Value.ToObject<JObject>().Property("buyorders").Value.ToObject<JArray>();
                var sellorders = response.Property("return").Value.ToObject<JObject>().Property("sellorders").Value.ToObject<JArray>();

                Console.WriteLine();
                Console.WriteLine("Buys");
                Console.WriteLine("price\t\tquantity\ttotal");
                foreach (JObject o in buyorders)
                {
                    Console.WriteLine(String.Format("{0}\t{1}\t{2}",
                        o.Property("buyprice").Value.ToString(),
                        o.Property("quantity").Value.ToString(),
                        o.Property("total").Value.ToString()
                    ));
                }

                Console.WriteLine();
                Console.WriteLine("Sells");
                Console.WriteLine("price\t\tquantity\ttotal");
                foreach (JObject o in sellorders)
                {
                    Console.WriteLine(String.Format("{0}\t{1}\t{2}",
                        o.Property("sellprice").Value.ToString(),
                        o.Property("quantity").Value.ToString(),
                        o.Property("total").Value.ToString()
                    ));
                }
            }
            else
            {
                Console.WriteLine("MarketOrders Error: {0}", response.Property("error").ToObject<string>());
            }
        }
    }
}
