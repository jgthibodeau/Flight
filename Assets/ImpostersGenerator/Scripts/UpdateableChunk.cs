using UnityEngine;
using System.Collections;

public class UpdateableChunk : MonoBehaviour, IUpdateableChunk {

    private Vector2 cachedPosition;
    private Vector3 cachedPosition3;
    private IUpdateableObject[] updateableObjects;

    void Awake() {
        cachedPosition = new Vector2(this.transform.position.x, this.transform.position.z);
        cachedPosition3 = this.transform.position;

        updateableObjects = this.GetComponentsInChildren<IUpdateableObject>();

        Vector3 _minPos = new Vector3(float.MaxValue,float.MaxValue,float.MaxValue);
        Vector3 _maxPos = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        for (int i = 0; i < updateableObjects.Length; i++) {
            _minPos = Vector3.Min(_minPos, updateableObjects[i].Position);
            _maxPos = Vector3.Max(_maxPos, updateableObjects[i].Position);
        }

        var _center = (_minPos + _maxPos) * 0.5f;
        var _size = (_maxPos - _minPos);

        //UpdatesManager.Instance.AddObject(this, new Bounds(_center,_size));
    }

    public void UpdateChunk(Vector3 _position) {
        for (int i = 0; i < updateableObjects.Length; i++) {
            updateableObjects[i].UpdateObject(_position);
        }
    }

    public Vector3 Position {
        get { return cachedPosition3; }
    }

    public void NotifyUpdate() {
        this.UpdateChunk(Camera.main.transform.position);
    }
}
