﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using System.Collections;
using TMPro;

namespace TeamUtility.IO.Examples
{
	[RequireComponent(typeof(Image))]
	public class RebindInput : MonoBehaviour, IPointerDownHandler 
	{
		public enum RebindType
		{
			Keyboard, KeyboardAxis, GamepadButton, GamepadAxis
		}

		[SerializeField]
		[FormerlySerializedAs("m_normalState")]
		private Sprite _normalState;

		[SerializeField]
		[FormerlySerializedAs("m_scanningState")]
		private Sprite _scanningState;

		[SerializeField]
		[FormerlySerializedAs("m_keyDescription")]
		public TextMeshProUGUI _keyDescription;

		[SerializeField]
		[FormerlySerializedAs("m_inputConfigName")]
		public string _inputConfigName;

		[SerializeField]
		[FormerlySerializedAs("m_axisConfigName")]
		public string _axisConfigName;

		[SerializeField]
		[FormerlySerializedAs("m_cancelButton")]
		private string _cancelButton;

		[SerializeField]
		[FormerlySerializedAs("m_timeout")]
		private float _timeout;

		[SerializeField]
		[FormerlySerializedAs("m_changePositiveKey")]
		public bool _changePositiveKey;

		[SerializeField]
		[FormerlySerializedAs("m_changeAltKey")]
		public bool _changeAltKey;

		[SerializeField]
		[FormerlySerializedAs("m_allowAnalogButton")]
		public bool _allowAnalogButton;

		[SerializeField]
		[FormerlySerializedAs("m_joystick")]
		[Range(0, AxisConfiguration.MaxJoysticks)]
		private int _joystick = 0;

		[SerializeField]
		[FormerlySerializedAs("m_rebindType")]
		public RebindType _rebindType;
		
		private AxisConfiguration _axisConfig;
		private Image _image;
		public static string[] _axisNames = new string[] { "X", "Y", "3rd", "4th", "5th", "6th", "7th", "8th", "9th", "10th" };
		public GamepadButtons gamepadButtons;

		public RebindInput(RebindType rebindType, string inputConfigName, string axisConfigName, bool allowAnalogButton, bool changePositiveKey) {
			_rebindType = rebindType;
			_inputConfigName = inputConfigName;
			_axisConfigName = axisConfigName;
			_allowAnalogButton = allowAnalogButton;
			_changePositiveKey = changePositiveKey;
		}

		private void Awake()
		{
			_image = GetComponent<Image>();
			_image.overrideSprite = _normalState;
			//InitializeAxisConfig();
			
			//	The axis config needs to be reinitialized because loading can invalidate
			//	the input configurations
			InputManager.Instance.Loaded += InitializeAxisConfig;
			InputManager.Instance.ConfigurationDirty += HandleConfigurationDirty;
		}
		
		private void OnDestroy()
		{
			if(InputManager.Instance != null)
			{
				InputManager.Instance.Loaded -= InitializeAxisConfig;
				InputManager.Instance.ConfigurationDirty -= HandleConfigurationDirty;
			}
		}
		
		public void InitializeAxisConfig()
        {
            //Debug.Log(this.transform.parent.parent.gameObject);
            //Debug.Log("Initializing axis config for " + _inputConfigName + " " + _axisConfigName);
            _axisConfig = InputManager.GetAxisConfiguration(_inputConfigName, _axisConfigName);
			if(_axisConfig != null)
			{
				if (_rebindType == RebindType.Keyboard) {
					if (_changePositiveKey) {
						if (_changeAltKey)
							_keyDescription.text = _axisConfig.altPositive == KeyCode.None ? "" : _axisConfig.altPositive.ToString ();
						else
							_keyDescription.text = _axisConfig.positive == KeyCode.None ? "" : _axisConfig.positive.ToString ();
					} else {
						if (_changeAltKey)
							_keyDescription.text = _axisConfig.altNegative == KeyCode.None ? "" : _axisConfig.altNegative.ToString ();
						else
							_keyDescription.text = _axisConfig.negative == KeyCode.None ? "" : _axisConfig.negative.ToString ();
					}
				} else if (_rebindType == RebindType.GamepadButton) {
					string keycode = "";
					if (_changePositiveKey) {
						if (_changeAltKey)
							keycode = _axisConfig.altPositive == KeyCode.None ? "" : _axisConfig.altPositive.ToString ();
						else
							keycode = _axisConfig.positive == KeyCode.None ? "" : _axisConfig.positive.ToString ();
					} else {
						if (_changeAltKey)
							keycode = _axisConfig.altNegative == KeyCode.None ? "" : _axisConfig.altNegative.ToString ();
						else
							keycode = _axisConfig.negative == KeyCode.None ? "" : _axisConfig.negative.ToString ();
					}
					_keyDescription.text = gamepadButtons.GetMapping (keycode);
				} else if (_rebindType == RebindType.GamepadAxis) {
					_keyDescription.text = gamepadButtons.GetMapping (_axisConfig.axis.ToString ());
				}
				else {
					_keyDescription.text = _axisConfig.axis.ToString ();
				}
			}
			else
			{
				_keyDescription.text = "";
				Debug.LogError(string.Format(@"Input configuration '{0}' does not exist or axis '{1}' does not exist", _inputConfigName, _axisConfigName));
			}
		}

		private void HandleConfigurationDirty(string configName)
		{
			if(configName == _inputConfigName)
				InitializeAxisConfig();
		}

		public void OnPointerDown(PointerEventData data)
		{
			StartCoroutine(StartInputScanDelayed());
		}

		private IEnumerator StartInputScanDelayed()
		{
			yield return null;

			if(!InputManager.IsScanning && _axisConfig != null)
			{
				_image.overrideSprite = _scanningState;
				_keyDescription.text = "...";
				
				ScanSettings settings;
				settings.joystick = _joystick;
				settings.cancelScanButton = _cancelButton;
				settings.timeout = _timeout;
				settings.userData = null;
				if(_rebindType == RebindType.GamepadAxis)
				{
					settings.scanFlags = ScanFlags.JoystickAxis;
					InputManager.StartScan(settings, HandleJoystickAxisScan);
				}
				else if(_rebindType == RebindType.GamepadButton)
				{
					settings.scanFlags = ScanFlags.JoystickButton;
					if(_allowAnalogButton)
					{
						settings.scanFlags = settings.scanFlags | ScanFlags.JoystickAxis;
					}
					InputManager.StartScan(settings, HandleJoystickButtonScan);
				}
				else if(_rebindType == RebindType.KeyboardAxis)
				{
					settings.scanFlags = ScanFlags.Key;
					InputManager.StartScan(settings, HandleKeyAxisScan);
				}
				else
				{
					settings.scanFlags = ScanFlags.Key;
					InputManager.StartScan(settings, HandleKeyScan);
				}
			}
		}

		private bool HandleKeyAxisScan(ScanResult result) {
			//	When you return false you tell the InputManager that it should keep scaning for other keys
			if(!IsKeyValid(result.key))
				return false;

			//	The key is KeyCode.None when the timeout has been reached or the scan has been canceled
			if(result.key != KeyCode.None)
			{
				//	If the key is KeyCode.Backspace clear the current binding
				result.key = (result.key == KeyCode.Backspace) ? KeyCode.None : result.key;

				_axisConfig.positive = result.key;

				_keyDescription.text = (result.key == KeyCode.None) ? "" : result.key.ToString();

				_keyDescription.text += "/";

				ScanSettings settings;
				settings.joystick = _joystick;
				settings.cancelScanButton = _cancelButton;
				settings.timeout = _timeout;
				settings.userData = null;
				settings.scanFlags = ScanFlags.Key;
				InputManager.StartScan(settings, HandleKeyAxisNegScan);
			}
			else
			{
				KeyCode currentKey = GetCurrentKeyCode();
				_keyDescription.text = (currentKey == KeyCode.None) ? "" : currentKey.ToString();
			}

			return true;
		}

		private bool HandleKeyAxisNegScan(ScanResult result) {
			//	When you return false you tell the InputManager that it should keep scaning for other keys
			if(!IsKeyValid(result.key))
				return false;

			//	The key is KeyCode.None when the timeout has been reached or the scan has been canceled
			if(result.key != KeyCode.None)
			{
				//	If the key is KeyCode.Backspace clear the current binding
				result.key = (result.key == KeyCode.Backspace) ? KeyCode.None : result.key;

				_axisConfig.negative = result.key;

				_keyDescription.text += (result.key == KeyCode.None) ? "" : result.key.ToString();

				ScanSettings settings;
				settings.joystick = _joystick;
				settings.cancelScanButton = _cancelButton;
				settings.timeout = _timeout;
				settings.userData = null;
				settings.scanFlags = ScanFlags.Key;
				InputManager.StartScan(settings, HandleKeyScan);
			}
			else
			{
				KeyCode currentKey = GetCurrentKeyCode();
				_keyDescription.text = (currentKey == KeyCode.None) ? "" : currentKey.ToString();
			}

			_image.overrideSprite = _normalState;
			return true;
		}

		private bool HandleKeyScan(ScanResult result)
		{
			//	When you return false you tell the InputManager that it should keep scaning for other keys
			if(!IsKeyValid(result.key))
				return false;

			//	The key is KeyCode.None when the timeout has been reached or the scan has been canceled
			if(result.key != KeyCode.None)
			{
				//	If the key is KeyCode.Backspace clear the current binding
				result.key = (result.key == KeyCode.Backspace) ? KeyCode.None : result.key;
				if(_changePositiveKey)
				{
					if(_changeAltKey)
						_axisConfig.altPositive = result.key;
					else
						_axisConfig.positive = result.key;
				}
				else
				{
					if(_changeAltKey)
						_axisConfig.altNegative = result.key;
					else
						_axisConfig.negative = result.key;
				}
				_keyDescription.text = (result.key == KeyCode.None) ? "" : result.key.ToString();
			}
			else
			{
				KeyCode currentKey = GetCurrentKeyCode();
				_keyDescription.text = (currentKey == KeyCode.None) ? "" : currentKey.ToString();
			}

			_image.overrideSprite = _normalState;
			return true;
		}

		private bool IsKeyValid(KeyCode key)
		{
			bool isValid = true;

			if(_rebindType == RebindType.Keyboard)
			{
				if((int)key >= (int)KeyCode.JoystickButton0)
					isValid = false;
				else if(key == KeyCode.LeftApple || key == KeyCode.RightApple)
					isValid = false;
				else if(key == KeyCode.LeftWindows || key == KeyCode.RightWindows)
					isValid = false;
			}
			else
			{
				isValid = false;
			}

			return isValid;
		}

		private bool HandleJoystickButtonScan(ScanResult result)
		{
			if(result.scanFlags == ScanFlags.JoystickButton)
			{
				//	When you return false you tell the InputManager that it should keep scaning for other keys
				if(!IsJoytickButtonValid(result.key))
					return false;
				
				//	The key is KeyCode.None when the timeout has been reached or the scan has been canceled
				if(result.key != KeyCode.None)
				{
					//	If the key is KeyCode.Backspace clear the current binding
					result.key = (result.key == KeyCode.Backspace) ? KeyCode.None : result.key;
					_axisConfig.type = InputType.Button;
					if(_changePositiveKey)
					{
						if(_changeAltKey)
							_axisConfig.altPositive = result.key;
						else
							_axisConfig.positive = result.key;
					}
					else
					{
						if(_changeAltKey)
							_axisConfig.altNegative = result.key;
						else
							_axisConfig.negative = result.key;
					}
					_keyDescription.text = gamepadButtons.GetMapping((result.key == KeyCode.None) ? "" : result.key.ToString());
				}
				else
				{
					if(_axisConfig.type == InputType.Button)
					{
						KeyCode currentKey = GetCurrentKeyCode();
						_keyDescription.text = gamepadButtons.GetMapping((currentKey == KeyCode.None) ? "" : currentKey.ToString());
					}
					else
					{
						_keyDescription.text = gamepadButtons.GetMapping((_axisConfig.invert ? "-" : "+") + gamepadButtons.GetMapping(_axisConfig.axis.ToString ()));
					}
				}
				_image.overrideSprite = _normalState;
			}
			else
			{
				//	The axis is negative when the timeout has been reached or the scan has been canceled
				if(result.joystickAxis >= 0)
				{
					_axisConfig.type = InputType.AnalogButton;
					_axisConfig.invert = result.joystickAxisValue < 0.0f;
					_axisConfig.SetAnalogButton(_joystick, result.joystickAxis);
					_keyDescription.text = (_axisConfig.invert ? "-" : "+") + gamepadButtons.GetMapping(_axisConfig.axis.ToString ());
				}
				else
				{
					if(_axisConfig.type == InputType.AnalogButton)
					{
						_keyDescription.text = (_axisConfig.invert ? "-" : "+") + gamepadButtons.GetMapping(_axisConfig.axis.ToString ());
					}
					else
					{
						KeyCode currentKey = GetCurrentKeyCode();
						_keyDescription.text = gamepadButtons.GetMapping((currentKey == KeyCode.None) ? "" : currentKey.ToString());
					}
				}
				_image.overrideSprite = _normalState;
			}
			
			return true;
		}

		private bool IsJoytickButtonValid(KeyCode key)
		{
			bool isValid = true;
			
			if(_rebindType == RebindType.GamepadButton)
			{
				//	Allow KeyCode.None to pass because it means that the scan has been canceled or the timeout has been reached
				//	Allow KeyCode.Backspace to pass so it can clear the current binding
				if((int)key < (int)KeyCode.JoystickButton0 && key != KeyCode.None && key != KeyCode.Backspace)
					isValid = false;
			}
			else
			{
				isValid = false;
			}
			
			return isValid;
		}

		private bool HandleJoystickAxisScan(ScanResult result)
		{
			//	The axis is negative when the timeout has been reached or the scan has been canceled
			if(result.joystickAxis >= 0)
				_axisConfig.SetAnalogAxis(_joystick, result.joystickAxis);

			_image.overrideSprite = _normalState;
			_keyDescription.text = gamepadButtons.GetMapping(_axisConfig.axis.ToString ());
			return true;
		}

		private KeyCode GetCurrentKeyCode()
		{
			if(_rebindType == RebindType.GamepadAxis)
				return KeyCode.None;

			if(_changePositiveKey)
			{
				if(_changeAltKey)
					return _axisConfig.altPositive;
				else
					return _axisConfig.positive;
			}
			else
			{
				if(_changeAltKey)
					return _axisConfig.altNegative;
				else
					return _axisConfig.negative;
			}
		}
	}
}