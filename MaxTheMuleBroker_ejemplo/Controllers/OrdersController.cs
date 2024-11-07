using Microsoft.AspNetCore.Mvc;
using MaxTheMuleBroker_ejemplo.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System;

namespace MaxTheMuleBroker_ejemplo.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrdersController : ControllerBase
	{
		// Métodos GET
		[HttpGet("OrdersByYear")]
		public List<Actions> OrdersByYear(int year)
		{
			Logic l = new Logic();
			List<Actions> actions = l.getOrdersFromDataBaseByYear(year);

			foreach (var item in actions)
			{
				item.TotalPrice = (decimal)(item.Quantity * item.Price);
			}

			return actions;
		}

		[HttpGet("TxNumber_Order")]
		public List<Actions> OperationsByStatus2(int TX_NUMBER)
		{
			Logic l = new Logic();
			List<Actions> actions = l.getOrdersFromDataBase2(TX_NUMBER);

			foreach (var item in actions)
			{
				item.TotalPrice = (decimal)(item.Quantity * item.Price);
			}

			return actions;
		}

		[HttpGet("getOrdersByStatus")]
		public List<Actions> OperationsByStatus(string status)
		{
			Logic l = new Logic();
			List<Actions> actions = l.getOrdersFromDataBase(status);

			foreach (var item in actions)
			{
				item.TotalPrice = (decimal)(item.Quantity * item.Price);
			}

			return actions;
		}

		// Método POST para insertar en las tablas ORDERS y ORDERS_HISTORY en una sola transacción
		[HttpPost("CreateOrder")]
		public IActionResult CreateOrder(List<Orders> order)
		{
			Logic logic = new Logic();

			using (SqlConnection sqlConn = new SqlConnection(logic.connectionString))
			{
				sqlConn.Open();
				using (SqlTransaction transaction = sqlConn.BeginTransaction())
				{
					try
					{
						foreach (Orders o in order)
						{
							// Insertar en la tabla ORDERS
							string insertOrderQuery = "INSERT INTO ORDERS(ORDER_DATE, ACTION, STATUS, SYMBOL, QUANTITY, PRICE) " +
													   "VALUES (@OrderDate, @Action, @Status, @Symbol, @Quantity, @Price)";
							using (SqlCommand cmd = new SqlCommand(insertOrderQuery, sqlConn, transaction))
							{
								cmd.Parameters.AddWithValue("@OrderDate", o.OrderDate);
								cmd.Parameters.AddWithValue("@Action", o.Action);
								cmd.Parameters.AddWithValue("@Status", o.Status);
								cmd.Parameters.AddWithValue("@Symbol", o.Symbol);
								cmd.Parameters.AddWithValue("@Quantity", o.Quantity);
								cmd.Parameters.AddWithValue("@Price", o.Price);
								cmd.ExecuteNonQuery();
							}

							// Insertar en la tabla ORDERS_HISTORY
							string insertHistoryQuery = "INSERT INTO ORDERS_HISTORY (ORDER_DATE, ACTION, STATUS, SYMBOL, QUANTITY, PRICE) " +
														 "VALUES (@OrderDate, @Action, @Status, @Symbol, @Quantity, @Price)";
							using (SqlCommand historyCmd = new SqlCommand(insertHistoryQuery, sqlConn, transaction))
							{
								historyCmd.Parameters.AddWithValue("@OrderDate", o.OrderDate);
								historyCmd.Parameters.AddWithValue("@Action", o.Action);
								historyCmd.Parameters.AddWithValue("@Status", o.Status);
								historyCmd.Parameters.AddWithValue("@Symbol", o.Symbol);
								historyCmd.Parameters.AddWithValue("@Quantity", o.Quantity);
								historyCmd.Parameters.AddWithValue("@Price", o.Price);
								historyCmd.ExecuteNonQuery();
							}
						}

						transaction.Commit();
						return Ok("Orders inserted successfully");
					}
					catch (Exception ex)
					{
						transaction.Rollback();
						return BadRequest("Error inserting order: " + ex.Message);
					}
				}
			}
		}

		// Método PUT para actualizar el estado en la tabla ORDERS y registrar en ORDERS_HISTORY
		[HttpPut("updateOrdersByStatus")]
		public IActionResult UpdateOrdersByStatus([FromBody] List<ModificarOrder> updatedStatuses)
		{
			Logic logic = new Logic();
			using (SqlConnection connection = new SqlConnection(logic.connectionString))
			{
				connection.Open();
				using (SqlTransaction transaction = connection.BeginTransaction())
				{
					try
					{
						foreach (var order in updatedStatuses)
						{
							// Actualizar el estado en la tabla ORDERS
							string updateQuery = "UPDATE ORDERS SET STATUS = @Status WHERE TX_NUMBER = @TxNumber";
							using (SqlCommand command = new SqlCommand(updateQuery, connection, transaction))
							{
								command.Parameters.AddWithValue("@Status", order.Status);
								command.Parameters.AddWithValue("@TxNumber", order.TxNumber);
								command.ExecuteNonQuery();
							}

							// Insertar en la tabla ORDERS_HISTORY
							string insertHistoryQuery = "INSERT INTO ORDERS_HISTORY (ORDER_DATE, ACTION, STATUS, SYMBOL, QUANTITY, PRICE) " +
														 "SELECT ORDER_DATE, ACTION, @Status, SYMBOL, QUANTITY, PRICE FROM ORDERS WHERE TX_NUMBER = @TxNumber";
							using (SqlCommand historyCmd = new SqlCommand(insertHistoryQuery, connection, transaction))
							{
								historyCmd.Parameters.AddWithValue("@Status", order.Status);
								historyCmd.Parameters.AddWithValue("@TxNumber", order.TxNumber);
								historyCmd.ExecuteNonQuery();
							}
						}

						transaction.Commit();
						return Ok("Orders updated successfully.");
					}
					catch (Exception ex)
					{
						transaction.Rollback();
						return StatusCode(500, "Failed to update orders: " + ex.Message);
					}
				}
			}
		}
		// Método DELETE para eliminar órdenes y registrar en ORDERS_HISTORY con estado "Delete"
		[HttpDelete("DeleteOrders")]
		public IActionResult DeleteOrders([FromBody] List<int> orderIds)
		{
			Logic logic = new Logic();
			bool result = logic.DeleteOrdersWithHistory(orderIds);

			if (result)
			{
				return Ok("Orders deleted successfully and recorded in history with status 'Delete'.");
			}
			else
			{
				return StatusCode(500, "Failed to delete orders and record history.");
			}
		}
	}
}