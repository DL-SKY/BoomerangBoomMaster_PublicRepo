using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;

[Serializable]
public class MarketData
{
    public List<MarketItemData> items = new List<MarketItemData>();

    public bool isStartedOfferD1;
    public bool isEndOfferD1;
    public DateTime dateStartedOfferD1
    {
        get
        {
            if (string.IsNullOrEmpty(dateTextD1))
                return new DateTime();
            else
                return DateTime.Parse(dateTextD1, System.Globalization.CultureInfo.InvariantCulture);
        }
        set
        {
            dateTextD1 = value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
    }
    public string dateTextD1;

    public bool isStartedOfferD3;
    public bool isEndOfferD3;
    public DateTime dateStartedOfferD3
    {
        get
        {
            if (string.IsNullOrEmpty(dateTextD3))
                return new DateTime();
            else
                return DateTime.Parse(dateTextD3, System.Globalization.CultureInfo.InvariantCulture);
        }
        set
        {
            dateTextD3 = value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
    }
    public string dateTextD3;

    public bool isStartedOfferD7;
    public bool isEndOfferD7;
    public DateTime dateStartedOfferD7
    {
        get
        {
            if (string.IsNullOrEmpty(dateTextD7))
                return new DateTime();
            else
                return DateTime.Parse(dateTextD7, System.Globalization.CultureInfo.InvariantCulture);
        }
        set
        {
            dateTextD7 = value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
    }
    public string dateTextD7;

    public List<string> bonuses = new List<string>();

    public void Validate()
    {
        //Удаляем неактуальное, обновляем поля у существующих
        for (int i = items.Count-1; i >= 0; i--)
        {
            var item = items[i];
            var origin = DefaultMarketData.items.Find(x => x.id == item.id);

            if (origin == null)
            {
                items.RemoveAt(i);
            }
            else
            {
                item.sortIndex = origin.sortIndex;
                item.resourcePrice = origin.resourcePrice;
                item.price = origin.price;
                item.resourceItem = origin.resourceItem;
                item.amount = origin.amount;
            }
        }

        //Добавляем недостающие
        foreach (var origin in DefaultMarketData.items)
            if (!items.Any(x => x.id == origin.id))
                items.Add
                    (
                        new MarketItemData()
                        {
                            id = origin.id,
                            sortIndex = origin.sortIndex,
                            resourcePrice = origin.resourcePrice,
                            productType = origin.productType,
                            androidID = origin.androidID,
                            iosID = origin.iosID,
                            price = origin.price,
                            resourceItem = origin.resourceItem,
                            amount = origin.amount,
                        }
                    );
    }
}

[Serializable]
public class MarketItemData
{
    public string id;

    public int sortIndex;

    //Тип товара (за что продается)
    public string resourcePrice;

    //Идентификаторы товара (для IAP)
    public ProductType productType;
    public string androidID;
    public string iosID;

    //Цена (не для IAP)
    public int price;

    //Покупка
    public string resourceItem;
    public int amount;
}



[Serializable]
public class InventoryrData
{
    public string activeBoomerang = DefaultInventoryData.IdStandardBoomerang;

    public List<InventoryItemData> items = new List<InventoryItemData>();

    public void SetActiveBoomerang(string _id)
    {
        activeBoomerang = _id;
        GameManager.Instance.EventChangeBoomerang?.Invoke();
    }

    public void Validate()
    {
        //Удаляем неактуальное, обновляем поля у существующих
        for (int i = items.Count - 1; i >= 0; i--)
        {
            var item = items[i];
            var origin = DefaultInventoryData.items.Find(x => x.id == item.id);

            if (origin == null)
            {
                items.RemoveAt(i);
            }
            else
            {
                item.sortIndex = origin.sortIndex;
                item.isHide = origin.isHide;
                item.hp = origin.hp;
                item.freeze = origin.freeze;
                item.resource = origin.resource;
                item.price = origin.price;
            }
        }

        //Добавляем недостающие
        foreach (var origin in DefaultInventoryData.items)
            if (!items.Any(x => x.id == origin.id))
                items.Add
                    (
                        new InventoryItemData()
                        {
                            id = origin.id,
                            sortIndex = origin.sortIndex,
                            isActive = origin.isActive,
                            isHide = origin.isHide,
                            hp = origin.hp,
                            freeze = origin.freeze,
                            resource = origin.resource,
                            price = origin.price,
                        }
                    );
    }
}

[Serializable]
public class InventoryItemData
{
    public string id;

    public int sortIndex;

    public bool isActive;
    public bool isHide;

    //Характеристики бумеранга
    public int hp;
    public float freeze;

    //Стоимость
    public string resource;
    public int price;
}



[Serializable]
public class MissionsData
{
    public List<MissionsItemData> items = new List<MissionsItemData>();

    public void Validate()
    {

    }
}

[Serializable]
public class MissionsItemData
{
    public string id;

    //Условие
    public string mission;
    public float maxProgress;

    //Прогресс
    public float progress;

    //Награда
    public string resource;
    public int amount;
}



[Serializable]
public class BoostersData
{
    public int boosterHP = 0;
    public int boosterFreeze = 0;
    public int boosterScore = 0;

    public static BoostersData GetDefault()
    {
        var data = new BoostersData();

        data.boosterHP = 1;
        data.boosterFreeze = 1;
        data.boosterScore = 1;

        return data;
    }

    public int GetBoosterCount(EnumBoosterType _type)
    {
        var count = 0;

        switch (_type)
        {
            case EnumBoosterType.HitPoints:
                count = boosterHP;
                break;
            case EnumBoosterType.Freeze:
                count = boosterFreeze;
                break;
            case EnumBoosterType.Score:
                count = boosterScore;
                break;
        }

        return Mathf.Max(0, count);
    }

    public void DecrementBoosterCount(EnumBoosterType _type)
    {
        switch (_type)
        {
            case EnumBoosterType.HitPoints:
                boosterHP = Mathf.Max(0, boosterHP - 1);
                break;
            case EnumBoosterType.Freeze:
                boosterFreeze = Mathf.Max(0, boosterFreeze - 1);
                break;
            case EnumBoosterType.Score:
                boosterScore = Mathf.Max(0, boosterScore - 1);
                break;
        }
    }

    public bool GetBoostersMarker()
    {
        var result = false;

        var boosters = new int[] { boosterHP, boosterFreeze, boosterScore, };
        result = boosters.Any(x => x > 0);

        return result;
    }
}
