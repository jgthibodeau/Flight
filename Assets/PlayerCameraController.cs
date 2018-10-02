using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor.Presets;

public class PlayerCameraController : MonoBehaviour
{
    [System.Serializable]
    public class CameraConfig
    {
        //public Preset controllerPreset;
        //public Preset followPreset;
        public GameObject cameraPrefab;
        public Transform target;
        public GameObject backflapCameraPrefab;
        public Transform backflapTarget;
    }

    private Player player;
    private Rigidbody playerRb;
    private GlideV2 playerGlideScript;

    public FreeFormCameraTarget freeFormCameraTarget;

    public ThirdPersonCamera.Follow mainFollowCamera;

    public ThirdPersonCamera.CameraController cameraController;
    public ThirdPersonCamera.Follow follow;

    public CameraConfig mainConfig;
    public CameraConfig headConfig;
    public CameraConfig firstPersonConfig;

    public bool useFpsCamera = false;

    void Start () {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerRb = player.GetComponent<Rigidbody>();
        playerGlideScript = player.GetComponent<GlideV2>();
        freeFormCameraTarget.DisallowFreeForm();
        follow.freeFormCameraTarget = freeFormCameraTarget;
        follow.cc = cameraController;

        SetCameraConfig(mainConfig);
    }

    void Update()
    {
        //freeFormCameraTarget.DisallowFreeForm();
        if (Util.GetButtonDown("Toggle Camera"))
        {
            useFpsCamera = !useFpsCamera;
        }

        freeFormCameraTarget.AllowFreeForm();
        if (useFpsCamera)
        {
            SetCameraConfig(firstPersonConfig);
        }
        else
        {
            if (player.isFlaming && playerRb.velocity.magnitude < 5f)
            {
                SetCameraConfig(headConfig);
                freeFormCameraTarget.DisallowFreeForm();
            }
            else
            {
                SetCameraConfig(mainConfig);
            }
        }
    }

	public void SetMainCameraTight (bool tight) {
		if (tight) {
			mainFollowCamera.SetFollowType (ThirdPersonCamera.FOLLOW_TYPE.TIGHT);
		} else {
			mainFollowCamera.UnsetFollowType ();
		}
	}

    public void SetCameraConfig(CameraConfig cc)
    {
        //    cc.controllerPreset.ApplyTo(cameraController);
        //    cc.followPreset.ApplyTo(follow);
        
        if (playerGlideScript.isBackFlapping)
        {
            cameraController.target = cc.backflapTarget;
            cameraController.CloneFrom(cc.backflapCameraPrefab.GetComponent<ThirdPersonCamera.CameraController>());
            follow.CloneFrom(cc.backflapCameraPrefab.GetComponent<ThirdPersonCamera.Follow>());
        }
        else
        {
            cameraController.target = cc.target;
            cameraController.CloneFrom(cc.cameraPrefab.GetComponent<ThirdPersonCamera.CameraController>());
            follow.CloneFrom(cc.cameraPrefab.GetComponent<ThirdPersonCamera.Follow>());
        }
    }
}
