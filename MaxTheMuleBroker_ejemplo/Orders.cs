namespace MaxTheMuleBroker_ejemplo
{
	public class Orders
	{
		public int TxNumber { get; set; } // TX_NUMBER- CLAVE PRIMARIA

		public DateTime OrderDate { get; set; } // ORDER DATE

		public string Action { get; set; } // Action (vachar(4))

		public string Status { get; set; } // STATUS(VARCHAR(10))

		public string Symbol { get; set; } // Symbol (vachar(5))

		public int Quantity { get; set; } // quantity

		public decimal Price { get; set; } //PRICE(asumido como decimal)

		//public decimal? TotalPrice { get; set; }
		//public object TX_NUMBER { get; internal set; }
	}
}
