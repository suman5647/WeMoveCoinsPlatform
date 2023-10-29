using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WMC.Data;
using WMC.Logic.Models;

namespace WMC.Logic
{
    public class WebhookUtility
    {
        public static async Task PublishOrder(long orderId)
        {
            var dataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));

            var order = dataUnitOfWork.Orders.GetById(orderId);
            if (!String.IsNullOrEmpty(order.ReferenceId) && !String.IsNullOrEmpty(order.MerchantCode))
            {
                var merchant = dataUnitOfWork.Merchants.Get(x => x.MerchantCode == order.MerchantCode).FirstOrDefault();
                if (!String.IsNullOrEmpty(merchant.MerchantWebhookUrl))
                {

                    WebhookResponse data = GetMerchantData(order.Id);
                    var callPost = await PostData(merchant.MerchantWebhookUrl, data);
                }
            }
        }

        private static async Task<HttpResponseMessage> PostData(string merchantUrl, WebhookResponse webhookResponse)
        {
            HttpClient client = new HttpClient();
            HttpContent c = new StringContent(JsonConvert.SerializeObject(webhookResponse), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(merchantUrl, c).ConfigureAwait(false);
            return response;
        }

        private static WebhookResponse GetMerchantData(long orderId)
        {
            var dataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
            var order = dataUnitOfWork.Orders.GetById(orderId);

            //get the incoming transaction
            var inputTxs = dataUnitOfWork.Transactions.Get(x => x.OrderId == order.Id && x.Type == 1)
                .Select(q => new TransactionsLite
                {
                    OrderId = q.OrderId,
                    ExtRef = q.ExtRef,
                    Amount = q.Amount,
                    Currency = dataUnitOfWork.Currencies.Get(x => x.Id == q.Currency).Select(x => x.Code).FirstOrDefault(),
                    Type = dataUnitOfWork.TransactionTypes.Get(x => x.Id == q.Type).Select(x => x.Text).FirstOrDefault()
                })
                .ToList();
            //get the outing transaction 
            var outputTxs = dataUnitOfWork.Transactions.Get(x => x.OrderId == order.Id && x.Type == 2 && x.ToAccount == 9)
                .Select(q => new TransactionsLite
                {
                    OrderId = q.OrderId,
                    ExtRef = q.ExtRef,
                    Amount = q.Amount,
                    Currency = dataUnitOfWork.Currencies.Get(x => x.Id == q.Currency).Select(x => x.Code).FirstOrDefault(),
                    Type = dataUnitOfWork.TransactionTypes.Get(x => x.Id == q.Type).Select(x => x.Text).FirstOrDefault()
                })
                .ToList();

            var getOrderStatus = dataUnitOfWork.OrderStatus.Get(x => x.Id == order.Status).Select(q => q.Text).FirstOrDefault();
            var merchantName = dataUnitOfWork.Sites.Get(x => x.Id == order.SiteId).Select(x => x.Text).FirstOrDefault();

            List<TransactionsLite> Txs = new List<TransactionsLite>
                (new TransactionsLite[]
                {

                });
            WebhookResponse webhookResponse = new WebhookResponse
            {
                OrderId = 123,
                OrderStatus = "OrderStatus",
                ReferenceId = "ReferenceId",
                Rate = (decimal?)123,
                Transactions = Txs
            };

            foreach (var inputTx in inputTxs)
            {
                webhookResponse.Transactions.Add(inputTx);
            }
            foreach (var outputTx in outputTxs)
            {
                webhookResponse.Transactions.Add(outputTx);
            }

            webhookResponse.OrderId = order.Id;
            webhookResponse.OrderStatus = getOrderStatus;
            webhookResponse.ReferenceId = order.ReferenceId;
            webhookResponse.Rate = order.Rate;
            webhookResponse.Currency = order.CurrencyCode;
            webhookResponse.MerchantName = merchantName;
            return webhookResponse;
        }

        public delegate void OrderStateHandler(object sender, OrderStateChangedArgs e);
        public class OrderStateChangedArgs
        {
            public long OrderId { get; set; }
            public string Status { get; set; }
        }

        // single instance
        public static class OrderStatusChangedMessageBroker
        {
            static OrderStatusChangedMessageBroker()
            {
                //OrderStateChanged += MerchantRelayMessageBroker_OrderStateChanged;
            }

            public static void OrderState(object sender, OrderStateChangedArgs e)
            {
                OrderStateChanged?.Invoke(sender, e);
            }

            public static event OrderStateHandler OrderStateChanged;
        }

        // single instance
        //public static class MerchantWebHook
        //{
        //    static MerchantWebHook()
        //    {
        //        OrderStatusChangedMessageBroker.OrderStateChanged += OrderStatusChangedMessageBroker_OrderStateChanged;
        //    }

        //    private static async void OrderStatusChangedMessageBroker_OrderStateChanged(object sender, OrderStateChangedArgs e)
        //    {
        //        // Relay messaging / retrier
        //        // check order is to handle merchant webhook
        //        // get order and trn data
        //        // call merchant webhook
        //        var dataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
        //        RetrierSettings retrierAppSettings = SettingsManager.GetDefault().Get("RetrierSettings").GetJsonData<RetrierSettings>();
        //        var getPaymentContent = new Retrier<WebhookResponse>().Try(() =>
        //        {
        //            var order = dataUnitOfWork.Orders.GetById(e.OrderId);
        //            if (!String.IsNullOrEmpty(order.ReferenceId) && !String.IsNullOrEmpty(order.MerchantCode))
        //            {
        //                var merchant = dataUnitOfWork.Merchants.Get(x => x.MerchantCode == order.MerchantCode).FirstOrDefault();
        //                if (!String.IsNullOrEmpty(merchant.MerchantWebhookUrl))
        //                {
        //                    var webhook = new WebhookUtility();
        //                    WebhookResponse data = await GetMerchantData(order.Id);
        //                    var callPost = await PostData(merchant.MerchantWebhookUrl, data);
        //                }
        //            }
        //            return new WebhookResponse();
        //        }
        //        , int.Parse(retrierAppSettings.MaxRetries), int.Parse(retrierAppSettings.DelayInMilliseconds));

        //    }
        //}

        public class OrderProcessor
        {
            public void Completed(long orderId)
            {
                OrderStatusChangedMessageBroker.OrderState(this, new OrderStateChangedArgs() { OrderId = orderId, Status = "Completed" });
            }

            public void Cancelled(long orderId)
            {
                OrderStatusChangedMessageBroker.OrderState(this, new OrderStateChangedArgs() { OrderId = orderId, Status = "Cancelled" });
            }

            public void KYCCancelled(long orderId)
            {
                OrderStatusChangedMessageBroker.OrderState(this, new OrderStateChangedArgs() { OrderId = orderId, Status = "KYCCancelled" });
            }
        }

    }
}
