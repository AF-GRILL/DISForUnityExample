using GRILLDIS;
using UnityEngine;

public class MenuSettings
{
    public const string KEY_EXERCISE_ID = "ExerciseID";
    public const string KEY_SITE_ID = "SiteID";
    public const string KEY_APPLICATION_ID = "ApplicationID";
    public const string KEY_CONNECT_RECEIVE_SOCKET = "ConnectReceiveSocket";
    public const string KEY_IS_USING_MULTICAST = "UseMulticast";
    public const string KEY_RECEIVE_LOOPBACK = "ReceiveLoopback";
    public const string KEY_RECEIVER_IP = "ReceiverIP";
    public const string KEY_RECEIVER_PORT = "ReceiverPort";
    public const string KEY_SEND_CONNECTION_TYPE = "SendConnectionType";
    public const string KEY_CONNECT_SEND_SOCKET = "ConnectSendSocket";
    public const string KEY_SENDER_IP = "SenderIP";
    public const string KEY_SENDER_PORT = "SenderPort";
    public const string KEY_SHOW_LABELS = "ShowLabels";
    public const string KEY_SHOW_TRAILS = "ShowTrails";
    public const string KEY_DEMO_CAMERA = "DemoCamera";
    public const string KEY_DCAMERA_SPEED = "DemoCamSpeed";
    public const string KEY_DCAMERA_TIME = "DemoCamTime";

    private int exerciseID;
    private int siteID;
    private int applicationID;
    private bool connectRecvSocket;
    private bool isUsingMulticast;
    private bool receiveLoopback;
    private string receiverIP;
    private int receiverPort;
    private bool connectSendSocket;
    private EConnectionType connectionType;
    private string senderIP;
    private int senderPort;
    private bool showLabels;
    private bool showTrails;
    private bool demoCamera;
    private float demoCamSpeed;
    private float demoCamTime;

    public int GetExerciseID() { return exerciseID; }
    public void SetExerciseID(int ExerciseID) { exerciseID = ExerciseID; }

    public int GetSiteID() { return siteID; }
    public void SetSiteID(int SiteID) { siteID = SiteID; }

    public int GetApplicationID() { return applicationID; }
    public void SetApplicationID(int ApplicationID) { applicationID = ApplicationID; }

    public bool GetConnectRecvSocket() { return connectRecvSocket; }
    public void SetConnectRecvSocket(bool ConnectRecvSocket) { connectRecvSocket = ConnectRecvSocket; }

    public bool GetIsUsingMulticast() { return isUsingMulticast; }
    public void SetIsUsingMulticast(bool UseMulticast) { isUsingMulticast = UseMulticast; }

    public bool GetReceiveLoopback() { return receiveLoopback; }
    public void SetReceiveLoopback(bool ReceiveLoopback) { receiveLoopback = ReceiveLoopback; }

    public string GetReceiverIP() { return receiverIP; }
    public void SetReceiverIP(string ReceiverIP) { receiverIP = ReceiverIP; }

    public int GetReceiverPort() { return receiverPort; }
    public void SetReceiverPort(int ReceiverPort) { receiverPort = ReceiverPort; }

    public bool GetConnectSendSocket() { return connectSendSocket; }
    public void SetConnectSendSocket(bool ConnectSendSocket) { connectSendSocket = ConnectSendSocket; }

    public EConnectionType GetConnectionType() { return connectionType; }
    public void SetConnectionType(EConnectionType ConnectionType) { connectionType = ConnectionType; }

    public string GetSenderIP() { return senderIP; }
    public void SetSenderIP(string SenderIP) { senderIP = SenderIP; }

    public int GetSenderPort() { return senderPort; }
    public void SetSenderPort(int SenderPort) { senderPort = SenderPort; }

    public bool GetShowLabels() { return showLabels; }
    public void SetShowLabels(bool ShowLabels) { showLabels = ShowLabels; }

    public bool GetShowTrails() { return showTrails; }
    public void SetShowTrails(bool ShowTrails) { showTrails = ShowTrails; }

    public bool GetDemoCamera() { return demoCamera; }
    public void SetDemoCamera(bool DemoCamera) { demoCamera = DemoCamera; }

    public float GetDemoCamSpeed() { return demoCamSpeed; }
    public void SetDemoCamSpeed(float DemoCamSpeed) { demoCamSpeed = DemoCamSpeed; }

    public float GetDemoCamTime() { return demoCamTime; }
    public void SetDemoCamTime(float DemoCamTime) { demoCamTime = DemoCamTime; }


    public static MenuSettings LoadMenuSettings()
    {
        MenuSettings temp = new MenuSettings();

        temp.SetExerciseID(PlayerPrefs.GetInt(KEY_EXERCISE_ID));
        temp.SetSiteID(PlayerPrefs.GetInt(KEY_SITE_ID));
        temp.SetApplicationID(PlayerPrefs.GetInt(KEY_APPLICATION_ID));
        temp.SetConnectRecvSocket(PlayerPrefs.GetInt(KEY_CONNECT_RECEIVE_SOCKET) == 1);
        temp.SetIsUsingMulticast(PlayerPrefs.GetInt(KEY_IS_USING_MULTICAST) == 1);
        temp.SetReceiveLoopback(PlayerPrefs.GetInt(KEY_RECEIVE_LOOPBACK) == 1);
        temp.SetReceiverIP(PlayerPrefs.GetString(KEY_RECEIVER_IP));
        temp.SetReceiverPort(PlayerPrefs.GetInt(KEY_RECEIVER_PORT));
        temp.SetConnectSendSocket(PlayerPrefs.GetInt(KEY_CONNECT_SEND_SOCKET) == 1);
        temp.SetConnectionType((EConnectionType)PlayerPrefs.GetInt(KEY_SEND_CONNECTION_TYPE));
        temp.SetSenderIP(PlayerPrefs.GetString(KEY_SENDER_IP));
        temp.SetSenderPort(PlayerPrefs.GetInt(KEY_SENDER_PORT));
        temp.SetShowLabels(PlayerPrefs.GetInt(KEY_SHOW_LABELS) == 1);
        temp.SetShowTrails(PlayerPrefs.GetInt(KEY_SHOW_TRAILS) == 1);
        temp.SetDemoCamera(PlayerPrefs.GetInt(KEY_DEMO_CAMERA) == 1);
        temp.SetDemoCamSpeed(PlayerPrefs.GetFloat(KEY_DCAMERA_SPEED));
        temp.SetDemoCamTime(PlayerPrefs.GetFloat(KEY_DCAMERA_TIME));

        return temp;
    }

    public static void SaveMenuSettings(MenuSettings menuSettings)
    {
        PlayerPrefs.SetInt(KEY_EXERCISE_ID, menuSettings.GetExerciseID());
        PlayerPrefs.SetInt(KEY_SITE_ID, menuSettings.GetSiteID());
        PlayerPrefs.SetInt(KEY_APPLICATION_ID, menuSettings.GetApplicationID());

        //Receiver
        PlayerPrefs.SetInt(KEY_CONNECT_RECEIVE_SOCKET, BoolToInt(menuSettings.GetConnectRecvSocket()));
        PlayerPrefs.SetInt(KEY_IS_USING_MULTICAST, BoolToInt(menuSettings.GetIsUsingMulticast()));
        PlayerPrefs.SetInt(KEY_RECEIVE_LOOPBACK, BoolToInt(menuSettings.GetReceiveLoopback()));
        PlayerPrefs.SetString(KEY_RECEIVER_IP, menuSettings.GetReceiverIP());
        PlayerPrefs.SetInt(KEY_RECEIVER_PORT, menuSettings.GetReceiverPort());

        //Sender
        PlayerPrefs.SetInt(KEY_CONNECT_SEND_SOCKET, BoolToInt(menuSettings.GetConnectSendSocket()));
        PlayerPrefs.SetInt(KEY_SEND_CONNECTION_TYPE, (int) menuSettings.GetConnectionType());
        PlayerPrefs.SetString(KEY_SENDER_IP, menuSettings.GetSenderIP());
        PlayerPrefs.SetInt(KEY_SENDER_PORT, menuSettings.GetSenderPort());

        //Visuals
        PlayerPrefs.SetInt(KEY_SHOW_LABELS, BoolToInt(menuSettings.GetShowLabels()));
        PlayerPrefs.SetInt(KEY_SHOW_TRAILS, BoolToInt(menuSettings.GetShowTrails()));
        PlayerPrefs.SetInt(KEY_DEMO_CAMERA, BoolToInt(menuSettings.GetDemoCamera()));
        PlayerPrefs.SetFloat(KEY_DCAMERA_SPEED, menuSettings.GetDemoCamSpeed());
        PlayerPrefs.SetFloat(KEY_DCAMERA_TIME, menuSettings.GetDemoCamTime());

        PlayerPrefs.Save();
    }

    public static bool SettingsToLoad() { return PlayerPrefs.HasKey(KEY_EXERCISE_ID); }

    public static int BoolToInt(bool boolean) { if (boolean) { return 1; } else { return 0; } }

}
