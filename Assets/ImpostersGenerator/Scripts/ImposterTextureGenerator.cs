using UnityEngine;
using System.Collections;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;

public class ImposterTextureGenerator : MonoBehaviour {

    [SerializeField]
    private int horizontalTiles = 8;
    [SerializeField]
    private int verticalTiles = 8;

    [SerializeField]
    private int horizontalMinAngle = 90;
    [SerializeField]
    private int horizontalMaxAngle = 270;

    [SerializeField]
    private int verticalMinAngle = -90;
    [SerializeField]
    private int verticalMaxAngle = 90;

    [SerializeField]
    private Vector2 textureSize = new Vector2(1024, 1024);

    public void Generate() {

        var _frameResolution = new Vector2(textureSize.x / (float)horizontalTiles, textureSize.y / (float)verticalTiles);
        RenderTexture _renderTexture = null;

        var _camera = this.GetComponentInChildren<Camera>();
        var _texture = new Texture2D((int)textureSize.x, (int)textureSize.y, TextureFormat.ARGB32, true);

        var _hAngleStep = (horizontalMaxAngle - horizontalMinAngle) / horizontalTiles;
        var _vAngleStep = (verticalMaxAngle - verticalMinAngle) / verticalTiles;

        for (int i = 0; i < verticalTiles; i++) {
            for (int j = 0; j < horizontalTiles; j++ ) {
                this.transform.rotation = Quaternion.Euler(verticalMinAngle + i * _vAngleStep,horizontalMinAngle - j * _hAngleStep, 0);

                RenderTexture.active = null;
                _camera.targetTexture = null;
                _renderTexture = new RenderTexture((int)_frameResolution.x, (int)_frameResolution.y, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
                _camera.targetTexture = _renderTexture;
                _camera.Render();

                RenderTexture.active = _renderTexture;
                _texture.ReadPixels(new Rect(0, 0, _frameResolution.x, _frameResolution.y), (int)(j * _frameResolution.x), (int)textureSize.y- (int)((i + 1) * _frameResolution.y));
                _texture.Apply();
            }
        }

        this.transform.rotation = Quaternion.identity;

        var bytes = _texture.EncodeToPNG();

        var _textureName = "GeneratedTexture.png";
        File.WriteAllBytes(Application.dataPath + "/ImpostersGenerator/" + _textureName, bytes);
        EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath("Assets/ImpostersGenerator/" + _textureName));
    }
}

#endif
