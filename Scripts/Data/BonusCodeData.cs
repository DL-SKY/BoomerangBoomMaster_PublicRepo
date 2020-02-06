using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BonusCodeData
{
    public string id;
    public Dictionary<string, int> prizes = new Dictionary<string, int>();
}

public static class Codes
{
    public static List<BonusCodeData> list = new List<BonusCodeData>()
    {
        new BonusCodeData()                                                 //Промо-код для Пикабу-сообщества
        {
            id = "PIKABU",
            prizes =
            {
                { DefaultInventoryData.IdPikabuBonus, 1 },
                { ConstantsResource.MONEY, 500 },
                { EnumBoosterType.Freeze.ToString(), 3 },
                { EnumBoosterType.HitPoints.ToString(), 2 },                
                { EnumBoosterType.Score.ToString(), 1 },
            },
        },

        new BonusCodeData()
        {
            id = "GoldPlease",
            prizes =
            {
                { ConstantsResource.MONEY, 750 },
            },
        },

        new BonusCodeData()
        {
            id = "d2Xor8",
            prizes =
            {
                { ConstantsResource.MONEY, 500 },
            },
        },

        new BonusCodeData()
        {
            id = "B95pY3",
            prizes =
            {
                { ConstantsResource.MONEY, 500 },
            },
        },

        new BonusCodeData()
        {
            id = "iA6F9k",
            prizes =
            {
                { ConstantsResource.MONEY, 500 },
            },
        },

        new BonusCodeData()
        {
            id = "KjHH6a",
            prizes =
            {
                { ConstantsResource.MONEY, 500 },
            },
        },

        new BonusCodeData()
        {
            id = "7kSM3K",
            prizes =
            {
                { ConstantsResource.MONEY, 500 },
            },
        },

        new BonusCodeData()                                                 //На форс-мажор?
        {
            id = "D8NoLY",
            prizes =
            {
                { ConstantsResource.MONEY, 1000 },
                { EnumBoosterType.Freeze.ToString(), 1 },
                { EnumBoosterType.HitPoints.ToString(), 1 },
                { EnumBoosterType.Score.ToString(), 1 },
            },
        },

        new BonusCodeData()                                                 //На форс-мажор?
        {
            id = "NSt93a",
            prizes =
            {
                { ConstantsResource.MONEY, 1000 },
                { EnumBoosterType.Freeze.ToString(), 1 },
                { EnumBoosterType.HitPoints.ToString(), 1 },
                { EnumBoosterType.Score.ToString(), 1 },
            },
        },

        new BonusCodeData()                                                 //ДР Мартишки?
        {
            id = "Martha",
            prizes =
            {
                { ConstantsResource.MONEY, 1000 },
                { EnumBoosterType.Freeze.ToString(), 4 },
                { EnumBoosterType.HitPoints.ToString(), 2 },
                { EnumBoosterType.Score.ToString(), 1 },
            },
        },

        new BonusCodeData()                                                 //День матери?
        {
            id = "ILoveMyMom",
            prizes =
            {
                { ConstantsResource.MONEY, 1000 },
                { EnumBoosterType.Freeze.ToString(), 6 },
                { EnumBoosterType.HitPoints.ToString(), 4 },
                { EnumBoosterType.Score.ToString(), 2 },
            },
        },

        new BonusCodeData()                                                 //Новый год/рождество?
        {
            id = "HoHoHo",
            prizes =
            {
                { ConstantsResource.MONEY, 1500 },
                { EnumBoosterType.Freeze.ToString(), 6 },
                { EnumBoosterType.HitPoints.ToString(), 4 },
                { EnumBoosterType.Score.ToString(), 2 },
            },
        },
    };
}

