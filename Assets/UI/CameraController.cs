using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenDis.Dis1998;
using GRILLDIS;
using CesiumForUnity;

public class CameraController : MonoBehaviour
{
    public GameObject scrollContent;
    public Dropdown followTypeDropDown;
    public GameObject EntitySelectionPrefab;
    public Camera mainCam;
    public DISGameManager disGameManager;
    public GameObject CesiumGeoreference;

    private GameObject camArm;
    private GameObject camRotator;

    private Vector3 prevPos;
    private Vector3 nextPos;
    private List<EntityListEntry> entries = new List<EntityListEntry>();
    private FollowType currentFollow;

    private bool demoCam = false;
    private float demoCamSpeed = 60.0f;
    private float demoCamTime = 10.0f;
    private float demoTimeElapsed;

    public void Start()
    {
        followTypeDropDown.onValueChanged.AddListener(ChangeFollowOption);
        disGameManager.e_CreateDISEntity.AddListener(OnDISEntityCreated);
        disGameManager.e_DestroyDISEntity.AddListener(OnDISEntityDestroyed);

        ButtonManager bm = GameObject.FindObjectOfType<ButtonManager>();
        SetSettings(bm.menuSettings);
        bm.onDISOptionsChangedEvent.AddListener(SetSettings);

        if (CesiumGeoreference == null)
        {
            CesiumGeoreference = FindObjectOfType<CesiumGeoreference>().gameObject;
        }

        SetUpCameraArm();
    }


    public void Update()
    {
        if (demoCam)
        {
            if (entries.Count == 0) { return; }

            camRotator.transform.Rotate(Vector3.up, demoCamSpeed * Time.deltaTime, Space.World);
            demoTimeElapsed += Time.deltaTime;
            if (demoTimeElapsed > demoCamTime)
            {
                EntityListEntry.selected = entries[(entries.IndexOf(EntityListEntry.selected) + 1) % entries.Count];
                ChangeView(FollowType.Orbital_View_Global);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            prevPos = mainCam.ScreenToViewportPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            nextPos = mainCam.ScreenToViewportPoint(Input.mousePosition);
            camRotator.transform.Rotate(Vector3.right, (prevPos.y - nextPos.y) * 180);
            camRotator.transform.Rotate(Vector3.up, -(prevPos.x - nextPos.x) * 180, Space.World);
            prevPos = nextPos;
        }
        if ((int)currentFollow >= 2)
        {
            mainCam.transform.localPosition = new Vector3(mainCam.transform.localPosition.x, mainCam.transform.localPosition.y, Mathf.Clamp(mainCam.transform.localPosition.z + (1) * Input.mouseScrollDelta.y, -100.0f, 0.0f));
        }
    }

    public void SetSettings(MenuSettings menuSettings)
    {
        demoCam = menuSettings.GetDemoCamera();
        demoCamSpeed = menuSettings.GetDemoCamSpeed();
        demoCamTime = menuSettings.GetDemoCamTime();

        if (demoCam) { ChangeView(FollowType.Orbital_View_Global); }
    }

    public void LateUpdate()
    {
        if (currentFollow == FollowType.Orbital_View_Global)
        {
            camArm.transform.rotation =
                Quaternion.Euler(-EntityListEntry.selected.disEntity.transform.rotation.x, -EntityListEntry.selected.disEntity.transform.rotation.y, -EntityListEntry.selected.disEntity.transform.rotation.z);
        }
    }

    private void OnDISEntityCreated(GameObject newEntity, EntityStatePdu pdu)
    {
        GameObject entryToAdd = Instantiate(EntitySelectionPrefab, scrollContent.transform);
        EntityListEntry entityEntry = entryToAdd.GetComponent<EntityListEntry>();
        entityEntry.SetUp(newEntity, pdu, this.GetComponent<RectTransform>());
        entries.Add(entityEntry);
        entries.Sort();

        foreach (EntityListEntry entry in entries)
        {
            entry.transform.SetSiblingIndex(entries.IndexOf(entry));
        }

        if (demoCam && EntityListEntry.selected == null)
        {
            EntityListEntry.selected = entityEntry;
        }
    }

    private void OnDISEntityDestroyed(GameObject entity, EDestroyCode destroyCode)
    {
        if (EntityListEntry.selected != null && EntityListEntry.selected.disEntity.Equals(entity))
        {
            ChangeView(FollowType.Free_Flying);
            GameObject.Destroy(EntityListEntry.selected);
            EntityListEntry.selected = null;

        }

        foreach (EntityListEntry entry in entries)
        {
            if (entry.disEntity.Equals(entity))
            {
                GameObject.Destroy(entry.gameObject);
                entries.Remove(entry);
                return;
            }
        }

    }

    

    public void ChangeView(FollowType option)
    {
        followTypeDropDown.value = (int)option;

        if (demoCam)
        {
            option = FollowType.Orbital_View_Global;
            demoTimeElapsed = 0;
        }

        currentFollow = option;
        if (camArm == null) { SetUpCameraArm(); }
        foreach (EntityListEntry entry in entries) { entry.SetColor(); }

        switch (option)
        {
            case FollowType.Free_Flying:
                mainCam.transform.parent = CesiumGeoreference.transform;
                mainCam.transform.position = EntityListEntry.selected.location.position;
                mainCam.transform.rotation = new Quaternion(0, 0, 0, 0);

                camArm.transform.parent = CesiumGeoreference.transform;
                camArm.transform.localPosition = Vector3.zero;
                camArm.transform.localRotation = new Quaternion(0, 0, 0, 0);

                camRotator.transform.localPosition = Vector3.zero;
                camRotator.transform.localRotation = new Quaternion(0, 0, 0, 0);

                EnableMainCam();
                break;

            case FollowType.Direct_View:
                if (EntityListEntry.selected == null) { return; }
                DisableMainCam();
                mainCam.transform.parent = EntityListEntry.selected.disEntity.transform;
                mainCam.transform.localPosition = EntityListEntry.selected.directView.localPosition;
                mainCam.transform.rotation = EntityListEntry.selected.directView.rotation;
                break;

            case FollowType.Orbital_View_Local:
                if (EntityListEntry.selected == null) { return; }
                DisableMainCam();

                camArm.transform.parent = EntityListEntry.selected.disEntity.transform;
                camArm.transform.localPosition = Vector3.zero;
                camArm.transform.localRotation = new Quaternion(0, 0, 0, 0);

                camRotator.transform.localPosition = Vector3.zero;
                camRotator.transform.localRotation = new Quaternion(0, 0, 0, 0);

                mainCam.transform.parent = camRotator.transform;
                mainCam.transform.localPosition = ScaleUnityTransform(EntityListEntry.selected.orbitalView.localPosition, EntityListEntry.selected.disEntity.transform.localScale);
                mainCam.transform.rotation = new Quaternion(0, 0, 0, 0);
                break;

            case FollowType.Orbital_View_Global:
                if (EntityListEntry.selected == null) { return; }
                DisableMainCam();

                camArm.transform.parent = EntityListEntry.selected.disEntity.transform;
                camArm.transform.localPosition = Vector3.zero;
                camArm.transform.localRotation = new Quaternion(0, 0, 0, 0);

                camRotator.transform.localPosition = Vector3.zero;
                camRotator.transform.localRotation = new Quaternion(0, 0, 0, 0);

                mainCam.transform.parent = camRotator.transform;
                mainCam.transform.localPosition = ScaleUnityTransform(EntityListEntry.selected.orbitalView.localPosition, EntityListEntry.selected.disEntity.transform.localScale);
                mainCam.transform.rotation = new Quaternion(0, 0, 0, 0);
                break;

            default:
                Debug.LogError("ERROR: Invalid CameraSelection");
                break;
        }
    }



    public void ChangeSelection()
    {
        ChangeView((FollowType)followTypeDropDown.value);
    }

    public void ChangeFollowOption(int value)
    {
        ChangeView((FollowType)value);
    }

    private void DisableMainCam()
    {
        mainCam.GetComponent<FreeFlyCamera>().enabled = false;
        mainCam.GetComponent<CesiumOriginShift>().enabled = false;
        mainCam.GetComponent<CesiumGlobeAnchor>().enabled = false;
    }

    private void EnableMainCam()
    {
        mainCam.GetComponent<FreeFlyCamera>().enabled = true;
        mainCam.GetComponent<CesiumOriginShift>().enabled = true;
        mainCam.GetComponent<CesiumGlobeAnchor>().enabled = true;
    }

    private void SetUpCameraArm()
    {
        if (camArm == null)
        {
            camArm = new GameObject();
            camArm.name = "CameraArm";
            camArm.transform.parent = CesiumGeoreference.transform;
        }

        if (camRotator == null)
        {
            camRotator = new GameObject();
            camRotator.name = "ArmBase";
            camRotator.transform.parent = camArm.transform;
        }
    }

    public bool GetDemoCam() { return demoCam; }
    public float GetDemoCamSpeed() { return demoCamSpeed; }

    public List<EntityListEntry> GetEntries() { return entries; }
    private Vector3 ScaleUnityTransform(Vector3 vec, Vector3 scale) { return new Vector3(vec.x * scale.x, vec.y * scale.y, vec.z * scale.z); }

}
