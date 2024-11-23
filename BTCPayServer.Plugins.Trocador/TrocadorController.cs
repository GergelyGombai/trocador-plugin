using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BTCPayServer.Abstractions.Constants;
using BTCPayServer.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace BTCPayServer.Plugins.Trocador
{
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Cookie)]
    [Authorize(Policy = Policies.CanModifyStoreSettings, AuthenticationSchemes = AuthenticationSchemes.Cookie)]
    [Route("plugins/{storeId}/Trocador")]
    public class TrocadorController : Controller
    {
        private readonly BTCPayNetworkProvider _btcPayNetworkProvider;

        private readonly BTCPayServerClient _btcPayServerClient;
        private readonly TrocadorService _TrocadorService;

        public TrocadorController(BTCPayNetworkProvider btcPayNetworkProvider,
            BTCPayServerClient btcPayServerClient, TrocadorService TrocadorService)
        {
            _btcPayNetworkProvider = btcPayNetworkProvider;
            _btcPayServerClient = btcPayServerClient;
            _TrocadorService = TrocadorService;
        }

        [HttpGet("")]
        public async Task<IActionResult> UpdateTrocadorSettings(string storeId)
        {
            var store = await _btcPayServerClient.GetStore(storeId);

            UpdateTrocadorSettingsViewModel vm = new UpdateTrocadorSettingsViewModel();
            vm.StoreName = store.Name;
            TrocadorSettings Trocador = null;
            try
            {
                Trocador = await _TrocadorService.GetTrocadorForStore(storeId);
            }
            catch (Exception)
            {
                // ignored
            }

            SetExistingValues(Trocador, vm);

            if (vm.Enabled)
            {
                Dictionary<string, string> newPaymentMethods = new Dictionary<string, string>();

                var paymentMethods = (await _btcPayServerClient.GetStorePaymentMethods(storeId));

                foreach (var paymentMethod in paymentMethods)
                {
                    object data = paymentMethod.Config;

                    string cryptoCode = paymentMethod.PaymentMethodId;
                    var network = _btcPayNetworkProvider.GetNetwork(cryptoCode);
                    string label = network != null ? network.DisplayName : cryptoCode;

                    if (cryptoCode == label && (cryptoCode.EndsWith("LightningNetwork") || cryptoCode.EndsWith("LNURLPAY")))
                    {
                        label = "Lightning";
                    }


                    newPaymentMethods[label] = cryptoCode;

                }

                vm.PaymentMethods = newPaymentMethods;
            }

            return View(vm);
        }

        private void SetExistingValues(TrocadorSettings existing, UpdateTrocadorSettingsViewModel vm)
        {
            if (existing == null)
                return;
            vm.Enabled = existing.Enabled;
            vm.FiatDenominated = existing.FiatDenominated;
            vm.DefaultPaymentMethodId = existing.DefaultPaymentMethodId;
            vm.ReferralCode = existing.ReferralCode;
            vm.PaymentMethodTitle = existing.PaymentMethodTitle;
            vm.PaymentMethodSubtitle = existing.PaymentMethodSubtitle;
            vm.PreselectedCoin = existing.PreselectedCoin;
            vm.ShowFirst = existing.ShowFirst;
        }

        [HttpPost("")]
        public async Task<IActionResult> UpdateTrocadorSettings(string storeId, UpdateTrocadorSettingsViewModel vm,
            string command)
        {
            if (vm.Enabled)
            {
                if (!ModelState.IsValid)
                {
                    return View(vm);
                }
            }

            var TrocadorSettings = new TrocadorSettings()
            {
                Enabled = vm.Enabled,
                FiatDenominated = vm.FiatDenominated,
                DefaultPaymentMethodId = vm.DefaultPaymentMethodId,
                ReferralCode = vm.ReferralCode,
                PaymentMethodTitle = vm.PaymentMethodTitle,
                PaymentMethodSubtitle = vm.PaymentMethodSubtitle,
                PreselectedCoin = vm.PreselectedCoin,
                ShowFirst = vm.ShowFirst,
            };

            if (!string.IsNullOrEmpty(vm.PaymentMethodTitle))
            {
                TrocadorSettings.PaymentMethodTitle = vm.PaymentMethodTitle;
            }
            else {
                TrocadorSettings.PaymentMethodTitle = "Altcoins";
            }

            switch (command)
            {
                case "save":
                    await _TrocadorService.SetTrocadorForStore(storeId, TrocadorSettings);
                    TempData["SuccessMessage"] = "Trocador settings modified";
                    return RedirectToAction(nameof(UpdateTrocadorSettings), new { storeId });

                default:
                    return View(vm);
            }
        }
    }
}
