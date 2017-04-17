using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StaticOctreeNode{

    #region Members

    private static readonly int MAX_OBJECTS_IN_NODE = 10;

    private StaticOctreeNode[] children = null;
    private List<IOctreeObject> objects = new List<IOctreeObject>();

    private Bounds bounds = default(Bounds);

    private Vector3 center;
    private float size;
    private float minSize;

    #endregion

    #region Properties


    #endregion

    #region Methods

    public StaticOctreeNode(Vector3 _center, float _size, float _minSize) {
        center = _center;
        size = _size;
        minSize = _minSize;

        bounds = new Bounds(center, Vector3.one * size);
    }

    public void Add(IOctreeObject _object) {

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

        var _vector = (_position - center);
        var _dist = _vector.magnitude;
        if (_dist - size * 0.5f > _maxDistance) {
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

    private int GetChildIndex(Vector3 _position) {
        var _index = (_position.x <= center.x ? 0 : 1);
        if (_position.y < center.y) _index += 4;
        if (_position.z > center.z) _index += 2;
        return _index;
    }

    private void Split() {

        float _quarter = size * 0.25f;
        float _newSize = size * 0.5f;
        children = new StaticOctreeNode[8];

        for (int i = 0; i < 8; i++) {
            var xDir = i % 2 == 0 ? -1 : 1;
            var yDir = i > 3 ? -1 : 1;
            var zDir = (i < 2 || (i > 3 && i < 6)) ? -1 : 1;

            children[i] = new StaticOctreeNode(this.center + new Vector3(xDir * _quarter, yDir * _quarter, zDir * _quarter), _newSize, minSize);
        }
    }

    public void DebugDraw(float _depth = 0, float _totalDepth = 10) {

        // draw self (colorize depth)
        var _scale = _depth / _totalDepth;
        Gizmos.color = Color.Lerp(Color.green, Color.red, _scale);
        Gizmos.DrawWireCube(bounds.center, bounds.size);

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
