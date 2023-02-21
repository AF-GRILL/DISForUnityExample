using UnityEngine;
using OpenDis.Dis1998;
using GRILLDIS;
using CesiumForUnity;

public class DISEntitySceneComponent : MonoBehaviour
{
    public Transform directView;
    public Transform orbitalView;
    public GameObject labelPrefab;

    [Header("")]
    public string entityMarking;
    public int forceID;
    public string disEnumeration;
    public float trailWidthMultiplier;

    [Header("Appearance")]
    public bool isEntityRibbonTrailEnabled = true;
    public float ribbonWidth;

    private DISEntityLabel label;
    private TrailRenderer ribbonTrailRenderer;
    private ButtonManager buttonManager;
    public void SetUp(EntityStatePdu pdu)
    {
        
        entityMarking = PDUUtil.getEntityStatePDUMarkingSAE(pdu);
        forceID = (int)pdu.ForceId;
        disEnumeration = pdu.EntityID.ToString();
        GameObject obj = Instantiate(labelPrefab, this.transform);
        label = obj.GetComponent<DISEntityLabel>();
        label.SetUp(this.gameObject, entityMarking);


        GameObject ribbonTrailGameObject = this.transform.Find("RibbonTrail").gameObject;
        ribbonTrailRenderer = ribbonTrailGameObject.GetComponent<TrailRenderer>();
        ribbonTrailRenderer.startWidth = ribbonWidth;
        ribbonTrailRenderer.endWidth = 0.0f;

        switch (forceID)
        {
            case 1:
                setRibbonTrailRendererColor(ribbonTrailRenderer, Color.blue);
                break;
            case 2:
                setRibbonTrailRendererColor(ribbonTrailRenderer, Color.red);
                break;
            default:
                setRibbonTrailRendererColor(ribbonTrailRenderer, Color.white);
                break;
        }

        int menuRibbonTrails = PlayerPrefs.GetInt("ShowTrails");
        bool isMenuRibbonTrailEnabled = (menuRibbonTrails == 1);

        setRibbonTrailVisibiliyFromPreferences(ribbonTrailRenderer, isMenuRibbonTrailEnabled, isEntityRibbonTrailEnabled);
        buttonManager = GameObject.FindObjectOfType<ButtonManager>();
        buttonManager.onDISOptionsChangedEvent.AddListener(onMenuOptionsUpdated);
    }

    private void setRibbonTrailRendererColor(TrailRenderer ribbonTrailRenderer, Color color) {
        ribbonTrailRenderer.startColor = color;
        ribbonTrailRenderer.endColor = color;
        ribbonTrailRenderer.material.SetColor("_EmissionColor", color);
        ribbonTrailRenderer.material.SetColor("_BaseColor", color);
    }

    public void onMenuOptionsUpdated(MenuSettings menuSettings)
    {
        setRibbonTrailVisibiliyFromPreferences(ribbonTrailRenderer, menuSettings.GetShowTrails(), isEntityRibbonTrailEnabled);
    }

    private void setRibbonTrailVisibiliyFromPreferences(TrailRenderer ribbonTrail, bool isMenuRibbonTrailEnabled, bool isEntityRibbonTrailEnabled)
    {

        //If the ribbon trail is set off for the entity, don't show it!
        if (!isEntityRibbonTrailEnabled)
        {
            ribbonTrail.emitting = false;
            return;
        }
        //If the ribbon trail is enabled by the user, then toggle it on and off
        ribbonTrail.emitting = isMenuRibbonTrailEnabled;
        if (!isMenuRibbonTrailEnabled)
        {
            //Remove any existing trails floating out there
            ribbonTrail.Clear();
        }
    }
}
