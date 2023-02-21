using OpenDis.Dis1998;
using System;
using UnityEngine;
using UnityEngine.UI;
using GRILLDIS;
using CesiumForUnity;
public class EntityInfo : MonoBehaviour
{
    public Text entityName;
    public Text entityForce;
    public Text entityID;
    public Text entityLocation;
    public Text entityAltitude;
    public Text entityHeading;
    public Text entitySpeed;
    public Text entityEnumeration;
    public CameraController controller;

    //The degree symbol as a char
    public const char DEGREE_CHAR = (char)176;

    private GameObject entity;
    private DISReceiveComponent disReceiveComponent;
    private CesiumGlobeAnchor entityGlobeAnchor;


    public void Start()
    {
        controller = GameObject.FindObjectOfType<CameraController>();
    }

    void Update()
    {
        if (this.gameObject.activeSelf && entity != null)
        {
            entityName.text = disReceiveComponent.EntityMarking;
            entityForce.text =  Enum.GetName(typeof(EForceID), disReceiveComponent.EntityForceID);
            entityID.text = RemovePara(disReceiveComponent.CurrentEntityID.ToString());
            entityLocation.text = FormatLocation(entityGlobeAnchor.latitude, entityGlobeAnchor.longitude);
            entityAltitude.text = FormatAltitude(entityGlobeAnchor.height);
            entityHeading.text = FormatHeading(entity.transform.rotation.eulerAngles.y);
            
            entitySpeed.text = Math.Round(disReceiveComponent.MostRecentDeadReckoningPDU.EntityLinearVelocity.CalculateLength(), 0).ToString() + " kn";
            entityEnumeration.text = RemovePara(disReceiveComponent.CurrentEntityType.ToString());
        }
    }


    public void Open(GameObject newEntity)
    {
        entity = newEntity;
        disReceiveComponent = entity.GetComponent<DISReceiveComponent>();
        entityGlobeAnchor = entity.GetComponent<CesiumGlobeAnchor>();

        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        entity = null;
        disReceiveComponent = null;
        this.gameObject.SetActive(false);
    }

    public void DirectView()
    {
        foreach (EntityListEntry ele in controller.GetEntries())
        {
            if (ele.disEntity.Equals(entity))
            {
                EntityListEntry.selected = ele;
                controller.ChangeView(FollowType.Direct_View);
                return;
            }
        }
    }


    public static string FormatLocation(double latitude, double longitude)
    {        
        return Math.Round(latitude, 3).ToString("F3") + DEGREE_CHAR + ", " + Math.Round(longitude, 3).ToString("F3") + DEGREE_CHAR;
    }

    public static string FormatAltitude(double height)
    {
        return Math.Round(height * 0.003281, 1).ToString("F1") + " Kft";
    }

    public static string FormatHeading(float heading)
    {
        heading = Mathf.Round(heading + 360) % 360;
        while (heading < 0) { heading = Mathf.Round(heading + 360) % 360; }
        return Math.Round(heading, 0).ToString() + DEGREE_CHAR;
    }

    public static string RemovePara(string toTrim)
    {
        return toTrim.Replace("(", "").Replace(")", "");
    }


}
