using UnityEngine;
using System.Collections;
using DllSky.Patterns;
using UnityEngine.Purchasing;
using System;
using UnityEngine.Purchasing.Security;

public class IapManager : Singleton<IapManager>, IStoreListener
{
    #region Variables
    public Action EventInitialized;

    private IStoreController controller;
    private IExtensionProvider extensions;

    private Action callbackOk;
    private Action callbackFailed;
    #endregion

    #region Unity methods
    private void Start()
    {
        Initialize();
    }
    #endregion

    #region Public methods
    public bool IsInitialized()
    {
#if UNITY_EDITOR
        return true;
#endif
        return controller != null && extensions != null;
    }

    public string GetPrice(string _productId)
    {
        Product product = controller?.products?.WithID(_productId);
        return product?.metadata?.localizedPriceString ?? "";
    }

    public void BuyProductID(string _productId, Action _callbackOk = null, Action _callbackFailed = null)
    {
#if UNITY_EDITOR
        _callbackOk?.Invoke();
        return;
#endif
        try
        {
            if (IsInitialized())
            {
                Product product = controller.products.WithID(_productId);

                callbackOk = _callbackOk;
                callbackFailed = _callbackFailed;

                if (product != null && product.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                    // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
                    controller.InitiatePurchase(product);
                }
                else
                {
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            else
            {
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }
        catch (Exception e)
        {
            Debug.Log("BuyProductID: FAIL. Exception during purchase. " + e);
        }
    }

    //public void RestorePurchases()
    //{
    //    if (!IsInitialized())
    //    {
    //        Debug.Log("RestorePurchases FAIL. Not initialized.");
    //        return;
    //    }

    //    if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
    //    {
    //        Debug.Log("RestorePurchases started ...");

    //        var apple = extensions.GetExtension<IAppleExtensions>();
    //        apple.RestoreTransactions((result) =>
    //        {
    //            Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
    //        });
    //    }
    //    else
    //    {
    //        Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
    //    }
    //}
    #endregion

    #region Private methods
    private void Initialize()
    {
        Debug.LogWarning("[IAP] Start Initialize()");

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        foreach (var item in DefaultMarketData.items)
            if (item.resourcePrice == ConstantsResource.IAP)
                builder.AddProduct(item.id, item.productType,
                                    new IDs
                                    {
                                        {item.androidID, GooglePlay.Name},
                                        //{item.iosID, MacAppStore.Name}
                                    });

        foreach (var item in DefaultMarketData.offers)
            if (item.resourcePrice == ConstantsResource.IAP)
                builder.AddProduct(item.id, item.productType,
                                    new IDs
                                    {
                                        {item.androidID, GooglePlay.Name},
                                        //{item.iosID, MacAppStore.Name}
                                    });        

        UnityPurchasing.Initialize(this, builder);
    }  
    #endregion

    #region Coroutines
    #endregion

    #region IStoreListener
    /// <summary>
    /// Called when Unity IAP is ready to make purchases.
    /// </summary>
    public void OnInitialized(IStoreController _controller, IExtensionProvider _extensions)
    {
        Debug.LogWarning("[IAP] Complete Initialize");

        controller = _controller;
        extensions = _extensions;

        EventInitialized?.Invoke();
    }

    /// <summary>
    /// Called when Unity IAP encounters an unrecoverable initialization error.
    ///
    /// Note that this will not be called if Internet is unavailable; Unity IAP
    /// will attempt initialization until it becomes available.
    /// </summary>
    public void OnInitializeFailed(InitializationFailureReason _error)
    {
        Debug.LogError("[IAP] Failed Initialize");
    }

    /// <summary>
    /// Called when a purchase completes.
    ///
    /// May be called at any time after OnInitialized().
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs _e)
    {
        //callbackOk?.Invoke();
        //return PurchaseProcessingResult.Complete;

        bool validPurchase = true; // Presume valid for platforms with no R.V.

        // Unity IAP's validation logic is only included on these platforms.
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
        // Prepare the validator with the secrets we prepared in the Editor
        // obfuscation window.
        var validator = new CrossPlatformValidator
                            (
                                GooglePlayTangle.Data(),
                                AppleTangle.Data(), 
                                Application.identifier
                            );

        try
        {
            // On Google Play, result has a single product ID.
            // On Apple stores, receipts contain multiple products.
            var result = validator.Validate(_e.purchasedProduct.receipt);
            // For informational purposes, we list the receipt(s)
            Debug.Log("Receipt is valid. Contents:");
            foreach (IPurchaseReceipt productReceipt in result)
            {
                Debug.Log(productReceipt.productID);
                Debug.Log(productReceipt.purchaseDate);
                Debug.Log(productReceipt.transactionID);
            }
        }
        catch (IAPSecurityException)
        {
            Debug.Log("Invalid receipt, not unlocking content");
            validPurchase = false;
        }
#endif

        if (validPurchase)
        {
            callbackOk?.Invoke();
        }
        else
        {
            callbackFailed?.Invoke();
        }

        return PurchaseProcessingResult.Complete;
    }

    /// <summary>
    /// Called when a purchase fails.
    /// </summary>
    public void OnPurchaseFailed(Product _i, PurchaseFailureReason _p)
    {
        callbackFailed?.Invoke();
    }
    #endregion
}
