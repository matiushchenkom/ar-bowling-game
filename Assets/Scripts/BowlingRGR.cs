using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Main controller for the AR Bowling project. 
/// Handles pin placement, ball physics, and scoring logic.
/// </summary>
public class BowlingRGR : MonoBehaviour {
    [Header("Prefab Settings")]
    [Tooltip("The actual pins that will be spawned in AR")]
    public GameObject realPinsPrefab;
    [Tooltip("Visual guide for valid placement")]
    public GameObject blueGhostPrefab;
    [Tooltip("Visual guide for invalid placement")]
    public GameObject redGhostPrefab;

    [Header("Interaction Objects")]
    [Tooltip("The bowling ball prefab with Rigidbody")]
    public GameObject ballPrefab;
    [Tooltip("The position where the ball appears before throwing")]
    public Transform handPos;

    [Header("AR Systems")]
    public ARRaycastManager rayManager;

    [Header("User Interface")]
    public TextMeshProUGUI scoreText;
    public GameObject congratsUI;
    public GameObject strikeUI;
    
    private int score = 0;
    private bool isFirstThrow = false;
    private bool didThrowInThisSet = false; 

    private GameObject currentGhost;
    private GameObject currentBall;
    private GameObject spawnedPinsGroup;
    private bool isPlacingMode = false;

    private Vector3 lastMousePosition;
    private float swingForce;

    void Start() {
        if (congratsUI != null) congratsUI.SetActive(false);
        if (strikeUI != null) strikeUI.SetActive(false);
        UpdateScoreUI();
    }

    /// <summary>
    /// Enters the mode to place pins on a detected AR plane.
    /// </summary>
    public void EnablePlacementMode() {
        if (congratsUI != null) congratsUI.SetActive(false);
        if (strikeUI != null) strikeUI.SetActive(false);
        
        isPlacingMode = true;
        score = 0; 
        isFirstThrow = true;      
        didThrowInThisSet = false; 
        UpdateScoreUI();
        
        if (spawnedPinsGroup != null) {
            Destroy(spawnedPinsGroup);
            spawnedPinsGroup = null; 
        }
        
        CancelInvoke("DisableFirstThrowFlag");
    }

    /// <summary>
    /// Spawns a new bowling ball in the player's "hand" position.
    /// </summary>
    public void SpawnBallInHand() {
        if (didThrowInThisSet) {
            isFirstThrow = false;
        }

        GameObject oldBall = GameObject.FindGameObjectWithTag("Ball");
        if (oldBall != null) Destroy(oldBall);
        if (currentBall != null) Destroy(currentBall);

        currentBall = Instantiate(ballPrefab, handPos);
        currentBall.transform.localPosition = Vector3.zero;
        currentBall.transform.localRotation = Quaternion.identity;
        currentBall.tag = "Ball"; 
        
        Rigidbody rb = currentBall.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true; 
    }

    void UpdateScoreUI() {
        if (scoreText != null) scoreText.text = "PINS DOWN: " + score;
    }

    void Update() {
        if (isPlacingMode) HandlePlacement();

        if (currentBall != null && currentBall.transform.parent == handPos) {
            CalculateSwing();
            if (Input.GetMouseButtonDown(0)) {
                ThrowBall();
            }
        }

        if (!isPlacingMode && spawnedPinsGroup != null) {
            CheckPinsStatus();
        }
    }

    /// <summary>
    /// Evaluates pins to see if they have fallen based on their orientation.
    /// </summary>
    void CheckPinsStatus() {
        int fallenCount = 0;
        foreach (Transform pin in spawnedPinsGroup.transform) {
            if (Vector3.Dot(pin.forward, Vector3.up) < 0.5f) {
                fallenCount++;
            }
        }

        if (fallenCount != score) {
            score = fallenCount;
            UpdateScoreUI();
        }

        if (score == 6) {
            if (congratsUI != null && !congratsUI.activeSelf) congratsUI.SetActive(true);
            
            if (isFirstThrow && strikeUI != null && !strikeUI.activeSelf) {
                strikeUI.SetActive(true);
            }
        } else {
            if (congratsUI != null && congratsUI.activeSelf) congratsUI.SetActive(false);
            if (strikeUI != null && strikeUI.activeSelf) strikeUI.SetActive(false);
        }
    }

    void HandlePlacement() {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

        if (rayManager.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon)) {
            SwitchGhost(blueGhostPrefab, hits[0].pose.position);

            if (Input.GetMouseButtonDown(0)) { 
                Vector3 spawnPos = hits[0].pose.position + Vector3.up * 0.05f;
                spawnedPinsGroup = Instantiate(realPinsPrefab, spawnPos, currentGhost.transform.rotation);
                spawnedPinsGroup.AddComponent<ARAnchor>(); 
                
                isPlacingMode = false; 
                if (currentGhost != null) Destroy(currentGhost);
            }
        } else {
            Vector3 farPos = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;
            SwitchGhost(redGhostPrefab, farPos);
        }

        if (currentGhost != null) {
            currentGhost.transform.LookAt(new Vector3(Camera.main.transform.position.x, currentGhost.transform.position.y, Camera.main.transform.position.z));
            currentGhost.transform.Rotate(0, 180, 0); 
        }
    }

    void SwitchGhost(GameObject prefab, Vector3 position) {
        if (currentGhost != null && currentGhost.name != prefab.name + "(Clone)") Destroy(currentGhost);
        if (currentGhost == null) currentGhost = Instantiate(prefab);
        currentGhost.transform.position = position;
    }

    void CalculateSwing() {
        Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
        swingForce = mouseDelta.magnitude;
        lastMousePosition = Input.mousePosition;
        
        if (currentBall != null) {
            currentBall.transform.localPosition = Vector3.Lerp(currentBall.transform.localPosition, Vector3.zero, Time.deltaTime * 5f);
        }
    }

    /// <summary>
    /// Applies force to the ball based on the calculated mouse/touch swing.
    /// </summary>
    void ThrowBall() {
        currentBall.transform.SetParent(null);
        Rigidbody rb = currentBall.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.isKinematic = false; 
            float finalForce = Mathf.Clamp(500f + (swingForce * 8f), 400f, 1600f);
            rb.AddForce(Camera.main.transform.forward * finalForce);
        }
        currentBall = null;
        didThrowInThisSet = true; 

        Invoke("DisableFirstThrowFlag", 4.0f); 
    }

    void DisableFirstThrowFlag() {
        isFirstThrow = false;
    }
}