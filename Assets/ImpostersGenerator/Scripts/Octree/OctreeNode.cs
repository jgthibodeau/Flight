using System;
using System.Collections.Generic;
using UnityEngine;

public class OctreeNode {

     #region Members

    private static readonly int MAX_OBJECTS_IN_NODE = 10;

    private OctreeNode[] children = null;
    private List<IOctreeObject> objects = new List<IOctreeObject>();

    private Bounds bounds = default(Bounds);
    private Vector3[] childrenCenters;

    private Vector3 center;
    private float size;
    private float minSize;

    #endregion

    #region Properties

    public Vector3 Center {
        get { return center; }
        private set { center = value; }
    }

    public Bounds Bounds {
        get { return bounds; }
        private set { bounds = value; }
    }

    public float Size {
        get { return size; }
        private set { size = value; }
    }

    #endregion

    #region Methods

    public OctreeNode(Vector3 _center, float _size, float _minSize) {
        Center = _center;
        Size = _size;
        minSize = _minSize;

        Bounds = new Bounds(center, Vector3.one * size);
        UpdateChildrenCenters();
    }

    public OctreeNode(OctreeNode _other) {
        this.Center = _other.center;
        this.Size = _other.Size;
        this.minSize = _other.minSize;

        this.Bounds = new Bounds(center, Vector3.one * size);
    }

    public bool Add(IOctreeObject _object) {

        if (!this.Bounds.Contains(_object.Position)) {
            return false;
        }

        Insert(_object);
        return true;
    }

    public bool Remove(IOctreeObject _object) {
        var _removed = false;

        for (int i = 0; i < objects.Count; i++) {
            if (objects[i].Equals(_object)) {
                _removed = true;
                break;
            }
        }

        if (!_removed && children != null) {
            for (int i = 0; i < 8; i++) {
                _removed = children[i].Remove(_object);

                if (_removed) {
                    Merge();
                    break;
                }
            }
        }

        return _removed;
    }

    public void SetChildren(OctreeNode[] _children) {
        children = _children;
    }

    private void Insert(IOctreeObject _object) {
        if (children == null && objects.Count < MAX_OBJECTS_IN_NODE) {
            objects.Add(_object);
        } else {
            if (children == null) {
                Split();

                for (int i = 0; i < objects.Count; i++) {
                    var _index = GetChildIndex(objects[i].Position);

                    children[_index].Add(objects[i]);
                    objects.RemoveAt(i);
                    i--;
                }
            }

            var _objIndex = GetChildIndex(_object.Position);
            children[_objIndex].Add(_object);
        }
    }

    public void GetInView(ref List<IOctreeObject> _objects, Vector3 _position, Vector3 _forward, float _maxDistance, float _minAngle) {

        var _vector = (_position - Center);
        var _dist = _vector.magnitude;
        if (_dist - Size * 0.5f > _maxDistance) {
            return;
        }

        if (objects.Count > 0 && Vector3.Dot(_forward, _vector) < _minAngle) {
            _objects.AddRange(objects);
        }

        if (children != null) {
            for (int i = 0; i < 8; i++) {
                children[i].GetInView(ref _objects, _position, _forward, _maxDistance, _minAngle);
            }
        }
    }

    public OctreeNode ShrinkIfPossible(float _initialSize) {

        if (size < (2 * _initialSize)) {
            return this;
        }

        if (objects.Count == 0 && (children == null || children.Length == 0)) {
            return this;
        }

        int bestFit = -1;
        for (int i = 0; i < objects.Count; i++) {
            IOctreeObject curObj = objects[i];
            int newBestFit = GetChildIndex(curObj.Position);
            if (i == 0 || newBestFit == bestFit) {
                if (bestFit < 0) {
                    bestFit = newBestFit;
                }
            } else {
                return this; 
            }
        }

        if (children != null) {
            bool childHadContent = false;
            for (int i = 0; i < children.Length; i++) {
                if (children[i].HasAnyObjects()) {
                    if (childHadContent) {
                        return this; 
                    }
                    if (bestFit >= 0 && bestFit != i) {
                        return this; 
                    }
                    childHadContent = true;
                    bestFit = i;
                }
            }
        }

        if (children == null) {
            size = size / 2;
            center = childrenCenters[bestFit];
            Bounds = new Bounds(center, Vector3.one * size);
            UpdateChildrenCenters();
            return this;
        } else {
            return children[bestFit];
        }
    }

    private bool HasAnyObjects() {
        if (objects.Count > 0) return true;

        if (children != null) {
            for (int i = 0; i < 8; i++) {
                if (children[i].HasAnyObjects()) return true;
            }
        }

        return false;
    }

    private int GetChildIndex(Vector3 _position) {
        var _index = (_position.x <= Center.x ? 0 : 1);
        if (_position.y < Center.y) _index += 4;
        if (_position.z > Center.z) _index += 2;
        return _index;
    }

    private void Split() {

        float _quarter = Size * 0.25f;
        float _newSize = Size * 0.5f;
        children = new OctreeNode[8];

        for (int i = 0; i < 8; i++) {
            var xDir = i % 2 == 0 ? -1 : 1;
            var yDir = i > 3 ? -1 : 1;
            var zDir = (i < 2 || (i > 3 && i < 6)) ? -1 : 1;

            children[i] = new OctreeNode(this.Center + new Vector3(xDir * _quarter, yDir * _quarter, zDir * _quarter), _newSize, minSize);
        }
    }

    private void Merge() {

        for (int i = 0; i < 8; i++) {
            OctreeNode curChild = children[i];
            var numObjects = curChild.objects.Count;

            for (int j = 0; j < numObjects - 1; j++ ) {
                IOctreeObject curObj = curChild.objects[j];
                objects.Add(curObj);
            }
        }

        children = null;
    }

    private void UpdateChildrenCenters() {

        float quarter = size / 4f;
        childrenCenters = new Vector3[8];
        childrenCenters[0] = Center + new Vector3(-quarter, quarter, -quarter);
        childrenCenters[1] = Center + new Vector3(quarter, quarter, -quarter);
        childrenCenters[2] = Center + new Vector3(-quarter, quarter, quarter);
        childrenCenters[3] = Center + new Vector3(quarter, quarter, quarter);
        childrenCenters[4] = Center + new Vector3(-quarter, -quarter, -quarter);
        childrenCenters[5] = Center + new Vector3(quarter, -quarter, -quarter);
        childrenCenters[6] = Center + new Vector3(-quarter, -quarter, quarter);
        childrenCenters[7] = Center + new Vector3(quarter, -quarter, quarter);
    }

    public void DebugDraw(float _depth = 0, float _totalDepth = 10) {

        // draw self (colorize depth)
        var _scale = _depth / _totalDepth;
        Gizmos.color = Color.Lerp(Color.green, Color.red, _scale);
        Gizmos.DrawWireCube(Bounds.center, Bounds.size);

        // draw objects
        Gizmos.color = Color.blue;
        for (int i = 0; i < objects.Count; i++) {
            var _childBounds = new Bounds(objects[i].Position, Vector3.one * minSize * 0.5f);
            Gizmos.DrawWireCube(_childBounds.center, _childBounds.size);
        }

        // draw children
        if (children != null) {
            _depth++;
            for (int i = 0; i < 8; i++) {
                children[i].DebugDraw(_depth, _totalDepth);
            }
        }

        // restore color
        Gizmos.color = Color.white;
    }

    #endregion
}

