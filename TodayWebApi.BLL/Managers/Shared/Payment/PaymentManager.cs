﻿using Microsoft.Extensions.Configuration;
using Stripe;
using TodayWebAPi.DAL.Data.Models;
using TodayWebAPi.DAL.Repos.Basket;
using TodayWebAPi.DAL.Repos.UnitOfWork;

namespace TodayWebApi.BLL.Managers.Shared.Payment
{
    public class PaymentManager : IPaymentManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketRepo _basketRepo;
        private readonly IConfiguration _config;

        public PaymentManager(IUnitOfWork unitOfWork, IBasketRepo basketRepo, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _basketRepo = basketRepo;
            _config = config;
        }
        public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId)
        {
            StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];

            var basket = await _basketRepo.GetBasketAsync(basketId);
            if (basket == null)
            {
                throw new ArgumentException($"Basket with ID {basketId} not found.");
            }

            var shippingPrice = 0m;

            if (!string.IsNullOrEmpty(basket.DeliveryMethodId))
            {
                var deliveryMethod = await _unitOfWork.Repo<DeliveryMethod>()
                    .GetByIdAsync(basket.DeliveryMethodId);
                shippingPrice = deliveryMethod?.Price ?? 0m;
            }

            foreach (var item in basket.Items)
            {
                var productItem = await _unitOfWork.Repo<TodayWebAPi.DAL.Data.Models.Product>()
                    .GetByIdAsync(item.ProductId);
                if (productItem != null && item.Price != productItem.Price)
                {
                    item.Price = productItem.Price;
                }
            }

            var service = new PaymentIntentService();
            PaymentIntent intent;

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(basket.Items.Sum(i => i.Quantity * i.Price) * 100) + (long)shippingPrice * 100,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                };
                intent = await service.CreateAsync(options);
                basket.PaymentIntentId = intent.Id;
                basket.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long)(basket.Items.Sum(i => i.Quantity * i.Price) * 100) + (long)shippingPrice * 100
                };
                try
                {
                    await service.UpdateAsync(basket.PaymentIntentId, options);
                }
                catch (StripeException ex) when (ex.Message.Contains("No such payment_intent"))
                {
                    var createOptions = new PaymentIntentCreateOptions
                    {
                        Amount = options.Amount,
                        Currency = "usd",
                        PaymentMethodTypes = new List<string> { "card" }
                    };
                    intent = await service.CreateAsync(createOptions);
                    basket.PaymentIntentId = intent.Id;
                    basket.ClientSecret = intent.ClientSecret;
                }
            }

            await _basketRepo.UpdateBasketAsync(basket);
            return basket;
        }
    }
}
