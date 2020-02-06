using UnityEngine;
using System.Collections;
using DllSky.Patterns;
using Unity.Notifications.Android;
using System;
using System.Linq;

//https://docs.unity3d.com/Packages/com.unity.mobile.notifications@1.0/manual/index.html

public enum EnumNotificationChannels
{
    ID_Notification,
    ID_Reminder,    
}

public enum EnumNotificationID
{
    ID_OfferD1,
    ID_OfferD3,
    ID_OfferD7,

    ID_OneDaysAgo,
    ID_ThreeDaysAgo,
}

/* (ID) УВЕДОМЛЕНИЯ

    (Уведомления/ID_Notification)
 * (ID_OfferD1) Предложение 1го дня
 * (ID_OfferD3) Предложение 3го дня
 * (ID_OfferD7) Предложение 7го дня
 * 

    (Напоминания/ID_Reminder)
 * (ID_ThreeDaysAgo) Отсутствие в игре 3 дня 
 * 

 */

public class NotificationManager : Singleton<NotificationManager>
{
    #region Variables
    public float pauseBeforeInit = 5.0f;

    [Space()]
    [SerializeField]
    private bool IsInit = false;
    #endregion

    #region Unity methods
    public void Start()
    {
        StartCoroutine(PauseBeforeInit());
    }
    #endregion

    #region Public methods
    public void SendAllNotifications()
    {
        //TODO: временно отключил
        SendAll();
    }

    public void ClearAllNotifications()
    {
        ClearAll();
    }
    #endregion

    #region Private methods
    private void CheckChannels()
    {
        var arrChannels = (EnumNotificationChannels[])Enum.GetValues(typeof(EnumNotificationChannels));

#if (UNITY_ANDROID || UNITY_EDITOR)
        var channels = AndroidNotificationCenter.GetNotificationChannels();

        //Удаляем лишние Каналы
        foreach (var channel in channels)
            if (!arrChannels.Any(x => x.ToString() == channel.Id))
                AndroidNotificationCenter.DeleteNotificationChannel(channel.Id);

        //Добавляем недостающие Каналы
        foreach (var channel in arrChannels)
            if (!channels.Any(x => x.Id == channel.ToString()))
            {
                var localization = LocalizationManager.Instance;
                var id = channel.ToString();

                var newChannel = new AndroidNotificationChannel()
                {
                    Id = id,
                    Name = localization.GetString(string.Format("{0}_Name", id)),
                    Importance = GetImportance(channel),
                    Description = localization.GetString(string.Format("{0}_Description", id)),
                    EnableVibration = true,
                };

                AndroidNotificationCenter.RegisterNotificationChannel(newChannel);
            }        
#elif (UNITY_IOS || UNITY_IPHONE)
        
#endif

        IsInit = true;
    }

    private Importance GetImportance(EnumNotificationChannels _channel)
    {
        var result = Importance.Default;

        switch (_channel)
        {
            case EnumNotificationChannels.ID_Notification:
                result = Importance.High;
                break;
            case EnumNotificationChannels.ID_Reminder:
                result = Importance.Low;
                break;

            default:
                result = Importance.Default;
                break;
        }

        return result;
    }

    private void ClearAll()
    {
#if (UNITY_ANDROID || UNITY_EDITOR)
        AndroidNotificationCenter.CancelAllNotifications();
#elif (UNITY_IOS || UNITY_IPHONE)

#endif
    }

    private void SendAll()
    {
        var instance = GameManager.Instance;
        var localization = LocalizationManager.Instance;
        var now = DateTime.Now;

        Debug.LogWarning("DayFirstSessionUTC: " + instance.dayFirstSessionUTC.ToString());

#if (UNITY_ANDROID || UNITY_EDITOR)
        //Специальное предложение 1го дня
        var id = EnumNotificationID.ID_OfferD1.ToString();
        var notification = new AndroidNotification();
        //if (instance.GetDayInGame() < 1)
        //{
        //    notification.Title = localization.GetString(string.Format("{0}_Title", id));
        //    notification.Text = localization.GetString(string.Format("{0}_Text", id));
        //    notification.FireTime = instance.dayFirstSessionUTC.ToLocalTime().AddDays(1);
        //    notification.SmallIcon = "icon_0";
        //    notification.LargeIcon = "icon_1";
        //    Send(notification, EnumNotificationChannels.ID_Notification.ToString());

        //    Debug.LogWarning(string.Format("[Notification] id: {0}, time: {1}", id, notification.FireTime.ToString()));
        //}

        //Специальное предложение 3го дня        
        //if (instance.GetDayInGame() < 3)
        //{
        //    id = EnumNotificationID.ID_OfferD3.ToString();
        //    notification = new AndroidNotification();
        //    notification.Title = localization.GetString(string.Format("{0}_Title", id));
        //    notification.Text = localization.GetString(string.Format("{0}_Text", id));
        //    notification.FireTime = instance.dayFirstSessionUTC.ToLocalTime().AddDays(3);
        //    notification.SmallIcon = "icon_0";
        //    notification.LargeIcon = "icon_1";
        //    Send(notification, EnumNotificationChannels.ID_Notification.ToString());

        //    Debug.LogWarning(string.Format("[Notification] id: {0}, time: {1}", id, notification.FireTime.ToString()));
        //}

        //Специальное предложение 7го дня        
        //if (instance.GetDayInGame() < 7)
        //{
        //    id = EnumNotificationID.ID_OfferD7.ToString();
        //    notification = new AndroidNotification();
        //    notification.Title = localization.GetString(string.Format("{0}_Title", id));
        //    notification.Text = localization.GetString(string.Format("{0}_Text", id));
        //    notification.FireTime = instance.dayFirstSessionUTC.ToLocalTime().AddDays(7);
        //    notification.SmallIcon = "icon_0";
        //    notification.LargeIcon = "icon_1";
        //    Send(notification, EnumNotificationChannels.ID_Notification.ToString());

        //    Debug.LogWarning(string.Format("[Notification] id: {0}, time: {1}", id, notification.FireTime.ToString()));
        //}

        //Test
        id = "TEST02";
        notification = new AndroidNotification();
        notification.Title = localization.GetString(string.Format("{0}_Title", id));
        notification.Text = localization.GetString(string.Format("{0}_Text", id));
        notification.FireTime = now.AddSeconds(15);
        notification.SmallIcon = "icon_2";
        notification.LargeIcon = "icon_1";
        Send(notification, EnumNotificationChannels.ID_Reminder.ToString());
        id = "TEST03";
        notification = new AndroidNotification();
        notification.Title = localization.GetString(string.Format("{0}_Title", id));
        notification.Text = localization.GetString(string.Format("{0}_Text", id));
        notification.FireTime = now.AddSeconds(20);
        notification.SmallIcon = "icon_3";
        //notification.LargeIcon = "icon_1";
        Send(notification, EnumNotificationChannels.ID_Reminder.ToString());

        //Test
        id = "TEST01";
        notification = new AndroidNotification();
        notification.Title = localization.GetString(string.Format("{0}_Title", id));
        notification.Text = localization.GetString(string.Format("{0}_Text", id));
        notification.FireTime = now.AddSeconds(10);
        notification.SmallIcon = "icon_0";
        notification.LargeIcon = "icon_1";
        Send(notification, EnumNotificationChannels.ID_Reminder.ToString());

        Debug.LogWarning(string.Format("[Notification] id: {0}, time: {1}", id, notification.FireTime.ToString()));

        //1 дня без игры
        id = EnumNotificationID.ID_OneDaysAgo.ToString();
        notification = new AndroidNotification();
        notification.Title = localization.GetString(string.Format("{0}_Title", id));
        notification.Text = localization.GetString(string.Format("{0}_Text", id));
        var fireDay1 = DateTime.Now.AddDays(1);
        notification.FireTime = new DateTime(fireDay1.Year, fireDay1.Month, fireDay1.Day, 12, 0, 0);
        notification.SmallIcon = "icon_0";
        notification.LargeIcon = "icon_1";
        Send(notification, EnumNotificationChannels.ID_Reminder.ToString());

        Debug.LogWarning(string.Format("[Notification] id: {0}, time: {1}", id, notification.FireTime.ToString()));

        //3 дня без игры
        id = EnumNotificationID.ID_ThreeDaysAgo.ToString();
        notification = new AndroidNotification();
        notification.Title = localization.GetString(string.Format("{0}_Title", id));
        notification.Text = localization.GetString(string.Format("{0}_Text", id));
        var fireDay3 = DateTime.Now.AddDays(3);
        notification.FireTime = new DateTime(fireDay3.Year, fireDay3.Month, fireDay3.Day, 12, 0, 0);
        notification.SmallIcon = "icon_0";
        notification.LargeIcon = "icon_1";
        Send(notification, EnumNotificationChannels.ID_Reminder.ToString());

        Debug.LogWarning(string.Format("[Notification] id: {0}, time: {1}", id, notification.FireTime.ToString()));
#elif (UNITY_IOS || UNITY_IPHONE)

#endif
    }

    private void Send(object _notification, string _channel)
    {
        if (!IsInit)
            CheckChannels();

#if (UNITY_ANDROID || UNITY_EDITOR)
        var notification = (AndroidNotification)_notification;
        AndroidNotificationCenter.SendNotification(notification, _channel);
#elif (UNITY_IOS || UNITY_IPHONE)

#endif
    }
    #endregion

    #region Coroutines
    private IEnumerator PauseBeforeInit()
    {
        yield return new WaitForSeconds(pauseBeforeInit);

        CheckChannels();
    }
    #endregion
}
