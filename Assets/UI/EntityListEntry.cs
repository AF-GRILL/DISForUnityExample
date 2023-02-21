using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using OpenDis.Dis1998;


public class EntityListEntry : MonoBehaviour, IComparable<EntityListEntry>
{
    public Text objName;

    public GameObject disEntity;
    public DISEntitySceneComponent sceneComponent;
    public static EntityListEntry selected;
    public static CameraController cameraController;
    public Transform directView, orbitalView, location;
    public Image image;

    private Color defaultColor =  new Color(255f / 255f, 255f / 255f, 255f / 255f);
    private Color selectedColor = new Color(238f / 255f, 204f / 255f, 068f / 255f);

    private RectTransform mainCanvas;
    private RectTransform rectTransform;


    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    private void OnEnable()
    {
        UpdateRect();
        
    }

    private void OnRectTransformDimensionsChange()
    {
        UpdateRect();
    }

    private void UpdateRect()
    {
        if (rectTransform == null) { rectTransform = GetComponent<RectTransform>(); }
        if (mainCanvas == null) { mainCanvas = GameObject.Find("UI").GetComponent<RectTransform>(); }
        rectTransform.sizeDelta = new Vector2(mainCanvas.sizeDelta.x, mainCanvas.sizeDelta.y * 0.05f);
    }


    public void SetUp(GameObject entity , EntityStatePdu pdu, RectTransform canvasTransform)
    {
        if (cameraController == null) { cameraController = gameObject.transform.GetComponentInParent<CameraController>(); }

        disEntity = entity;
        directView = entity.GetComponent<DISEntitySceneComponent>().directView;
        orbitalView = entity.GetComponent<DISEntitySceneComponent>().orbitalView;
        sceneComponent = entity.GetComponent<DISEntitySceneComponent>();
        location = entity.transform;
        mainCanvas = canvasTransform;
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        sceneComponent.SetUp(pdu);
        objName.text = sceneComponent.entityMarking;
        switch (sceneComponent.forceID)
        {
            case 1:
                objName.color = Color.blue;
                break;
            case 2:
                objName.color = Color.red;
                break;
            default:
                objName.color = Color.white;
                break;
        }

    }

    public void Select()
    {
        if(selected != null)
        {
            image.color = defaultColor;
        }

        selected = this;
        Dropdown dropDownViewMode = cameraController.followTypeDropDown;
        string dropDownViewModeSelectedOption = dropDownViewMode.options[dropDownViewMode.value].text;
        dropDownViewModeSelectedOption = dropDownViewModeSelectedOption.Replace(" ", "_");

        image.color = selectedColor;

        cameraController.ChangeSelection();
        
        
    }

    public void SetColor()
    {
        if (image == null) { return; }
        if (selected != null && selected.Equals(this)) { image.color = selectedColor; }
        else { image.color = defaultColor; }
    }

    public int CompareTo(EntityListEntry other)
    {
        return sceneComponent.entityMarking.CompareTo(other.sceneComponent.entityMarking);
    }
}
