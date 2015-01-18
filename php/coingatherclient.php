<?

$publicKey = "Your Public Key";
$privateKey = "Your Private Key";

$client = new APIClient($publicKey, $privateKey);

// show LIMX market
TestMarketOrders($client);

// create very small BUY and SELL orders for LIMX
// this will actually create orders, use with caution!
//TestCreateOrder($client);
//TestMarketOrders($client);

function TestCreateOrder($client) {
	$marketid = 55;
	
	
	$buyresponse = $client->createorder($marketid, "BUY", "0.00000001", "1.00000000");
	if ($buyresponse["success"] == 1) {
		echo "\r\n";
		echo "BUY Order ID: ".$buyresponse["return"]["orderid"]."\r\n";
	} else {
		echo "BUY CreateOrder Error: ".$buyresponse["error"]."\r\n";
	}
	
	$sellresponse = $client->createorder($marketid, "SELL", 1, .1);
	if ($sellresponse["success"] == 1) {
		echo "\r\n";
		echo "SELl Order ID: ".$sellresponse["return"]["orderid"]."\r\n";
	} else {
		echo "SELL CreateOrder Error: ".$sellresponse["error"]."\r\n";
	}
}


function TestMarketOrders($client) {
	$marketid = 55;
	$response = $client->marketorders($marketid);

	if ($response["success"] == 1) {
		$buyorders = $response["return"]["buyorders"];
		$sellorders = $response["return"]["sellorders"];
		
		echo "\r\n";
		echo "Buys\r\n";
		echo "price\t\tquantity\ttotal\r\n";
		foreach($buyorders as $o) {
			echo $o["buyprice"]."\t".$o["quantity"]."\t".$o["total"]."\r\n";
		}

		echo "\r\n";
		echo "Sells\r\n";
		echo "price\t\tquantity\ttotal\r\n";
		foreach($sellorders as $o) {
			echo $o["sellprice"]."\t".$o["quantity"]."\t".$o["total"]."\r\n";
		}		
		
		
	} else {
		echo $response["error"]."\r\n";
	}
}



class APIClient {

	protected $m_publicKey;
	protected $m_privateKey;
	protected $m_apiurl = "https://www.coingather.com/api/v1/";

	/* Constructor */
	
    public function __construct( $publicKey, $privateKey ) {
		$this->m_publicKey = $publicKey;
		$this->m_privateKey = $privateKey;
    }
	
	/* Public Methods */
	
	public function allmyorders() {
		$method = "allmyorders";
		$data = "nonce=".$this->getNonce();
		return $this->makeRequest($method, $data);
	}
	
	public function allmytrades() {
		$method = "allmytrades";
		$data = "nonce=".$this->getNonce();
		return $this->makeRequest($method, $data);
	}

	public function calculatefees($ordertype, $quantity, $price, $marketid) {
		$method = "calculatefees";
		$data = "nonce=".$this->getNonce()."&ordertype=".$ordertype."&quantity=".$quantity."&price=".$price."&marketid=".$marketid;
		return $this->makeRequest($method, $data);
	}
	
	public function cancelallorders() {
		$method = "cancelallorders";
		$data = "nonce=".$this->getNonce();
		return $this->makeRequest($method, $data);
	}	
	
	public function cancelmarketorders($marketid) {
		$method = "cancelmarketorders";
		$data = "nonce=".$this->getNonce()."&marketid=".$marketid;
		return $this->makeRequest($method, $data);
	}
	
	public function cancelorder($orderid) {
		$method = "cancelorder";
		$data = "nonce=".$this->getNonce()."&orderid=".$orderid;
		return $this->makeRequest($method, $data);
	}
	
	public function createorder($marketid, $ordertype, $price, $quantity) {
		$method = "createorder";
		$data = "nonce=".$this->getNonce()."&marketid=".$marketid."&ordertype=".$ordertype."&price=".$price."&quantity=".$quantity;
		return $this->makeRequest($method, $data);
	}
	
	public function generatenewaddress($currencycode) {
		$method = "generatenewaddress";
		$data = "nonce=".$this->getNonce()."&currencycode=".$currencycode;
		return $this->makeRequest($method, $data);
	}
	
	public function getcoindata() {
		$method = "getcoindata";
		$data = "nonce=".$this->getNonce();
		return $this->makeRequest($method, $data);
	}	

	public function getinfo() {
		$method = "getinfo";
		$data = "nonce=".$this->getNonce();
		return $this->makeRequest($method, $data);
	}	
	
	public function getmydepositaddresses() {
		$method = "getmydepositaddresses";
		$data = "nonce=".$this->getNonce();
		return $this->makeRequest($method, $data);
	}	
	
	public function getorderstatus($orderid) {
		$method = "getorderstatus";
		$data = "nonce=".$this->getNonce()."&orderid=".$orderid;
		return $this->makeRequest($method, $data);
	}
	
	public function marketorders($marketid) {
		$method = "marketorders";
		$data = "nonce=".$this->getNonce()."&marketid=".$marketid;
		return $this->makeRequest($method, $data);
	}
	
	public function markettrades($marketid) {
		$method = "markettrades";
		$data = "nonce=".$this->getNonce()."&marketid=".$marketid;
		return $this->makeRequest($method, $data);
	}
	
	public function myorders($marketid) {
		$method = "myorders";
		$data = "nonce=".$this->getNonce()."&marketid=".$marketid;
		return $this->makeRequest($method, $data);
	}
	
	public function mytrades($marketid) {
		$method = "mytrades";
		$data = "nonce=".$this->getNonce()."&marketid=".$marketid;
		return $this->makeRequest($method, $data);
	}
	
	public function mytransactions() {
		$method = "mytransactions";
		$data = "nonce=".$this->getNonce();
		return $this->makeRequest($method, $data);
	}	
	
	public function mytransfers() {
		$method = "mytransfers";
		$data = "nonce=".$this->getNonce();
		return $this->makeRequest($method, $data);
	}	
	
	
	/* Private Methods */
	
	
	
	private function getNonce() {
        $mt = explode(' ', microtime());
        return (1000 * $mt[1]) + (int)($mt[0] * 1000);
	}
	
	private function signData($data) {
		return hash_hmac('sha512', $data, $this->m_privateKey);
	}
	
	private function makeRequest($method, $data) {
        $sign = $this->signData($data);
		
        $headers = array(
			'Sign: '.$sign,
			'Key: '.$this->m_publicKey,
        );		
		
		static $ch = null;
        if (is_null($ch)) {
                $ch = curl_init();
                curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
                //curl_setopt($ch, CURLOPT_USERAGENT, 'Mozilla/4.0 (compatible; Cryptsy API PHP client; '.php_uname('s').'; PHP/'.phpversion().')');
        }
		curl_setopt($ch, CURLOPT_ENCODING ,"");
        curl_setopt($ch, CURLOPT_URL, $this->m_apiurl . $method);
        curl_setopt($ch, CURLOPT_POSTFIELDS, $data);
        curl_setopt($ch, CURLOPT_HTTPHEADER, $headers);
        curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, FALSE);

		$res = curl_exec($ch);
        if ($res === false) throw new Exception('Could not get reply: '.curl_error($ch));

		$__BOM = pack('CCC', 239, 187, 191);
		while(0 === strpos($res, $__BOM))
			$res = substr($res, 3);
		
		$dec = json_decode($res, true);
        if (!$dec) throw new Exception('Invalid data received, please make sure connection is working and requested API exists');
        return $dec;
	}
	
}
?>