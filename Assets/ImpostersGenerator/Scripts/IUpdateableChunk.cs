using System;
using UnityEngine;

public interface IUpdateableChunk {

    void UpdateChunk(Vector3 _position);
    Vector3 Position { get; }
}

