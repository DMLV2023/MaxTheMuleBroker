namespace MaxTheMuleBroker_ejemplo.Models
{
	//public class OrdersHistory
	public class Actions
	{
		public int TxNumber{get; set; }
        public DateTime OrderDate { get; set; }
        public string Action { get; set; }

		public string Status { get; set; }

		public string Symbol { get; set; }
		
		public int Quantity { get; set; }

		public double Price { get; set; }

		public decimal? TotalPrice { get; set; }
	}
}
