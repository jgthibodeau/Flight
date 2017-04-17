using System;
using System.Collections.Generic;
using UnityEngine;

public class Octree {

    #region Members

    private OctreeNode root = null;
    private float minSize;
    private float initialSize;

    #endregion

    #region Properties


    #endregion

    #region Methods

    public Octree(Vector3 _center, float _initialSize, float _minSize) {
        minSize = _minSize;
        initialSize = _initialSize;
        root = new OctreeNode(_center, _initialSize, _minSize);
    }

    public void Add(IOctreeObject _object){

        int safeCounter = 0;
        while (!root.Add(_object)) {
            Expand( _object.Position - root.Center);

            safeCounter++;
            if (safeCounter >= 10) {
                return;
            }
        }
    }

    public bool Remove(IOctreeObject _object) {
        var _removed = root.Remove(_object);

        if (_removed) {
            Shrink();
        }

        return _removed;
    }

    public void DebugDraw() {
        root.DebugDraw();
    }

    public void GetInView(ref List<IOctreeObject> _objects, Vector3 _position, Vector3 _forward, float _maxDistance, float _minAngle) {
        root.GetInView(ref _objects, _position, _forward, _maxDistance, _minAngle);
    }

    private void Shrink() {
        root = root.ShrinkIfPossible(initialSize);
    }

    private void Expand(Vector3 _expandDirection) {

        _expandDirection.x = _expandDirection.x >= 0 ? 1 : -1;
        _expandDirection.y = _expandDirection.y >= 0 ? 1 : -1;
        _expandDirection.z = _expandDirection.z >= 0 ? 1 : -1;

        OctreeNode _oldRoot = root;
        float _halfSize = root.Size * 0.5f;
        float _newSize = root.Size * 2f;
        Vector3 _center = root.Center + new Vector3(_expandDirection.x * _halfSize, _expandDirection.y * _halfSize, _expandDirection.z * _halfSize);
        root = new OctreeNode(_center, _newSize, minSize);

        int _existingChildIndex = GetNodeIndex(_expandDirection.x, _expandDirection.y, _expandDirection.z);
        OctreeNode[] _children = new OctreeNode[8];
        for (int i = 0; i < 8; i++) {
            if (_existingChildIndex == i) {
                _children[i] = _oldRoot;
            } else {
                _expandDirection.x = i % 2 == 0 ? -1 : 1;
                _expandDirection.y = i > 3 ? -1 : 1;
                _expandDirection.z = (i < 2 || (i > 3 && i < 6)) ? -1 : 1;

                _children[i] = new OctreeNode(_center + new Vector3(_expandDirection.x * _halfSize, _expandDirection.y * _halfSize, _expandDirection.z * _halfSize), _oldRoot.Size, minSize);
            }
        }

        root.SetChildren(_children);
    }

    private int GetNodeIndex(float xDir, float yDir, float zDir) {
        int result = xDir < 0 ? 1 : 0;
        if (yDir > 0) result += 4;
        if (zDir < 0) result += 2;
        return result;
    }

    #endregion
}

