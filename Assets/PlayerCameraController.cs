﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour {
	public ThirdPersonCamera.Follow mainFollowCamera;
	public List<MonoBehaviour> mainCameraScripts;
	public List<MonoBehaviour> headCameraScripts;

	public bool useMainCamera = true;

	void Start () {
		EnableMainCamera ();
	}

	public void SetMainCameraTight (bool tight) {
		if (tight) {
			mainFollowCamera.SetFollowType (ThirdPersonCamera.FOLLOW_TYPE.TIGHT);
		} else {
			mainFollowCamera.UnsetFollowType ();
		}
	}

	public void ToggleCamera() {
		if (useMainCamera) {
			EnableHeadCamera ();
		} else {
			EnableMainCamera ();
		}
	}

	public void EnableMainCamera() {
		useMainCamera = true;
		foreach (MonoBehaviour script in mainCameraScripts) {
			script.enabled = true;
		}
		DisableHeadCamera ();
	}
	public void EnableHeadCamera() {
		useMainCamera = false;
		foreach (MonoBehaviour script in headCameraScripts) {
			script.enabled = true;
		}
		DisableMainCamera ();
	}

	private void DisableMainCamera() {
		foreach (MonoBehaviour script in mainCameraScripts) {
			script.enabled = false;
		}
	}
	private void DisableHeadCamera() {
		foreach (MonoBehaviour script in headCameraScripts) {
			script.enabled = false;
		}
	}
}