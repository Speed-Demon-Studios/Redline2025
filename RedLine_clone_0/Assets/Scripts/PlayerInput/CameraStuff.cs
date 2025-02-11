using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStuff : MonoBehaviour
{
    private ShipsControls m_shipControls;

    [Header("Player Camera Stuff")]
    public List<int> playerLayers = new();
    public List<LayerMask> ignoreLayers = new();
    public Camera cam;
    [SerializeField] private CinemachineVirtualCamera m_virtualCam;

    [Header("Camera Values")]
    public float lerpTime;
    public float minFOV;
    public float maxFOV;
    private float m_currentFOV;
    private float m_desiredFOV;

    public void ActivateVirtualCam() { m_virtualCam.gameObject.SetActive(true); }
    public void DeActivateVirtualCam() { m_virtualCam.gameObject.SetActive(false); }

    // Start is called before the first frame update
    public void InitializCamera()
    {
        m_shipControls = GetComponent<ShipsControls>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gManager.raceStarted == true && GameManager.gManager.raceFinished == false) // if the race has started and not finished
        {
            CalculateFOV(); // calculate the FOV for the camera
        }
    }

    public void SwitchCameraLayers(int playerNumber)
    {
        if (m_virtualCam != null && playerLayers.Count > 0)
            m_virtualCam.gameObject.layer = playerLayers[playerNumber];
        if (cam != null && playerLayers.Count > 0)
            cam.cullingMask = ignoreLayers[playerNumber];
    }

    private void CalculateFOV()
    {
        // calculating how fast the ships going compaired to the top speed as a percentage                                  
        float speedPercentage = m_shipControls.RB.velocity.magnitude / m_shipControls.VariantObject.DefaultMaxSpeed;
        // if the ship is moving slightly
        if (speedPercentage > 0.001)
        {
            m_desiredFOV = ((maxFOV - minFOV) * speedPercentage) + minFOV; // calculate the desiredFOV                      
        }
        else // if its not moving
        {
            m_desiredFOV = minFOV; // then the FOV is now the min it can go
        }
        //m_desiredPOV = Mathf.Lerp(minPOV, maxPOV, speedPercentage);

        m_currentFOV = Mathf.Lerp(m_currentFOV, m_desiredFOV, lerpTime); // lerp to the desiredFOV so that its smooth
        m_currentFOV = Mathf.Clamp(m_currentFOV, 0, maxFOV + 10);
        if (m_virtualCam != null)
            m_virtualCam.m_Lens.FieldOfView = m_currentFOV; // set the FOV to the currentFOV                                
    }
}
