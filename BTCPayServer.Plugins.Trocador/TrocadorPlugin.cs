using System;
using System.Reflection;
using BTCPayServer.Abstractions.Contracts;
using Microsoft.Extensions.DependencyInjection;
using BTCPayServer.Abstractions.Models;
using BTCPayServer.Abstractions.Services;

namespace BTCPayServer.Plugins.Trocador
{
    public class TrocadorPlugin : BaseBTCPayServerPlugin
    {
        public override IBTCPayServerPlugin.PluginDependency[] Dependencies { get; } =
        {
            new() { Identifier = nameof(BTCPayServer), Condition = ">=2.0.0" }
        };

        public override void Execute(IServiceCollection services)
        {
            services.AddSingleton<TrocadorService>();

            // Sidebar Nav
            services.AddUIExtension("store-integrations-nav", "Trocador/TrocadorNav");
            // Store Settings Plugins Integration
            services.AddUIExtension("store-integrations-list", "Trocador/StoreIntegrationTrocadorOption");

            // -- Checkout v2 --

            // Tab (Payment Method)
            services.AddUIExtension("checkout-payment-method", "Trocador/CheckoutV2/CheckoutPaymentMethodExtension");
            // Widget
            services.AddUIExtension("checkout-payment", "Trocador/CheckoutV2/CheckoutPaymentExtension");


            // -- Checkout No-Script --
            services.AddUIExtension("checkout-noscript-end", "Trocador/CheckoutNoScript/CheckoutPaymentExtension");

            base.Execute(services);
        }
    }

}
