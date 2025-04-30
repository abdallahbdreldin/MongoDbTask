using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodayWebApi.BLL.Dtos;
using TodayWebApi.BLL.Managers;
using TodayWebAPi.DAL.Data.Models;

namespace TodayWebAPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : BaseController
    {
        private readonly IOrderManager _orderManager;
        private readonly IEmailManager _emailManager;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderManager orderManager, IEmailManager emailManager, ILogger<OrderController> logger)
        {
            _orderManager = orderManager;
            _emailManager = emailManager;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
        {
            var email = GetUserEmail();
            try
            {
                //var email = User?.Identity?.Name;

                _logger.LogInformation("Creating order for user: {Email}, BasketId: {BasketId}", email, orderDto.BasketId);
                var order = await _orderManager.CreateOrderAsync(email, orderDto.DeliveryMethodId, orderDto.BasketId);

                if (order == null)
                {
                    _logger.LogWarning("Order creation failed for user: {Email}", email);
                    return BadRequest("Order creation failed.");
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order for user: {Email}", email);
                return StatusCode(500, "An error occurred while creating the order."+ex.Message);
            }
        }

        [HttpGet("orders")]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUsers()
        {
            var email = GetUserEmail();
            try
            {
                //var email = User?.Identity?.Name;

                _logger.LogInformation("Fetching orders for user: {Email}", email);
                var ordersToReturn = await _orderManager.GetOrdersForUserAsync(email);

                return Ok(ordersToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching orders for user: {Email}", email);
                return StatusCode(500, "An error occurred while fetching orders.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdForUser(string id)
        {
            var email = GetUserEmail();
            try
            {
                //var email = User?.Identity?.Name;

                _logger.LogInformation("Fetching order {Id} for user: {Email}", id, email);
                var orderDto = await _orderManager.GetOrderByIdAsync(id, email);

                if (orderDto == null)
                {
                    _logger.LogWarning("Order {Id} not found for user: {Email}", id, email);
                    return NotFound("Order not found.");
                }

                return Ok(orderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching order {Id} for user: {Email}", id, email);
                return StatusCode(500, "An error occurred while fetching the order.");
            }
        }

        [AllowAnonymous]
        [HttpGet("delivery-methods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            try
            {
                _logger.LogInformation("Fetching delivery methods");
                var methods = await _orderManager.GetDeliveryMethodsAsync();
                return Ok(methods);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching delivery methods");
                return StatusCode(500, "An error occurred while fetching delivery methods.");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(string orderId, [FromBody] string newStatus)
        {
            var email = GetUserEmail();
            try
            {
                if (!OrderStatus.IsValid(newStatus)) 
                {
                    return BadRequest($"Invalid order status");
                }

                _logger.LogInformation("Admin {Email} updating order {OrderId} to status: {NewStatus}",
                    email, orderId, newStatus);

                var order = await _orderManager.GetOrderByIdForAdminAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning("Order {OrderId} not found", orderId);
                    return NotFound("Order not found.");
                }

                var success = await _orderManager.UpdateOrderStatusAsync(orderId, newStatus);
                if (!success)
                {
                    _logger.LogWarning("Order status update failed for order {OrderId}", orderId);
                    return BadRequest("Order status update failed.");
                }

                try
                {
                    await _emailManager.SendEmailAsync(
                        order.BuyerEmail,
                        $"Order #{orderId} Status Update",
                        $"Your order status has been updated to: {newStatus}");
                    _logger.LogInformation("Email sent to {Email} for order {OrderId} status update", order.BuyerEmail, orderId);
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "Failed to send email to {Email} for order {OrderId}", order.BuyerEmail, orderId);
                }

                return Ok(new { Message = "Order status updated successfully.", OrderId = orderId, NewStatus = newStatus.ToString() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order {OrderId} status by admin: {Email}", orderId, email);
                return StatusCode(500, "An error occurred while updating the order status.");
            }
        }
    }
}