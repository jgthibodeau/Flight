using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StaticOctree {

    #region Members

    private StaticOctreeNode root = null;

    #endregion

    #region Properties


    #endregion

    #region Methods

    public StaticOctree(IOctreeObject[] _objects, float _minSize) {

        if (_objects == null || _objects.Length == 0) {
            Debug.LogWarning("Empty array passed to StaticOctre. Aborting");
            return;
        }

        BuildTree(_objects, _minSize);
    }

    public void DebugDraw() {
        root.DebugDraw();
    }

    public void GetInView(ref List<IOctreeObject> _objects, Vector3 _position, Vector3 _forward, float _maxDistance, float _minAngle) {
        root.GetInView(ref _objects, _position, _forward, _maxDistance, _minAngle);
    }

    private void BuildTree(IOctreeObject[] _objects, float _minSize) {

        Vector3 _minPos = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 _maxPos = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        for (int i = 0; i < _objects.Length; i++) {
            _minPos = Vector3.Min(_minPos, _objects[i].Position);
            _maxPos = Vector3.Max(_maxPos, _objects[i].Position);
        }

        var _center = (_minPos + _maxPos) * 0.5f;
        var _size = (_maxPos - _minPos);
        var _maxSize = Mathf.Max(_size.x, _size.y, _size.z);

        if (_minSize > _maxSize || _minSize <= 0) {
            _minSize = _maxSize * 0.1f;
        }

        root = new StaticOctreeNode(_center, _maxSize, _minSize);

        for (int i = 0; i < _objects.Length; i++) {
            this.AddObject(_objects[i]);
        }
    }

    private void AddObject(IOctreeObject _object) {
        root.Add(_object);
    }

    #endregion
}
