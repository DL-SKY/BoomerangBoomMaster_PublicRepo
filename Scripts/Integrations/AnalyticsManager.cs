using DllSky.Patterns;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsManager : Singleton<AnalyticsManager>, IAnalyticsManager
{
    #region Variables
    public MonoBehaviour[] components;

    private List<IAnalyticsManager> managers = new List<IAnalyticsManager>();
    #endregion

    #region Unity methods
    private void Awake()
    {
        managers.Clear();

        foreach (var obj in components)
            managers.Add(obj as IAnalyticsManager);
    }
    #endregion

    #region Public methods
    public void SendAnalytics(AnalyticsData _data)
    {
        foreach (var manager in managers)
            manager.SendAnalytics(_data);

        Debug.LogWarning(string.Format("[Analytics] Send : {0}", _data.customEventName));        
    }
    #endregion
}

public interface IAnalyticsManager
{
    void SendAnalytics(AnalyticsData _data);
}

public class AnalyticsData
{
    public string customEventName = "";
    public Dictionary<string, object> eventData = new Dictionary<string, object>();
}

public static class ConstantsAnalyticsEventName
{
    public const string PROFILE = "profile";

    public const string MARKET = "market";
    public const string INVENTORY = "inventory";
    public const string BOOSTER = "booster";

    public const string GAME_OVER = "game_over";

    public const string BONUS_CODE = "bonus_code";
}

public static class ConstantsAnalyticsEventParam
{
    public const string ACTION = "action";
    public const string STATUS = "status";

    public const string ID = "id";
    public const string AMOUNT = "amount";

    public const string LVL = "level";
    public const string EXP = "exp";
    public const string MONEY = "money";
    public const string RESOURCE = "resource";
    public const string SCORE = "score";
    public const string MAX_SCORE = "max_score";
    public const string ADD_MONEY = "add_money";

    public const string DAYS_IN_GAME = "days_in_game";
}

public static class ConstantsAnalyticsEventValue
{
    public const string BUY = "buy";
    public const string SELECT = "select";

    public const string ADS = "ads";
    public const string COMMON = "common";
}
