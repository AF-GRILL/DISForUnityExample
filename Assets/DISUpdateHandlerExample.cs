using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenDis.Dis1998;
using CesiumForUnity;
using GRILLDIS;

public class DISUpdateHandlerExample : MonoBehaviour
{
    private DISReceiveComponent disComponent;
    private CesiumGlobeAnchor cesiumGlobeAnchorScript;
    private TrailRenderer trailRenderer;

    private void Start()
    {
        disComponent = GetComponent<DISReceiveComponent>();
        cesiumGlobeAnchorScript = GetComponent<CesiumGlobeAnchor>();
    }

    public void HandleDeadReckoningUpdate(EntityStatePdu DeadReckonedPDUIn)
    {
        if (cesiumGlobeAnchorScript)
        {
            FLatLonAlt llh = Conversions.CalculateLatLonHeightFromEcefXYZ(DeadReckonedPDUIn.EntityLocation);

            Orientation entityOrientation = DeadReckonedPDUIn.EntityOrientation;

            FHeadingPitchRoll hpr = Conversions.CalculateHeadingPitchRollDegreesFromPsiThetaPhiRadiansAtLatLon(new FPsiThetaPhi(entityOrientation), llh.Latitude, llh.Longitude);

            cesiumGlobeAnchorScript.SetPositionEarthCenteredEarthFixed(DeadReckonedPDUIn.EntityLocation.X, DeadReckonedPDUIn.EntityLocation.Y, DeadReckonedPDUIn.EntityLocation.Z);

            //Pitch and Roll are backwards in Unity vs DIS, so negate
            transform.rotation = Quaternion.Euler(-hpr.Pitch, hpr.Heading, -hpr.Roll);
        }
    }

    public void HandleEntityStateProcessed(EntityStatePdu EntityStatePDUIn)
    {
        if (cesiumGlobeAnchorScript)
        {
            FLatLonAlt llh = Conversions.CalculateLatLonHeightFromEcefXYZ(EntityStatePDUIn.EntityLocation);

            Orientation entityOrientation = EntityStatePDUIn.EntityOrientation;

            FHeadingPitchRoll hpr = Conversions.CalculateHeadingPitchRollDegreesFromPsiThetaPhiRadiansAtLatLon(new FPsiThetaPhi(entityOrientation), llh.Latitude, llh.Longitude);

            cesiumGlobeAnchorScript.SetPositionEarthCenteredEarthFixed(EntityStatePDUIn.EntityLocation.X, EntityStatePDUIn.EntityLocation.Y, EntityStatePDUIn.EntityLocation.Z);

            //Pitch and Roll are backwards in Unity vs DIS, so negate
            transform.rotation = Quaternion.Euler(-hpr.Pitch, hpr.Heading, -hpr.Roll);
        }
    }

    public void HandleFireProcessed(FirePdu FirePDUIn)
    {
        Debug.Log("Fire PDU Received. Finalize any logic that needs performed.");
    }

    public void HandleDetonationProcessed(DetonationPdu DetonationPDUIn)
    {
        if (disComponent)
        {
            Debug.Log("Detonation PDU Received. Destroying Entity: " + PDUUtil.getMarkingAsString(disComponent.MostRecentEntityStatePDU));
            
        }

        Destroy(gameObject);
    }

    public void HandleRemoveEntityProcessed(RemoveEntityPdu RemoveEntityPDUIn)
    {
        if (disComponent)
        {
            Debug.Log("Remove Entity PDU Received. Removing Entity: " + PDUUtil.getMarkingAsString(disComponent.MostRecentEntityStatePDU));
            
        }

        Destroy(gameObject);
    }

    public void HandleEntityStateUpdateProcessed(EntityStateUpdatePdu EntityStateUpdatePDUIn)
    {

    }
}
