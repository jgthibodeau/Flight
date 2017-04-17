using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class Imposter : MonoBehaviour , IUpdateableObject , IOctreeObject{

    [SerializeField]
    private Material sharedMaterial = null;
    private static Material[,] materialsLookupArray = null;
    private static int slices = 8;
    private Quaternion initialRotation;

    private Renderer meshRenderer;

    [ExecuteInEditMode]
    private void Awake() {
        GetMeshRenderer();
        meshRenderer.sharedMaterial = sharedMaterial;

        initialRotation = this.transform.parent != null ? this.transform.parent.rotation : Quaternion.identity;
        
        if (materialsLookupArray == null) {
            materialsLookupArray = new Material[slices, slices];

            for (int i = 0; i < slices; i++) {
                for (int j = 0; j < slices; j++) {
                    var _newMat = Instantiate(meshRenderer.sharedMaterial) as Material;
                    _newMat.SetFloat("_OffsetX", j * (1 / (float)slices));
                    _newMat.SetFloat("_OffsetY", i * (1 / (float)slices));
                    materialsLookupArray[j, i] = _newMat;
                }
            }
        }

        if (Application.isPlaying) {
            UpdatesManager.Instance.Add(this);
        }
    }

    private void GetMeshRenderer() {
        if (meshRenderer == null) {
            meshRenderer = this.GetComponent<Renderer>();
        }
    }

    [ExecuteInEditMode]
    private void OnDisable() {
        GetMeshRenderer();

        meshRenderer.material = sharedMaterial;
        meshRenderer.sharedMaterial = sharedMaterial;
    }

    
//#if UNITY_EDITOR
//    [ExecuteInEditMode]
//    private void Update() {
//        if (!Application.isPlaying) {
//            UpdateObject(Camera.main.transform.position);
//        }
//    }
//#endif

    public Vector3 Position {
        get { return this.transform.position; }
    }

    public void UpdateObject(Vector3 _position) {
        transform.LookAt(_position, Vector3.up);

#if UNITY_EDITOR
        // for proper removing of generated materials in edit mode
        if (!Application.isPlaying) {
            materialsLookupArray = null;
        }
#endif
        if (materialsLookupArray == null) {
            return;
        }

        // calculate uv offset
        Vector3 eyeVec = initialRotation * (_position - this.transform.position).normalized;

        Vector2 tangent = (new Vector2(-eyeVec.z, eyeVec.x)).normalized;
        float angle = Mathf.Atan2(-tangent.x, tangent.y);

        angle += angle < 0 ? Mathf.PI * 2 : 0;
        int xOffset = Mathf.FloorToInt(Mathf.Floor(angle + 0.5f) % slices);

        tangent = new Vector2(1 - Mathf.Abs(eyeVec.y), eyeVec.y);
        angle = Mathf.Atan2(tangent.x, tangent.y) * 2;

        angle += angle < 0 ? Mathf.PI * 2 : 0;
        angle *= Mathf.PI * 0.5f;
        int yOffset  = Mathf.FloorToInt(Mathf.Floor(angle) % slices);

        // get proper lookup material for batching
        meshRenderer.sharedMaterial = materialsLookupArray[xOffset, yOffset];
    }
}
