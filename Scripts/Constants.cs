using System.Collections.Generic;

//Тулза для иконок
//https://jgilfelt.github.io/AndroidAssetStudio/index.html

//Плагин GPGS
//https://github.com/playgameservices/play-games-plugin-for-unity

//IAP
//https://habr.com/ru/post/320224/
//https://docs.unity3d.com/Manual/UnityIAPInitialization.html
//https://docs.unity3d.com/Manual/UnityIAPValidatingReceipts.html

//Настройка разделителя, региональные стандарты
//float.Parse(settingsConfig.Find(x => x.id == "volume_sound").value, System.Globalization.CultureInfo.InvariantCulture);

//Размер объекта и камера http://www.unity3d.ru/distribution/viewtopic.php?f=105&t=36909
//
//Rect borders = new Rect(0, 0, 8, 8);
//float maxSize = borders.height / 2f - (borders.width / borders.height);
//camera.orthographicSize = maxSize;
//
//для широкоформатной камеры  Camera.main.orthographicSize = длинаПлашки * Camera.main.pixelHeight / Camera.main.pixelWidth * .5f;
//Для вертикальной камеры     Camera.main.orthographicSize = высотаПлашки * Camera.main.pixelWidth / Camera.main.pixelHeight * .5f;

#region CONSTANTS
public static class ConstantsSettings
{
    //Заглавный прелоадер
    public const float mainPreloaderTimer = 1.5f;

    //Уровни
    public static readonly float[] levels = { 0, 100, 250, 500, 750, 1000, 1250, 1500, 1750, 2000, 2500, 3000, 3500, 4000, 4750, 5500, 6250, 7000, 8000, 9000, 10000, 11000, 12000, 13000, 14000, 16500, 19000, 21500, 24000, 26500, 29000, 31500, 34000, 36500, 39000, 44000, 49000, 54000, 59000, 64000, 69000, 74000, 79000, 84000, 89000, 94000, 99000, 104000, 109000, 114000, };

    //Шанс появления в Режиме Игры
    public const int maxMoney = 500;
    public const float minMoneyChance = 50.0f;
    public const float maxMoneyChance = 100.0f;

    public const float maxHPChance = 10.0f;

    public const float maxTimeFreezeNormalize = 5.0f;               //1.0 - шанс даже при полной шкале
    public const float minTimeFreezeChance = 15.0f;
    public const float maxTimeFreezeChance = 50.0f;

    public const float blockChance = 100.0f;

    //Комбо
    public const int maxCombo = 4;

    //Изменение скорости игры (на основе кол-ва полученных очков рейтинга)
    public const int maxScoreFromSpeed = 5000;

    //Макс рекламы в день (наградной)
    public const int maxAds = 25;
    //Кол-во возвратов в Меню для вызова межстраничной/промежуточной рекламы
    public const int returnsToShowVideo = 3;

    //Кол-во восстановлений здоровья за рекламу за сессию
    public const int maxRestoreHitPointsAds = 1;
    //Таймер принятия решения восстановления здоровья за рекламу
    public const float timerRestoreHitPointsAdsDialog = 6.0f;
    //Стомость восстановления здоровья в монетах
    public const int priceToRestore = 25;

    //Бустеры
    public const float boosterScoreCoeff = 2.0f;

    //Камера
    public const float camWidthUnits = 5.0f;

    //Фон
    public const float bgWidth = 512;
    public const float bgHeight = 1024;
}

public static class ConstantsTag
{
    public const string TAG_BOOMERANG = "Boomerang";

    public const string TAG_OBSTACLE = "Obstacle";
    public const string TAG_TIME = "Time";
    public const string TAG_HP = "HitPoints";
    public const string TAG_MONEY = "Money";
}

public static class ConstantsPlayerPrefs
{
    public const string SPEED = "Speed";
    public const string SCORE = "Score";
    public const string EXP = "Experience";
    public const string MONEY = "Money";

    public const string SETTING_SOUND = "Sound";
    public const string SETTING_LANG = "Language";

    public const string MARKET = "Market";
    public const string INVENTORY = "Inventory";
    public const string MISSIONS = "Missions";
    public const string BOOSTERS = "Boosters";

    public const string DAY_FIRST_SESSION = "DayFirstSession";
    public const string LAST_UPDATE_STATISTIC = "LastUpdateStatistic";
    public const string MONEY_TODAY = "MoneyToday";
    public const string ADS_TODAY = "AdsToday";
}

public static class ConstantsLanguage
{
    public const string RUSSIAN = "rus";
    public const string ENGLISH = "eng";
    public const string DEUTSCH = "deu";
}

public static class ConstantsAds
{
    public const string REWARDED = "rewardedVideo";
    public const string INTERSTITIAL = "video";
}

public static class ConstantsResource
{
    //За что можно покупать
    public const string MONEY = "money";
    public const string IAP = "iap";
    public const string ADS = "ads";

    //Что можно покупать
    public const string IdMarketKingPackRes = "IdMarketKingPackRes";

    public const string IdMarketBoosterPack01Res = "IdMarketBoosterPack01Res";
    public const string IdMarketBoosterPack02Res = "IdMarketBoosterPack02Res";

    public const string IdOfferD1Res = "IdOfferD1Res";
    public const string IdOfferD3Res = "IdOfferD3Res";
    public const string IdOfferD7Res = "IdOfferD7Res";
}
#endregion

#region DEFAULT DATA
public static class DefaultMarketData
{
    /*
     ======== МОНЕТИЗАЦИЯ IAP ========

     * 1 руб = 10-16 монет 

     * 1 000 монет =                            100 руб     2.5$
     * 2 500 монет =                            200 руб     5$
     * 5 000 монет =                            350 руб     8.75$
     * 10 000 монет =                           600 руб     15$

     * бустеры F20, H10, S5 =                   100 руб     2.5$
     * бустеры F125, H75, S25 =                 500 руб     12.5$
     
     * 25 000 монет + бустеры F125, H75, S25 =  1500 руб    37.5$
     
     * бустер Freeze (F) =                                          25 монет    / 2,5 руб   / 4 шт          
     * бустер HP (H) =                                              50 монет    / 5 руб     / 2 шт          
     * бустер Score (S) =                                           100 монет   / 10 руб    / 1 шт    
     
     * OfferD1 (2500 монет + F20, H10, S5)      200 руб     5$
     * OfferD3 (5000 монет + F20, H10, S5)      350 руб     8.75$
     * OfferD7 (5000 монет + F40, H25, S10)     400 руб     10$

     */

    public const string IdMarketMoney1000 =     "IdMarketMoney1000";            //market_money_1000   -   ID in Android
    public const string IdMarketMoney2500 =     "IdMarketMoney2500";            //market_money_2500
    public const string IdMarketMoney5000 =     "IdMarketMoney5000";            //market_money_5000
    public const string IdMarketMoney10000 =    "IdMarketMoney10000";           //market_money_10000

    public const string IdMarketBoosterPack01 = "IdMarketBoosterPack01";        //market_booster_pack_01
    public const string IdMarketBoosterPack02 = "IdMarketBoosterPack02";        //market_booster_pack_02

    public const string IdMarketKingPack =      "IdMarketKingPack";             //market_king_pack

    public const string IdBoosterTimeFreeze =   "IdBoosterTimeFreeze";        
    public const string IdBoosterHitPoints =    "IdBoosterHitPoints";        
    public const string IdBoosterScore =        "IdBoosterScore";        

    public static List<MarketItemData> items = new List<MarketItemData>()
                                                {
                                                    new MarketItemData()
                                                    {
                                                        id = IdMarketMoney1000,
                                                        sortIndex = 10,
                                                        resourcePrice = ConstantsResource.IAP,
                                                        productType = UnityEngine.Purchasing.ProductType.Consumable,
                                                        androidID = "market_money_1000",                                                        
                                                        iosID = "",
                                                        price = 0,
                                                        resourceItem = ConstantsResource.MONEY,
                                                        amount = 1000,
                                                    },

                                                    new MarketItemData()
                                                    {
                                                        id = IdMarketMoney2500,
                                                        sortIndex = 20,
                                                        resourcePrice = ConstantsResource.IAP,
                                                        productType = UnityEngine.Purchasing.ProductType.Consumable,
                                                        androidID = "market_money_2500",
                                                        iosID = "",
                                                        price = 0,
                                                        resourceItem = ConstantsResource.MONEY,
                                                        amount = 2500,
                                                    },

                                                    new MarketItemData()
                                                    {
                                                        id = IdMarketMoney5000,
                                                        sortIndex = 30,
                                                        resourcePrice = ConstantsResource.IAP,
                                                        productType = UnityEngine.Purchasing.ProductType.Consumable,
                                                        androidID = "market_money_5000",
                                                        iosID = "",
                                                        price = 0,
                                                        resourceItem = ConstantsResource.MONEY,
                                                        amount = 5000,
                                                    },

                                                    new MarketItemData()
                                                    {
                                                        id = IdMarketMoney10000,
                                                        sortIndex = 40,
                                                        resourcePrice = ConstantsResource.IAP,
                                                        productType = UnityEngine.Purchasing.ProductType.Consumable,
                                                        androidID = "market_money_10000",
                                                        iosID = "",
                                                        price = 0,
                                                        resourceItem = ConstantsResource.MONEY,
                                                        amount = 10000,
                                                    },

                                                    new MarketItemData()
                                                    {
                                                        id = IdMarketBoosterPack01,
                                                        sortIndex = 50,
                                                        resourcePrice = ConstantsResource.IAP,
                                                        productType = UnityEngine.Purchasing.ProductType.Consumable,
                                                        androidID = "market_booster_pack_01",
                                                        iosID = "",
                                                        price = 0,
                                                        resourceItem = ConstantsResource.IdMarketBoosterPack01Res,
                                                        amount = 1,
                                                    },

                                                    new MarketItemData()
                                                    {
                                                        id = IdMarketBoosterPack02,
                                                        sortIndex = 60,
                                                        resourcePrice = ConstantsResource.IAP,
                                                        productType = UnityEngine.Purchasing.ProductType.Consumable,
                                                        androidID = "market_booster_pack_02",
                                                        iosID = "",
                                                        price = 0,
                                                        resourceItem = ConstantsResource.IdMarketBoosterPack02Res,
                                                        amount = 1,
                                                    },

                                                    new MarketItemData()
                                                    {
                                                        id = IdMarketKingPack,
                                                        sortIndex = 70,
                                                        resourcePrice = ConstantsResource.IAP,
                                                        productType = UnityEngine.Purchasing.ProductType.Consumable,
                                                        androidID = "market_king_pack",
                                                        iosID = "",
                                                        price = 0,
                                                        resourceItem = ConstantsResource.IdMarketKingPackRes,
                                                        amount = 1,
                                                    },

                                                    new MarketItemData()
                                                    {
                                                        id = IdBoosterTimeFreeze,
                                                        sortIndex = 80,
                                                        resourcePrice = ConstantsResource.MONEY,
                                                        productType = UnityEngine.Purchasing.ProductType.Consumable,
                                                        androidID = "",
                                                        iosID = "",
                                                        price = 25,
                                                        resourceItem = EnumBoosterType.Freeze.ToString(),
                                                        amount = 1,
                                                    },

                                                    new MarketItemData()
                                                    {
                                                        id = IdBoosterHitPoints,
                                                        sortIndex = 90,
                                                        resourcePrice = ConstantsResource.MONEY,
                                                        productType = UnityEngine.Purchasing.ProductType.Consumable,
                                                        androidID = "",
                                                        iosID = "",
                                                        price = 50,
                                                        resourceItem = EnumBoosterType.HitPoints.ToString(),
                                                        amount = 1,
                                                    },

                                                    new MarketItemData()
                                                    {
                                                        id = IdBoosterScore,
                                                        sortIndex = 100,
                                                        resourcePrice = ConstantsResource.MONEY,
                                                        productType = UnityEngine.Purchasing.ProductType.Consumable,
                                                        androidID = "",
                                                        iosID = "",
                                                        price = 100,
                                                        resourceItem = EnumBoosterType.Score.ToString(),
                                                        amount = 1,
                                                    },
                                                };


    public const string IdOfferD1 = "IdOfferD1";                                //special_offer_d1   -   ID in Android
    public const string IdOfferD3 = "IdOfferD3";                                //special_offer_d2
    public const string IdOfferD7 = "IdOfferD7";                                //special_offer_d3

    public static List<MarketItemData> offers = new List<MarketItemData>()
                                                {
                                                    new MarketItemData()
                                                    {
                                                        id = IdOfferD1,
                                                        sortIndex = 0,
                                                        resourcePrice = ConstantsResource.IAP,
                                                        productType = UnityEngine.Purchasing.ProductType.Consumable,
                                                        androidID = "special_offer_d1",                                                        
                                                        iosID = "",
                                                        price = 0,
                                                        resourceItem = ConstantsResource.IdOfferD1Res,
                                                        amount = 1,
                                                    },

                                                    new MarketItemData()
                                                    {
                                                        id = IdOfferD3,
                                                        sortIndex = 0,
                                                        resourcePrice = ConstantsResource.IAP,
                                                        productType = UnityEngine.Purchasing.ProductType.Consumable,
                                                        androidID = "special_offer_d2",
                                                        iosID = "",
                                                        price = 0,
                                                        resourceItem = ConstantsResource.IdOfferD3Res,
                                                        amount = 1,
                                                    },

                                                    new MarketItemData()
                                                    {
                                                        id = IdOfferD7,
                                                        sortIndex = 0,
                                                        resourcePrice = ConstantsResource.IAP,
                                                        productType = UnityEngine.Purchasing.ProductType.Consumable,
                                                        androidID = "special_offer_d3",
                                                        iosID = "",
                                                        price = 0,
                                                        resourceItem = ConstantsResource.IdOfferD7Res,
                                                        amount = 1,
                                                    },
                                                };
}

public static class DefaultResources
{
    public static List<BonusCodeData> list = new List<BonusCodeData>()
    {
        new BonusCodeData()
        {
            id = ConstantsResource.IdOfferD1Res,
            prizes =
            {
                { ConstantsResource.MONEY, 2500 },
                { EnumBoosterType.Freeze.ToString(), 20 },
                { EnumBoosterType.HitPoints.ToString(), 10 },
                { EnumBoosterType.Score.ToString(), 5 },
            },
        },

        new BonusCodeData()
        {
            id = ConstantsResource.IdOfferD3Res,
            prizes =
            {
                { ConstantsResource.MONEY, 5000 },
                { EnumBoosterType.Freeze.ToString(), 20 },
                { EnumBoosterType.HitPoints.ToString(), 10 },
                { EnumBoosterType.Score.ToString(), 5 },
            },
        },

        new BonusCodeData()
        {
            id = ConstantsResource.IdOfferD7Res,
            prizes =
            {
                { ConstantsResource.MONEY, 5000 },
                { EnumBoosterType.Freeze.ToString(), 40 },
                { EnumBoosterType.HitPoints.ToString(), 25 },
                { EnumBoosterType.Score.ToString(), 10 },
            },
        },

        new BonusCodeData()
        {
            id = ConstantsResource.IdMarketBoosterPack01Res,
            prizes =
            {
                { EnumBoosterType.Freeze.ToString(), 20 },
                { EnumBoosterType.HitPoints.ToString(), 10 },
                { EnumBoosterType.Score.ToString(), 5 },
            },
        },

        new BonusCodeData()
        {
            id = ConstantsResource.IdMarketBoosterPack02Res,
            prizes =
            {
                { EnumBoosterType.Freeze.ToString(), 125 },
                { EnumBoosterType.HitPoints.ToString(), 75 },
                { EnumBoosterType.Score.ToString(), 25 },
            },
        },

        new BonusCodeData()
        {
            id = ConstantsResource.IdMarketKingPackRes,
            prizes =
            {
                { ConstantsResource.MONEY, 25000 },
                { EnumBoosterType.Freeze.ToString(), 125 },
                { EnumBoosterType.HitPoints.ToString(), 75 },
                { EnumBoosterType.Score.ToString(), 25 },
            },
        },
    };
}

public static class DefaultInventoryData
{
    //Hidden/Bonus
    public const string IdPikabuBonus = "PikabuBonus";                      // [+] price: 0 (4-5.0)

    public const string IdStandardBoomerang = "StandardBoomerang";          // [+] price: 0 (3-4.0)
    public const string IdSecondBoomerang = "SecondBoomerang";              // [+] price: ADS (3-4.0)        //Как обычный, только другой цвет

    public const string IdThirdBoomerang = "ThirdBoomerang";                // [+] price: 150 (3-4.5)
    public const string IdFourthBoomerang = "FourthBoomerang";              // [+] price: 150 (3-4.5)
    public const string IdSpinnerBoomerang = "SpinnerdBoomerang";           // [] price: 150 (3-4.5)

    public const string IdBananaBoomerang = "BananaBoomerang";              // [] price: 250 (3-5.0)
    public const string IdButton = "Button";                                // [] price: 250 (3-5.0)        //Пуговица

    public const string IdDonut = "Donut";                                  // [+] price: 350 (4-5.0)        

    public const string IdWrench = "Wrench";                                // [] price: 500 (5-3.0)        //Гаечный ключ
    public const string IdEggBoomerang = "EggBoomerang";                    // [] price: 500 (4-4.5)

    public const string IdShieldBoomerang = "ShieldBoomerang";              // [+] price: 1000 (5-4.0)

    public const string IdBatarang = "Batarang";                            // [+] price: 1500 (3-6.0)

    public const string IdMjolnir = "Mjolnir";                              // [] price: 2500 (5-5.0)
    public const string IdStar = "Star";                                    // [+] price: 2500 (4-6.0)

    public const string IdUmbrella = "Umbrella";                            // [+] price: 5000 (8-6.0)    

    public static List<InventoryItemData> items = new List<InventoryItemData>()
                                                {
                                                    new InventoryItemData()
                                                    {
                                                        id = IdStandardBoomerang,
                                                        sortIndex = 0,
                                                        isActive = true,
                                                        isHide = false,
                                                        hp = 3,
                                                        freeze = 4.0f,
                                                        resource = ConstantsResource.MONEY,
                                                        price = 0,
                                                    },

                                                    new InventoryItemData()
                                                    {
                                                        id = IdPikabuBonus,
                                                        sortIndex = 0,
                                                        isActive = false,
                                                        isHide = true,
                                                        hp = 4,
                                                        freeze = 5.0f,
                                                        resource = ConstantsResource.MONEY,
                                                        price = 0,
                                                    },
                                                    //IdSecondBoomerang = "SecondBoomerang";              // [+] price: ADS (3-4.0)        //Как обычный, только другой цвет
                                                    new InventoryItemData()
                                                    {
                                                        id = IdSecondBoomerang,
                                                        sortIndex = 0,
                                                        isActive = false,
                                                        isHide = false,
                                                        hp = 3,
                                                        freeze = 4.0f,
                                                        resource = ConstantsResource.ADS,
                                                        price = 0,
                                                    },
                                                    //IdThirdBoomerang = "ThirdBoomerang";                // [+] price: 150 (3-4.5)
                                                    new InventoryItemData()
                                                    {
                                                        id = IdThirdBoomerang,
                                                        sortIndex = 0,
                                                        isActive = false,
                                                        isHide = false,
                                                        hp = 3,
                                                        freeze = 4.5f,
                                                        resource = ConstantsResource.MONEY,
                                                        price = 150,
                                                    },                                                    
                                                    //IdFourthBoomerang = "FourthBoomerang";              // *[] price: 150 (3-4.5)
                                                    new InventoryItemData()
                                                    {
                                                        id = IdFourthBoomerang,
                                                        sortIndex = 0,
                                                        isActive = false,
                                                        isHide = false,
                                                        hp = 3,
                                                        freeze = 4.5f,
                                                        resource = ConstantsResource.MONEY,
                                                        price = 150,
                                                    },
                                                    //IdSpinnerBoomerang = "SpinnerdBoomerang";           // [+] price: 150 (3-4.5)
                                                    //new InventoryItemData()
                                                    //{
                                                    //    id = IdSpinnerBoomerang,
                                                    //    sortIndex = 0,
                                                    //    isActive = false,
                                                    //    isHide = false,
                                                    //    hp = 3,
                                                    //    freeze = 4.5f,
                                                    //    resource = ConstantsResource.MONEY,
                                                    //    price = 150,
                                                    //},
                                                    //IdBananaBoomerang = "BananaBoomerang";              // [+] price: 250 (3-5.0)
                                                    //new InventoryItemData()
                                                    //{
                                                    //    id = IdBananaBoomerang,
                                                    //    sortIndex = 0,
                                                    //    isActive = false,
                                                    //    isHide = false,
                                                    //    hp = 3,
                                                    //    freeze = 5.0f,
                                                    //    resource = ConstantsResource.MONEY,
                                                    //    price = 250,
                                                    //},
                                                    //IdButton = "Button";                                // [+] price: 250 (3-5.0)        //Пуговица
                                                    //new InventoryItemData()
                                                    //{
                                                    //    id = IdButton,
                                                    //    sortIndex = 0,
                                                    //    isActive = false,
                                                    //    isHide = false,
                                                    //    hp = 3,
                                                    //    freeze = 5.0f,
                                                    //    resource = ConstantsResource.MONEY,
                                                    //    price = 250,
                                                    //},                                                    
                                                    //IdDonut = "Donut";                                  // *[] price: 350 (4-5.0)        
                                                    new InventoryItemData()
                                                    {
                                                        id = IdDonut,
                                                        sortIndex = 0,
                                                        isActive = false,
                                                        isHide = false,
                                                        hp = 4,
                                                        freeze = 5.0f,
                                                        resource = ConstantsResource.MONEY,
                                                        price = 350,
                                                    },
                                                    //IdWrench = "Wrench";                                // [+] price: 500 (5-3.0)        //Гаечный ключ
                                                    //new InventoryItemData()
                                                    //{
                                                    //    id = IdWrench,
                                                    //    sortIndex = 0,
                                                    //    isActive = false,
                                                    //    isHide = false,
                                                    //    hp = 5,
                                                    //    freeze = 3.0f,
                                                    //    resource = ConstantsResource.MONEY,
                                                    //    price = 500,
                                                    //},
                                                    //IdEggBoomerang = "EggBoomerang";                    // [+] price: 500 (4-4.5)
                                                    //new InventoryItemData()
                                                    //{
                                                    //    id = IdEggBoomerang,
                                                    //    sortIndex = 0,
                                                    //    isActive = false,
                                                    //    isHide = false,
                                                    //    hp = 4,
                                                    //    freeze = 4.5f,
                                                    //    resource = ConstantsResource.MONEY,
                                                    //    price = 500,
                                                    //},
                                                    //IdShieldBoomerang = "ShieldBoomerang";              // [+] price: 1000 (5-4.0)
                                                    new InventoryItemData()
                                                    {
                                                        id = IdShieldBoomerang,
                                                        sortIndex = 0,
                                                        isActive = false,
                                                        isHide = false,
                                                        hp = 5,
                                                        freeze = 4.0f,
                                                        resource = ConstantsResource.MONEY,
                                                        price = 1000,
                                                    },
                                                    //IdBatarang = "Batarang";                            // [+] price: 1500 (3-6.0)
                                                    new InventoryItemData()
                                                    {
                                                        id = IdBatarang,
                                                        sortIndex = 0,
                                                        isActive = false,
                                                        isHide = false,
                                                        hp = 3,
                                                        freeze = 6.0f,
                                                        resource = ConstantsResource.MONEY,
                                                        price = 1500,
                                                    },
                                                    //IdMjolnir = "Mjolnir";                              // [+] price: 2500 (5-5.0)
                                                    //new InventoryItemData()
                                                    //{
                                                    //    id = IdMjolnir,
                                                    //    sortIndex = 0,
                                                    //    isActive = false,
                                                    //    isHide = false,
                                                    //    hp = 5,
                                                    //    freeze = 5.0f,
                                                    //    resource = ConstantsResource.MONEY,
                                                    //    price = 2500,
                                                    //},                                                    
                                                    //IdStar = "Star";                                    // *[] price: 2500 (4-6.0)
                                                    new InventoryItemData()
                                                    {
                                                        id = IdStar,
                                                        sortIndex = 0,
                                                        isActive = false,
                                                        isHide = false,
                                                        hp = 4,
                                                        freeze = 6.0f,
                                                        resource = ConstantsResource.MONEY,
                                                        price = 2500,
                                                    },
                                                    //IdUmbrella = "Umbrella";                            // [+] price: 5000 (8-6.0) 
                                                    new InventoryItemData()
                                                    {
                                                        id = IdUmbrella,
                                                        sortIndex = 0,
                                                        isActive = false,
                                                        isHide = false,
                                                        hp = 8,
                                                        freeze = 6.0f,
                                                        resource = ConstantsResource.MONEY,
                                                        price = 5000,
                                                    },
                                                };
}

public static class DefaultMissionData
{
    public const string IdMission01 = "";

    public static List<MissionsItemData> items = new List<MissionsItemData>()
                                                {
                                                    new MissionsItemData()
                                                    {
                                                        id = IdMission01,
                                                        mission = "",
                                                        maxProgress = 1.0f,
                                                        progress = 0.0f,
                                                        resource = "",
                                                        amount = 0,
                                                    },
                                                };
}
#endregion

#region ENUM
public enum EnumGameStatus
{
    StartApp,

    MainMenu,

    Pause,

    Game,
}

public enum EnumPreloaders
{
    Loading,

    Common,
}

public enum EnumUI
{
    MainMenu,

    GameMenu,
    Settings,
    Market,
    Inventory,
    Missions,

    Credits,

    Pause,
    RespawnDialog,

    GameOver,

    ClaimDialog,
}

public enum EnumBoosterType
{
    HitPoints,
    Freeze,
    Score,
}

public enum EnumObstacles
{
    NA,

    Small,
    Horizontal,

    //Vertical,

    Money,
    Time,
    HP,
}
#endregion
