using MongoDB.Bson;
using MongoDB.Driver;
using TodayWebApi.BLL.Dtos.Orders;
using TodayWebAPi.DAL.Data.Entities;
using TodayWebAPi.DAL.Repos.Basket;
using TodayWebAPi.DAL.Repos.Generic;
using TodayWebAPi.DAL.Repos.UnitOfWork;

namespace TodayWebApi.BLL.Managers.Orders
{
    public class OrderManager : IOrderManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketRepo _basketRepo;

        public OrderManager(IUnitOfWork unitOfWork , IBasketRepo basketRepo)
        {
            _unitOfWork = unitOfWork;
            _basketRepo = basketRepo;       
        }

        private async Task<OrderToReturnDto> MapToDto(Order order)
        {
            if (order == null)
                return null;

            var deliveryMethod = await _unitOfWork.Repo<DeliveryMethod>()
                                    .GetByIdAsync(order.DeliveryMethodId);

            return new OrderToReturnDto
            {
                Id = order.Id,
                BuyerEmail = order.BuyerEmail,
                OrderDate = order.OrderDate,
                DeliveryMethodId = order.DeliveryMethodId.ToString(),
                ShippingPrice = deliveryMethod.Price,
                OrderItems = order.OrderItems.Select(i => new OrderItemDto
                {
                    ProductId = i.PrdouctId,
                    Price = i.Price,
                    Quantity = i.Quantity
                }).ToList(),
                SubTotal = order.SubTotal,
                Total = order.SubTotal + deliveryMethod.Price,
                Status = order.Status,
                PaymentIntentId = order.PaymentIntentId,
                PaymentId = order.PaymentId
            };
        }

        public async Task<OrderToReturnDto> CreateOrderAsync(string buyerEmail, string deliveryMethodId, string basketId)
        {
            var basket = await _basketRepo.GetBasketAsync(basketId);
            if (basket == null || !basket.Items.Any())
            {
                return null;
            }

            var items = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var productItem = await _unitOfWork.Repo<Product>().GetByIdAsync(item.ProductId);
                if (productItem == null)
                    continue;

                var orderItem = new OrderItem(productItem.Id, productItem.Price, item.Quantity);
                items.Add(orderItem);
            }

            var deliveryMethod = await _unitOfWork.Repo<DeliveryMethod>().GetByIdAsync(deliveryMethodId);
            if (deliveryMethod == null)
            {
                return null;
            }

            var subTotal = items.Sum(item => item.Price * item.Quantity);
            var total = subTotal + deliveryMethod.Price;

            var payment = new Payment
            {
                StripePaymentIntentId = basket.PaymentIntentId,
                Amount = total,
                Currency = "usd",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repo<Payment>().AddAsync(payment);

            var order = new Order(items, buyerEmail, deliveryMethod.Id, subTotal)
            {
                PaymentIntentId = basket.PaymentIntentId,
                Total = total,
                PaymentId = payment.Id
            };

            await _unitOfWork.Repo<Order>().AddAsync(order);

            await _basketRepo.DeleteBasketAsync(basketId);

            return await MapToDto(order);
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            return await _unitOfWork.Repo<DeliveryMethod>().GetAllAsync();
        }

        public async Task<OrderToReturnDto> GetOrderByIdAsync(string id, string buyerEmail)
        {
            var order = await _unitOfWork.Repo<Order>().FindOneAsync(o => o.Id == id && o.BuyerEmail == buyerEmail);
            return await MapToDto(order);
        }

        public async Task<OrderToReturnDto> GetOrderByIdForAdminAsync(string id)
        {
            var order = await _unitOfWork.Repo<Order>().GetByIdAsync(id);
            return await MapToDto(order);
        }

        public async Task<IReadOnlyList<OrderToReturnDto>> GetOrdersForUserAsync(string buyerEmail)
        {
            var orders = await _unitOfWork.Repo<Order>().FindAllAsync(o => o.BuyerEmail == buyerEmail);
            var dtoTasks = orders.Select(order => MapToDto(order));
            var dtoList = await Task.WhenAll(dtoTasks);
            return dtoList.ToList();
        }

        public async Task<bool> UpdateOrderStatusAsync(string orderId, string newStatus)
        {
            var orderRepo = (GenericRepo<Order>)_unitOfWork.Repo<Order>();

            var order = await orderRepo.GetByIdAsync(orderId);
            if (order == null)
                return false;

            var collection = orderRepo.GetCollection();

            var filter = Builders<Order>.Filter.Eq("_id", ObjectId.Parse(orderId));
            var update = Builders<Order>.Update.Set(s => s.Status, newStatus);

            var result = await collection.UpdateOneAsync(filter, update);

            return true;
        }
    }
}