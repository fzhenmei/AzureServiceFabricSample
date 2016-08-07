﻿using System;
using System.ServiceModel;
using Microsoft.ServiceFabric.Services.Client;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using Common;
namespace SimpleStoreClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri ServiceName = new Uri("fabric:/SimpleStoreApplication/ShoppingCartService");
            ServicePartitionResolver serviceResolver = new ServicePartitionResolver(() => new FabricClient());
            NetTcpBinding binding = CreateClientConnectionBinding();
            Client shoppingClient = new Client(new WcfCommunicationClientFactory<IShoppingCartService>(binding, null, serviceResolver), ServiceName);
            shoppingClient.AddItem(new ShoppingCartItem
            {
                ProductName = "XBOX ONE",
                UnitPrice = 329.0,
                Amount = 2
            }).Wait();
            var list = shoppingClient.GetItems().Result;
            foreach(var item in list)
            {
                Console.WriteLine(string.Format("{0}: {1:C2} X {2} = {3:C2}",
                    item.ProductName,
                    item.UnitPrice,
                    item.Amount,
                    item.LineTotal));
            }
            Console.ReadKey();
        }

        private static NetTcpBinding CreateClientConnectionBinding()
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None)
            {
                SendTimeout = TimeSpan.MaxValue,
                ReceiveTimeout = TimeSpan.MaxValue,
                OpenTimeout = TimeSpan.FromSeconds(5),
                CloseTimeout = TimeSpan.FromSeconds(5),
                MaxReceivedMessageSize = 1024 * 1024
            };
            binding.MaxBufferSize = (int)binding.MaxReceivedMessageSize;
            binding.MaxBufferPoolSize = Environment.ProcessorCount * binding.MaxReceivedMessageSize;
            return binding;
        }
    }
}
