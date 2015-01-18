using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace CoinGather.Client
{
    public class APIClient
    {
        private const string API_V1_URL = "https://www.coingather.com/api/v1/";
	    private static DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
	
	    public string PublicKey { get; private set; }
	    public string PrivateKey { get; private set; }
	    public string APIUrl { get; private set; }

        public APIClient(string publicKey, string privateKey, string apiurl = API_V1_URL)
	    {
		    this.PublicKey = publicKey;
		    this.PrivateKey = privateKey;
		    this.APIUrl = apiurl;
	    }
	
	
	    public JObject allmyorders()
	    {
		    var method = "allmyorders";
            var data = String.Format("nonce={0}", getNonce());
		    var response = makeRequest(method, data);
		    return JObject.Parse(response);
	    }
	
	    public JObject allmytrades()
	    {
		    var method = "allmytrades";
            var data = String.Format("nonce={0}", getNonce());
		    var response = makeRequest(method, data);
		    return JObject.Parse(response);
	    }
	
	    public JObject calculatefees(string ordertype, decimal quantity, decimal price, int marketid)
	    {
		    var method = "calculatefees";
		    var data = String.Format("nonce={0}&ordertype={1}&quantity={2}&price={3}&marketid={4}", getNonce(),
			    ordertype,
			    formatDecimal(quantity),
			    formatDecimal(price),
			    marketid
		    );
		    var response = makeRequest(method, data);
		    return JObject.Parse(response);
	    }
	
	    public JObject cancelallorders()
	    {
		    var method = "cancelallorders";
            var data = String.Format("nonce={0}", getNonce());
		    var response = makeRequest(method, data);
		    return JObject.Parse(response);
	    }
	
	    public JObject cancelmarketorders(int marketid)
	    {
		    var method = "cancelmarketorders";
		    var data = String.Format("nonce={0}&marketid={1}", getNonce(), marketid);
		    var response = makeRequest(method, data);
		    return JObject.Parse(response);
	    }
	
	    public JObject cancelorder(int orderid)
	    {
		    var method = "cancelorder";
		    var data = String.Format("nonce={0}&orderid={1}", getNonce(), orderid);
		    var response = makeRequest(method, data);
		    return JObject.Parse(response);
	    }
	
	    public JObject createorder(int marketid, string ordertype, decimal price, decimal quantity)
	    {
		    var method = "createorder";
		    var data = String.Format("nonce={0}&marketid={1}&ordertype={2}&price={3}&quantity={4}", getNonce(),
			    marketid,
			    ordertype,
			    formatDecimal(price),
			    formatDecimal(quantity)
		    );
		    var response = makeRequest(method, data);
		    return JObject.Parse(response);
	    }
	
	    public JObject generatenewaddress(string currencycode)
	    {
		    var method = "generatenewaddress";
		    var data = String.Format("nonce={0}&currencycode={1}", getNonce(), currencycode);
		    var response = makeRequest(method, data);
		    return JObject.Parse(response);
	    }
	
	    public JObject getcoindata()
	    {
		    var method = "getcoindata";
            var data = String.Format("nonce={0}", getNonce());
		    var response = makeRequest(method, data);
		    return JObject.Parse(response);
	    }
	
	    public JObject getinfo()
	    {
		    var method = "getinfo";
            var data = String.Format("nonce={0}", getNonce());
		    var response = makeRequest(method, data);
		    return JObject.Parse(response);
	    }
	
	    public JObject getmydepositaddresses()
	    {
		    var method = "getmydepositaddresses";
            var data = String.Format("nonce={0}", getNonce());
		    var response = makeRequest(method, data);
		    return JObject.Parse(response);
	    }
	
	    public JObject getorderstatus(int orderid)
	    {
		    var method = "getorderstatus";
		    var data = String.Format("nonce={0}&orderid={1}", getNonce(), orderid);
		    var response = makeRequest(method, data);
		    return JObject.Parse(response);
	    }
	
	    public JObject marketorders(int marketid)
	    {
		    var method = "marketorders";
		    var data = String.Format("nonce={0}&marketid={1}", getNonce(), marketid);
		    var response = makeRequest(method, data);
		    return JObject.Parse(response);
	    }
	
	    public JObject markettrades(int marketid)
	    {
		    var method = "markettrades";
		    var data = String.Format("nonce={0}&marketid={1}", getNonce(), marketid);
		    var response = makeRequest(method, data);
		    return JObject.Parse(response);
	    }
	
	    public JObject myorders(int marketid)
	    {
		    var method = "myorders";
		    var data = String.Format("nonce={0}&marketid={1}", getNonce(), marketid);
		    var response = makeRequest(method, data);
		    return JObject.Parse(response);
	    }
	
	    public JObject mytrades(int marketid)
	    {
		    var method = "mytrades";
		    var data = String.Format("nonce={0}&marketid={1}", getNonce(), marketid);
		    var response = makeRequest(method, data);
		    return JObject.Parse(response);
	    }
	
	    public JObject mytransactions()
	    {
		    var method = "mytransactions";
            var data = String.Format("nonce={0}", getNonce());
		    var response = makeRequest(method, data);
		    return JObject.Parse(response);
	    }
	
	    public JObject mytransfers()
	    {
		    var method = "mytransfers";
            var data = String.Format("nonce={0}", getNonce());
		    var response = makeRequest(method, data);
		    return JObject.Parse(response);
	    }
	
	
	
	
	    private static string formatDecimal(decimal d)
	    {
           var s = d.ToString("0.0000000000000000000000000000000000000000000000000000000000000000");
           var r = new Regex("^[0-9]{0,21}[.][0-9]{1,8}");
           var m = r.Match(s);

           if (!m.Success)
               throw new Exception("Error converting to string");
           return m.Groups[0].Value;
	    }
	
	    private string getNonce()
	    {
		    return ((long)(DateTime.UtcNow - UNIX_EPOCH).TotalMilliseconds).ToString();
	    }
	
	    private string makeRequest(string method, string data)
	    {
		    var sign = signData(data);
		    var bytes = System.Text.Encoding.UTF8.GetBytes(data);
		
		    var req = WebRequest.Create(this.APIUrl + method);
		    req.Method = "POST";
		    req.ContentType = "application/x-www-form-urlencoded";
		    req.Headers.Add("Key", this.PublicKey);
		    req.Headers.Add("Sign", sign);
		    req.ContentLength = bytes.Length;
		    req.GetRequestStream().Write(bytes, 0, bytes.Length);
		
		    var reader = new StreamReader(req.GetResponse().GetResponseStream());
		    var response = reader.ReadToEnd();
		
		    //response.Dump();
		    return response;
	    }
	
	

	    private string signData(string data)
        {
		    using (HMACSHA512 hmac = new HMACSHA512(System.Text.Encoding.ASCII.GetBytes(this.PrivateKey)))
		    {
			    byte[] hashValue = hmac.ComputeHash(System.Text.Encoding.ASCII.GetBytes(data));
			    return toHexLower(hashValue);
		    }
	    }
	

	    private static string toHexLower(byte[] ba)
	    {
		    StringBuilder hex = new StringBuilder(ba.Length * 2);
		    foreach (byte b in ba)
			    hex.AppendFormat("{0:x2}", b);
		    return hex.ToString();
	    }	


    }
}
