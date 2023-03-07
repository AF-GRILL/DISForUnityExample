using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using GRILLDIS;
using UnityEngine.Events;

public class ButtonManager : MonoBehaviour
{

    public DISGameManager disGameManager;

    [Header("Visual UI")]
    public GameObject visualsTab;
    public Dropdown entityLabels;
    public Dropdown entityTrails;
    public Toggle demoToggle;
    public InputField demoSpeedField;
    public InputField demoTimeField;

    [Header("Networking UI")]
    public GameObject networkingTab;
    public InputField exerciseID;
    public InputField siteID;
    public InputField applicationID;
    public Toggle connectReceiveSocket;
    public Toggle receiveUseMulticast;
    public Toggle receiveLoopback;
    public InputField[] receiverIP;
    public InputField receiverPort;
    public Toggle connectSendSocket;
    public Dropdown sendConnectionType;
    public InputField[] senderIP;
    public InputField senderPort;
    public Button connectButton;
    public Button disconnectButton;
    public GameObject errorMessage;
    [Tooltip("_**EDITOR_ONLY**_")]
    public bool editorConnectOnStart;

    [Header("Entity List UI")]
    public Dropdown followTypeDropDown;
    public GameObject scrollContent;
    
    public class OnDISOptionsChangedEvent : UnityEvent<MenuSettings> { }
    [Header("Events")]
    public OnDISOptionsChangedEvent onDISOptionsChangedEvent;

    public MenuSettings menuSettings;

    private PDUReceiver pduReceiver;
    private PDUSender pduSender;
    private GameObject mainMenu;
    private GameObject options;
    private GameObject gameUI;
    private GameObject entityList;
    private EntityInfo entityInfo;
    private FreeFlyCamera freeFlyCam;
    private bool inMM = true;

    
    public void Awake()
    {
        pduReceiver = disGameManager.gameObject.GetComponent<PDUReceiver>();
        pduSender = disGameManager.gameObject.GetComponent<PDUSender>();
        pduReceiver.OnFailedToConnect.AddListener(handleConnectionError);
        pduSender.OnFailedToConnect.AddListener(handleConnectionError);
        freeFlyCam = GameObject.FindObjectOfType<FreeFlyCamera>();

        mainMenu = this.transform.Find("MainMenu").gameObject;
        options = this.transform.Find("Options").gameObject;
        gameUI = this.transform.Find("GameUI").gameObject;
        entityList = this.transform.Find("EntityList").gameObject;
        entityInfo = this.transform.Find("EntityInfo").gameObject.GetComponent<EntityInfo>();

        freeFlyCam.enabled = false;
        mainMenu.SetActive(true);
        options.SetActive(false);
        gameUI.SetActive(false);
        entityList.SetActive(false);
        entityInfo.gameObject.SetActive(false);

        demoToggle.onValueChanged.AddListener(ToggleDemoCam);
        connectSendSocket.onValueChanged.AddListener(ToggleSendSocket);
        connectReceiveSocket.onValueChanged.AddListener(ToggleRecieveSocket);

        onDISOptionsChangedEvent = new OnDISOptionsChangedEvent();
        onDISOptionsChangedEvent.AddListener(setPluginSettings);

        SetupDropDowns();
        LoadOptions();
        ApplyOptions();
        StartCoroutine(removeCesiumAttibutionWait());
        
    }

    public void Start()
    {
        
#if UNITY_EDITOR
        if (editorConnectOnStart) { LoadOptions(); AttemptConnect(); }
#endif
    }

    public void StartDIS()
    {
        mainMenu.SetActive(false);
        options.gameObject.SetActive(false);
        gameUI.SetActive(true);
        freeFlyCam.enabled = true;
        inMM = false;
    }

    public void OpenOptions()
    {
        if (inMM)
        {
            this.transform.Find("MainMenu").gameObject.SetActive(false);
            this.transform.Find("Options").gameObject.SetActive(true);
        }else
        {
            this.transform.Find("Options").gameObject.SetActive(true);
        }
    }

    public void CloseOptions()
    {
        if (inMM)
        {
            this.transform.Find("MainMenu").gameObject.SetActive(true);
            this.transform.Find("Options").gameObject.SetActive(false);
        }
        else
        {
            this.transform.Find("Options").gameObject.SetActive(false);
        }
    }

    public void OpenEntityList()
    {
        entityList.SetActive(!entityList.activeSelf);
    }

    public void CloseEntityList()
    {
        entityList.SetActive(false);
    }

    public void OpenVisualsTab()
    {
        visualsTab.SetActive(true);
        networkingTab.SetActive(false);
    }

    public void OpenNetworkingTab()
    {
        visualsTab.SetActive(false);
        networkingTab.SetActive(true);
    }

    public void CloseEntityInfo()
    {
        entityInfo.gameObject.SetActive(false);
    }

    public void OpenEntityInfo(GameObject obj)
    {
        entityInfo.Open(obj);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    public void AttemptConnect()
    {
        ApplyOptions();
        connectButton.interactable = false;
        disconnectButton.interactable = true;
        if (pduSender.IsConnected()) { pduSender.Stop(); }
        if (connectSendSocket.isOn) { pduSender.Init(); }
        if (connectReceiveSocket.isOn) { pduReceiver.startUDPReceiver(); }
    }

    public void AttemptDisconnect()
    {
        disconnectButton.interactable = false;
        connectButton.interactable = true;
        pduSender.Stop();
        pduReceiver.stopUDPReceiver();
    }

    public void ApplyOptions()
    {

        //DISGameManager
        menuSettings.SetExerciseID(int.Parse(exerciseID.text));
        menuSettings.SetSiteID(int.Parse(siteID.text));
        menuSettings.SetApplicationID(int.Parse(applicationID.text));

        //Receiver
        menuSettings.SetConnectRecvSocket(connectReceiveSocket.isOn);
        menuSettings.SetIsUsingMulticast(receiveUseMulticast.isOn);
        menuSettings.SetReceiveLoopback(receiveLoopback.isOn);
        menuSettings.SetReceiverIP(InputFieldsToIP(receiverIP));
        menuSettings.SetReceiverPort(int.Parse(receiverPort.text));

        //Sender
        menuSettings.SetConnectSendSocket(connectSendSocket.isOn);
        menuSettings.SetConnectionType((EConnectionType)sendConnectionType.value);
        menuSettings.SetSenderIP(InputFieldsToIP(senderIP));
        menuSettings.SetSenderPort(int.Parse(senderPort.text));

        //Visuals
        menuSettings.SetShowLabels(entityLabels.value == 1);
        menuSettings.SetShowTrails(entityTrails.value == 1);
        menuSettings.SetDemoCamera(demoToggle.isOn);
        menuSettings.SetDemoCamSpeed(float.Parse(demoSpeedField.text));
        menuSettings.SetDemoCamTime(float.Parse(demoTimeField.text));

        MenuSettings.SaveMenuSettings(menuSettings);
        callDISOptionsChangedEvent(menuSettings);
    }

    public void LoadOptions()
    {
        if (MenuSettings.SettingsToLoad())
        {
            menuSettings = MenuSettings.LoadMenuSettings();

            exerciseID.text = menuSettings.GetExerciseID().ToString();
            siteID.text = menuSettings.GetSiteID().ToString();
            applicationID.text = menuSettings.GetApplicationID().ToString();

            connectReceiveSocket.isOn = menuSettings.GetConnectRecvSocket();
            receiveUseMulticast.isOn = menuSettings.GetIsUsingMulticast();
            receiveLoopback.isOn = menuSettings.GetReceiveLoopback();
            IPToInputFields(menuSettings.GetReceiverIP(), receiverIP);
            receiverPort.text = menuSettings.GetReceiverPort().ToString();

            connectSendSocket.isOn = menuSettings.GetConnectSendSocket();
            sendConnectionType.value = ((int)menuSettings.GetConnectionType());
            IPToInputFields(menuSettings.GetSenderIP(), senderIP);
            senderPort.text = menuSettings.GetSenderPort().ToString();

            entityLabels.value = BoolToInt(menuSettings.GetShowLabels());
            entityTrails.value = BoolToInt(menuSettings.GetShowTrails());
            demoToggle.isOn = menuSettings.GetDemoCamera();
            demoSpeedField.text = menuSettings.GetDemoCamSpeed().ToString();
            demoTimeField.text = menuSettings.GetDemoCamTime().ToString();
        }
        else
        {
            menuSettings = new MenuSettings();

            exerciseID.text = disGameManager.ExerciseID.ToString();
            siteID.text = disGameManager.SiteID.ToString();
            applicationID.text = disGameManager.ApplicationID.ToString();

            connectReceiveSocket.isOn = true;
            receiveUseMulticast.isOn = pduReceiver.useMulticast;
            receiveLoopback.isOn = pduReceiver.allowLoopback;
            IPToInputFields(pduReceiver.ipAddressString, receiverIP);
            receiverPort.text = pduReceiver.port.ToString();

            connectSendSocket.isOn = true;
            sendConnectionType.value = (int)pduSender.connectionType;
            IPToInputFields(pduSender.ipAddressString, senderIP);
            senderPort.text = pduSender.port.ToString();

            entityLabels.value = 0;
            entityTrails.value = 0;
            demoToggle.isOn = false;
            demoSpeedField.text = (60.0f).ToString();
            demoTimeField.text = (3.0f).ToString();
        }
    }

    private void callDISOptionsChangedEvent(MenuSettings newSettings)
    {
        if (onDISOptionsChangedEvent != null)
        {
            onDISOptionsChangedEvent.Invoke(newSettings);
        }
    }

    private void setPluginSettings(MenuSettings newSettings)
    {
        disGameManager.ExerciseID = newSettings.GetExerciseID();
        disGameManager.SiteID = newSettings.GetSiteID();
        disGameManager.ApplicationID = newSettings.GetApplicationID();

        pduReceiver.useMulticast = newSettings.GetIsUsingMulticast();
        pduReceiver.allowLoopback = newSettings.GetReceiveLoopback();
        pduReceiver.ipAddressString = newSettings.GetReceiverIP();
        pduReceiver.port = newSettings.GetReceiverPort();

        pduSender.connectionType = newSettings.GetConnectionType();
        pduSender.ipAddressString = newSettings.GetSenderIP();
        pduSender.port = newSettings.GetReceiverPort();
    }


    private void ToggleDemoCam(bool value)
    {
        demoSpeedField.interactable = value;
        demoTimeField.interactable = value;
    }

    private void ToggleSendSocket(bool value)
    {
        sendConnectionType.interactable = value;
        foreach (InputField ip in senderIP) { ip.interactable = value; }
        senderPort.interactable = value;
    }

    private void ToggleRecieveSocket(bool value)
    {
        receiveUseMulticast.interactable = value;
        receiveLoopback.interactable = value;
        foreach (InputField ip in receiverIP) { ip.interactable = value; }
        receiverPort.interactable = value;
    }

    private void SetupDropDowns()
    {
        entityLabels.ClearOptions();
        entityLabels.AddOptions(EnumsToStrings(new EntityLabel()));

        entityTrails.ClearOptions();
        entityTrails.AddOptions(EnumsToStrings(new EntityTrails()));

        sendConnectionType.ClearOptions();
        sendConnectionType.AddOptions(EnumsToStrings(new EConnectionType()));

        followTypeDropDown.ClearOptions();
        followTypeDropDown.AddOptions(EnumsToStrings(new FollowType()));
    }

    private List<string> EnumsToStrings (Enum enumToConvert)
    {
        List<string> list = new List<string>();
        foreach (string enumName in Enum.GetNames(enumToConvert.GetType()))
        {
            list.Add(enumName.ToString().Replace('_', ' '));
        }
        return list;
    }

    private string InputFieldsToIP(InputField[] input)
    {
        string ip = "";
        foreach (InputField IPr in input) { ip += IPr.text + "."; }
        return ip.TrimEnd('.');
    }

    private void IPToInputFields(string ip, InputField[] inputFields)
    {
        string[] splitIP = ip.Split('.');

        if (splitIP.Length != inputFields.Length) { throw new Exception("ERROR: IP Segments do not match Input Field"); }

        int i = 0;
        foreach (string segment in splitIP)
        {
            inputFields[i].text = segment;
            i++;
        }
    }

    public static void removeCesiumAttribution()
    {
        GameObject cesiumCreditSystemGameObject = GameObject.Find("CesiumCreditSystemDefault");
        if (cesiumCreditSystemGameObject != null)
        {
            cesiumCreditSystemGameObject.SetActive(false);
        }
    }

    public void handleConnectionError(Exception ex) {
        Debug.LogException(ex);
        connectButton.interactable = true;
        disconnectButton.interactable = false;
        StartCoroutine(displayError()); 
    }
    private IEnumerator displayError()
    {
        errorMessage.SetActive(true);
        yield return new WaitForSeconds(5.0f);
        errorMessage.SetActive(false);
    }

    IEnumerator removeCesiumAttibutionWait()
    {
        yield return new WaitForSeconds(2.0f);
        removeCesiumAttribution();
    }

    private int BoolToInt(bool boolean) { if (boolean) { return 1; } else { return 0; } }
    private bool IntToBool(int integer) { return (integer == 1); }

}
