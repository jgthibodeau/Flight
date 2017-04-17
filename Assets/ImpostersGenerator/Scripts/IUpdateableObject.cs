using System;
using UnityEngine;

public interface IUpdateableObject {
    void UpdateObject(Vector3 _cameraPosition);
    Vector3 Position { get; }
}

