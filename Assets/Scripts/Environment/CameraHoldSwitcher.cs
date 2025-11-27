using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

// Bascule instantanee entre deux CinemachineCamera (ex: Camera Follow Player / RearCam) en appuyant sur R.
public class CameraHoldSwitcher : MonoBehaviour
{
    [Header("Cameras (drag tes vcams ici)")]
    public CinemachineCamera followCam;
    public CinemachineCamera rearCam;

    [Header("Priorites")]
    public int activePriority = 50;
    public int inactivePriority = 0;

    [Header("Auto-find par nom (optionnel)")]
    public string followCamName = "Camera Follow Player";
    public string rearCamName = "RearCam";

    bool usingRear;

    void Awake()
    {
        AutoAssignIfNeeded();
        ApplyState(false); // follow par defaut
    }

    void OnEnable()
    {
        ApplyState(false); // securite au re-enable
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.rKey.wasPressedThisFrame)
        {
            ApplyState(!usingRear);
        }
    }

    void AutoAssignIfNeeded()
    {
        if (followCam == null && !string.IsNullOrEmpty(followCamName))
        {
            var go = GameObject.Find(followCamName);
            if (go != null) followCam = go.GetComponent<CinemachineCamera>();
        }
        if (rearCam == null && !string.IsNullOrEmpty(rearCamName))
        {
            var go = GameObject.Find(rearCamName);
            if (go != null) rearCam = go.GetComponent<CinemachineCamera>();
        }
        Debug.Log($"[CameraHoldSwitcher] followCam={(followCam ? followCam.name : "null")}, rearCam={(rearCam ? rearCam.name : "null")}");
    }

    void ApplyState(bool rearActive)
    {
        usingRear = rearActive;
        if (followCam != null)
        {
            followCam.Priority.Value = rearActive ? inactivePriority : activePriority;
        }
        if (rearCam != null)
        {
            rearCam.Priority.Value = rearActive ? activePriority : inactivePriority;
        }
        Debug.Log($"[CameraHoldSwitcher] state={(rearActive ? "Rear" : "Follow")}, followPri={(followCam ? followCam.Priority.Value : -1)}, rearPri={(rearCam ? rearCam.Priority.Value : -1)}");
    }
}
