using MaxTheMuleBroker_ejemplo;
using MaxTheMuleBroker_ejemplo.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

public class Logic
{
	public string connectionString { get; set; } = "Server=DESKTOP-54M18O1\\DIEGO;Database=Max_The_Mule_Broker;Integrated Security=True;Trusted_Connection=True;TrustServerCertificate=True";

	public Logic() { }

	// Método para obtener órdenes por estado
	public List<Actions> getOrdersFromDataBase(string Status)
	{
		List<Actions> orders = new List<Actions>();

		string query = "SELECT TX_NUMBER, ORDER_DATE, ACTION, STATUS, SYMBOL, QUANTITY, PRICE FROM ORDERS_HISTORY WHERE STATUS = @Status";
		using (SqlConnection sqlConn = new SqlConnection(connectionString))
		{
			sqlConn.Open();
			using (SqlCommand cmd = new SqlCommand(query, sqlConn))
			{
				cmd.Parameters.AddWithValue("@Status", Status);
				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						Actions action = new Actions
						{
							TxNumber = int.Parse(reader["TX_NUMBER"].ToString()),
							OrderDate = Convert.ToDateTime(reader["ORDER_DATE"]),
							Action = reader["ACTION"].ToString(),
							Status = reader["STATUS"].ToString(),
							Symbol = reader["SYMBOL"].ToString(),
							Quantity = int.Parse(reader["QUANTITY"].ToString()),
							Price = (double)decimal.Parse(reader["PRICE"].ToString())
						};
						action.TotalPrice = (decimal)action.Price * action.Quantity;
						orders.Add(action);
					}
				}
			}
		}

		return orders;
	}

	// Método para obtener órdenes por año
	public List<Actions> getOrdersFromDataBaseByYear(int year)
	{
		List<Actions> orders = new List<Actions>();

		string query = "SELECT TX_NUMBER, ORDER_DATE, ACTION, STATUS, SYMBOL, QUANTITY, PRICE FROM ORDERS_HISTORY WHERE YEAR(ORDER_DATE) = @Year";
		using (SqlConnection sqlConn = new SqlConnection(connectionString))
		{
			sqlConn.Open();
			using (SqlCommand cmd = new SqlCommand(query, sqlConn))
			{
				cmd.Parameters.AddWithValue("@Year", year);
				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						Actions action = new Actions
						{
							TxNumber = int.Parse(reader["TX_NUMBER"].ToString()),
							OrderDate = Convert.ToDateTime(reader["ORDER_DATE"]),
							Action = reader["ACTION"].ToString(),
							Status = reader["STATUS"].ToString(),
							Symbol = reader["SYMBOL"].ToString(),
							Quantity = int.Parse(reader["QUANTITY"].ToString()),
							Price = (double)decimal.Parse(reader["PRICE"].ToString())
						};
						action.TotalPrice = (decimal)action.Price * action.Quantity;
						orders.Add(action);
					}
				}
			}
		}

		return orders;
	}

	// Método para obtener órdenes por TX_NUMBER
	public List<Actions> getOrdersFromDataBase2(int TX_NUMBER)
	{
		List<Actions> orders = new List<Actions>();

		string query = "SELECT TX_NUMBER, ORDER_DATE, ACTION, STATUS, SYMBOL, QUANTITY, PRICE FROM ORDERS WHERE TX_NUMBER = @TxNumber";
		using (SqlConnection sqlConn = new SqlConnection(connectionString))
		{
			sqlConn.Open();
			using (SqlCommand cmd = new SqlCommand(query, sqlConn))
			{
				cmd.Parameters.AddWithValue("@TxNumber", TX_NUMBER);
				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						Actions action = new Actions
						{
							TxNumber = int.Parse(reader["TX_NUMBER"].ToString()),
							OrderDate = Convert.ToDateTime(reader["ORDER_DATE"]),
							Action = reader["ACTION"].ToString(),
							Status = reader["STATUS"].ToString(),
							Symbol = reader["SYMBOL"].ToString(),
							Quantity = int.Parse(reader["QUANTITY"].ToString()),
							Price = (double)decimal.Parse(reader["PRICE"].ToString())
						};
						action.TotalPrice = (decimal)action.Price * action.Quantity;
						orders.Add(action);
					}
				}
			}
		}

		return orders;
	}

	// Método para insertar en ORDERS_HISTORY
	public void InsertOrderHistory(Orders order)
	{
		using (SqlConnection sqlConn = new SqlConnection(connectionString))
		{
			sqlConn.Open();
			string query = "INSERT INTO ORDERS_HISTORY (ORDER_DATE, ACTION, STATUS, SYMBOL, QUANTITY, PRICE) VALUES (@OrderDate, @Action, @Status, @Symbol, @Quantity, @Price)";
			using (SqlCommand cmd = new SqlCommand(query, sqlConn))
			{
				cmd.Parameters.AddWithValue("@OrderDate", order.OrderDate);
				cmd.Parameters.AddWithValue("@Action", order.Action);
				cmd.Parameters.AddWithValue("@Status", order.Status);
				cmd.Parameters.AddWithValue("@Symbol", order.Symbol);
				cmd.Parameters.AddWithValue("@Quantity", order.Quantity);
				cmd.Parameters.AddWithValue("@Price", order.Price);
				cmd.ExecuteNonQuery();
			}
		}
	}

	// Método para actualizar órdenes y registrar en ORDERS_HISTORY
	public bool UpdateOrdersWithHistory(List<ModificarOrder> orders)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			connection.Open();
			using (SqlTransaction transaction = connection.BeginTransaction())
			{
				try
				{
					foreach (var order in orders)
					{
						// Actualizar en ORDERS
						string updateQuery = "UPDATE ORDERS SET STATUS = @Status WHERE TX_NUMBER = @TxNumber";
						using (SqlCommand command = new SqlCommand(updateQuery, connection, transaction))
						{
							command.Parameters.AddWithValue("@Status", order.Status);
							command.Parameters.AddWithValue("@TxNumber", order.TxNumber);
							command.ExecuteNonQuery();
						}

						// Insertar en ORDERS_HISTORY
						string insertHistoryQuery = "INSERT INTO ORDERS_HISTORY (ORDER_DATE, ACTION, STATUS, SYMBOL, QUANTITY, PRICE) SELECT ORDER_DATE, ACTION, @Status, SYMBOL, QUANTITY, PRICE FROM ORDERS WHERE TX_NUMBER = @TxNumber";
						using (SqlCommand historyCmd = new SqlCommand(insertHistoryQuery, connection, transaction))
						{
							historyCmd.Parameters.AddWithValue("@Status", order.Status);
							historyCmd.Parameters.AddWithValue("@TxNumber", order.TxNumber);
							historyCmd.ExecuteNonQuery();
						}
					}

					transaction.Commit();
					return true;
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					Console.WriteLine($"Error updating orders with history: {ex.Message}");
					return false;
				}
			}
		}
	}
	public bool updateOrdersInDataBase(List<ModificarOrder> orders)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			connection.Open();
			using (SqlTransaction transaction = connection.BeginTransaction())
			{
				try
				{
					foreach (var order in orders)
					{
						string query = "UPDATE ORDERS SET STATUS = @Status WHERE TX_NUMBER = @TxNumber";

						using (SqlCommand command = new SqlCommand(query, connection, transaction))
						{
							command.Parameters.AddWithValue("@Status", order.Status);
							command.Parameters.AddWithValue("@TxNumber", order.TxNumber);

							command.ExecuteNonQuery();
						}
					}

					transaction.Commit();
					return true;
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					Console.WriteLine($"Error updating database: {ex.Message}");
					return false;
				}
			}
		}
	}
	// Método para eliminar órdenes y registrar en ORDERS_HISTORY con estado "Delete"
	// Método para eliminar órdenes y registrar en ORDERS_HISTORY con estado "Delete"
	public bool DeleteOrdersWithHistory(List<int> orderIds)
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			connection.Open();
			using (SqlTransaction transaction = connection.BeginTransaction())
			{
				try
				{
					foreach (int txNumber in orderIds)
					{
						// Registrar en ORDERS_HISTORY con estado "Delete"
						string insertHistoryQuery = "INSERT INTO ORDERS_HISTORY (ORDER_DATE, ACTION, STATUS, SYMBOL, QUANTITY, PRICE) " +
													"SELECT ORDER_DATE, ACTION, 'Delete', SYMBOL, QUANTITY, PRICE FROM ORDERS WHERE TX_NUMBER = @TxNumber";
						using (SqlCommand historyCmd = new SqlCommand(insertHistoryQuery, connection, transaction))
						{
							historyCmd.Parameters.AddWithValue("@TxNumber", txNumber);
							historyCmd.ExecuteNonQuery();
						}

						// Eliminar de la tabla ORDERS
						string deleteQuery = "DELETE FROM ORDERS WHERE TX_NUMBER = @TxNumber";
						using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, connection, transaction))
						{
							deleteCmd.Parameters.AddWithValue("@TxNumber", txNumber);
							deleteCmd.ExecuteNonQuery();
						}
					}

					transaction.Commit();
					return true;
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					Console.WriteLine($"Error deleting orders with history: {ex.Message}");
					return false;
				}
			}
		}
	}
}