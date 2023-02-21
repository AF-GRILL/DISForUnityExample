using UnityEngine;
using UnityEngine.UI;

using System;

using CesiumForUnity;
using GRILLDIS;
using UnityEngine.InputSystem.Composites;

public class DISEntityLabel : MonoBehaviour
{
    public Text entityName;
    public Text altitude;
    public Text distance;

    private GameObject entity;
    private DISReceiveComponent receiveComponent;
    private DISEntitySceneComponent sceneComponent;
    private CesiumGlobeAnchor entityGlobeAnchor;
    private ButtonManager buttonManager;
    private bool inRange = true;
    private bool showLabels = true;

    public void SetUp(GameObject entity, string textToSet)
    {
        this.entity = entity;
        this.entityName.text = textToSet;
        receiveComponent = entity.GetComponent<DISReceiveComponent>();
        sceneComponent = entity.GetComponent<DISEntitySceneComponent>();
        entityGlobeAnchor = entity.GetComponent<CesiumGlobeAnchor>();
        buttonManager = GameObject.FindObjectOfType<ButtonManager>();
        SetSettings(buttonManager.menuSettings);
        buttonManager.onDISOptionsChangedEvent.AddListener(SetSettings);

        switch (sceneComponent.forceID)
        {
            case 1:
                entityName.color = Color.blue;
                altitude.color = Color.blue;
                distance.color = Color.blue;
                break;
            case 2:
                entityName.color = Color.red;
                altitude.color = Color.red;
                distance.color = Color.red;
                break;
            default:
                entityName.color = Color.white;
                altitude.color = Color.white;
                distance.color = Color.white;
                break;
        }
    }

    public void SetSettings(MenuSettings settings)
    {
        showLabels = settings.GetShowLabels();
    }

    public void OpenInfoPanel()
    {
        buttonManager.OpenEntityInfo(entity);
    }

    void Update()
    {
        if (Camera.main == null) { return; }
        float scalar = Vector3.Distance(this.transform.position, Camera.main.transform.position);

        inRange = (scalar > 75 && scalar < 100000);

        if (inRange && showLabels)
        {
            this.transform.GetChild(0).gameObject.SetActive(true);
            this.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
            float x = scalar * 0.0016f / transform.parent.localScale.x;
            float y = scalar * 0.0016f / transform.parent.localScale.y;
            float z = scalar * 0.0016f / transform.parent.localScale.z;
            this.transform.localScale = new Vector3(x, y ,z);
            this.transform.localPosition = new Vector3(0, 100f + 30f * y, 0);
            
            float alt = entity.transform.position.y * 0.003281f;
            
            //Only show the kilofeet altitude if the entity is an air entity
            if (receiveComponent.CurrentEntityType.domain == 2)
            {
                altitude.text = EntityInfo.FormatAltitude(entityGlobeAnchor.height);
            }
            else
            {
                altitude.text = "";
            }
            
            distance.text = Mathf.CeilToInt(Vector3.Distance(entity.transform.position, Camera.main.transform.position) / 1000f) + " KM";
            

        }else
        {
            this.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    
}
