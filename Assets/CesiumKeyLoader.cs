using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CesiumForUnity;
using System;

public class CesiumIonData
{
    public string WorldTerrainCesiumURL;
    public string WorldTerrainCesiumIonToken;
    public long WorldTerrainCesiumIonAssetID;
    public string RasterOverlayCesiumURL;
    public string RasterOverlayCesiumIonToken;
    public long RasterOverlayCesiumIonAssetID;
}

public class CesiumKeyLoader : MonoBehaviour
{
    private Cesium3DTileset cesium3DTilesetScript;
    private CesiumIonRasterOverlay cesiumRasterOverlayScript;

    private CesiumIonData cesiumIonData;

    // Start is called before the first frame update
    void Start()
    {
        if (ReadCesiumIonInfoFile())
        {
            cesium3DTilesetScript = GetComponentInChildren<Cesium3DTileset>();
            cesiumRasterOverlayScript = GetComponentInChildren<CesiumIonRasterOverlay>();

            UpdateCesiumTileset();
            UpdateCesiumRasterOverlay();
        }
    }

    bool ReadCesiumIonInfoFile()
    {
        TextAsset CesiumIonInfoFile = Resources.Load("CesiumIonInfo") as TextAsset;

        string json = CesiumIonInfoFile.text;

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("The CesiumIonInfo.txt file in the Resources folder is empty. Refer to project README on how to properly fill it out.");

            return false;
        }

        try
        {
            cesiumIonData = JsonUtility.FromJson<CesiumIonData>(json);

            return true;
        }
        catch (ArgumentException ex)
        {
            Debug.LogError("Issue with the CesiumIonInfo.txt file in the Resources folder. Refer to project README on how to properly fill it out.\nError: " + ex.Message);

            return false;
        }
    }

    void UpdateCesiumTileset()
    {
        cesium3DTilesetScript.tilesetSource = (string.IsNullOrEmpty(cesiumIonData.WorldTerrainCesiumURL)) ? CesiumDataSource.FromCesiumIon : CesiumDataSource.FromUrl;

        switch (cesium3DTilesetScript.tilesetSource)
        {
            case CesiumDataSource.FromCesiumIon:
                cesium3DTilesetScript.ionAssetID = cesiumIonData.WorldTerrainCesiumIonAssetID;
                cesium3DTilesetScript.ionAccessToken = cesiumIonData.WorldTerrainCesiumIonToken;
                break;
            case CesiumDataSource.FromUrl:
                cesium3DTilesetScript.url = cesiumIonData.WorldTerrainCesiumURL;
                break;
            default:
                break;
        }

        cesium3DTilesetScript.RecreateTileset();
    }

    void UpdateCesiumRasterOverlay()
    {
        if (string.IsNullOrEmpty(cesiumIonData.RasterOverlayCesiumURL))
        {
            cesiumRasterOverlayScript.ionAccessToken = cesiumIonData.RasterOverlayCesiumIonToken;
            cesiumRasterOverlayScript.ionAssetID = cesiumIonData.RasterOverlayCesiumIonAssetID;

            cesium3DTilesetScript.RecreateTileset();
        }
    }
}
