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
            services.AddUIExtension("Trocador/TrocadorNav", "store-integrations-nav");
            // Store Settings Plugins Integration
            services.AddUIExtension("Trocador/StoreIntegrationTrocadorOption", "store-integrations-list");

            // -- Checkout v2 --

            // Tab (Payment Method)
            services.AddUIExtension("Trocador/CheckoutV2/CheckoutPaymentMethodExtension", "checkout-payment-method");
            // Widget
            services.AddUIExtension("Trocador/CheckoutV2/CheckoutPaymentExtension", "checkout-payment");

            // -- Checkout Classic --

            // Tab
            services.AddUIExtension("Trocador/CheckoutClassic/CheckoutTabExtension", "checkout-bitcoin-post-tabs");
            services.AddUIExtension("Trocador/CheckoutClassic/CheckoutTabExtension", "checkout-lightning-post-tabs");
            // Widget
            services.AddUIExtension("Trocador/CheckoutClassic/CheckoutContentExtension", "checkout-bitcoin-post-content");
            services.AddUIExtension("Trocador/CheckoutClassic/CheckoutContentExtension", "checkout-lightning-post-content");

            services.AddUIExtension("Trocador/CheckoutEnd", "checkout-end");

            // -- Checkout No-Script --
            services.AddUIExtension("Trocador/CheckoutNoScript/CheckoutPaymentExtension", "checkout-noscript-end");

            base.Execute(services);
        }
    }

}
