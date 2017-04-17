using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UpdatesManager : MonoBehaviour {

    #region Singleton

    private static UpdatesManager instance = null;

    public static UpdatesManager Instance {
        get {
            if (instance == null) {
                var _go = new GameObject("_UpdatesManager");
                instance = _go.AddComponent<UpdatesManager>();
                instance.Initialize();
            }

            return instance;
        }
    }

    #endregion

    #region Members

    private Quadtree octree;

    private List<Imposter> objectsToUpdate = new List<Imposter>();
    private List<IOctreeObject> nodesToUpdate = new List<IOctreeObject>();
    private int iterationIndex = 0;
    private float nextCheckTime = 0;

    private List<Imposter> allImposters = new List<Imposter>();

    #endregion

    #region Properties


    #endregion

    #region Methods

    public void Add(Imposter _imposter) {
        octree.Add(_imposter);
    }

    public void Remove(Imposter _imposter) {
        octree.Remove(_imposter);
    }

    private void Initialize() {
        octree = new Quadtree(Vector3.zero, 100, 10);
    }

    private void FixedUpdate() {
        var _position = Camera.main.transform.position;

        if (iterationIndex <= 0) {
            if (nextCheckTime > Time.unscaledTime) {
                return;
            }
            nodesToUpdate.Clear();

            octree.GetInView(ref nodesToUpdate, Camera.main.transform.position, Camera.main.transform.forward, Camera.main.farClipPlane, -(1 - Camera.main.fieldOfView / 180f));
            iterationIndex = nodesToUpdate.Count - 1;

            if (iterationIndex < 0) {
                nextCheckTime = Time.unscaledTime + 1;
            } else {
                objectsToUpdate = nodesToUpdate.ConvertAll(_item => _item as Imposter);
            }
        } else {
            var _camPosition = Camera.main.transform.position;

            var counter = 100;
            while (counter > 0 && iterationIndex > 0) {
                objectsToUpdate[iterationIndex].UpdateObject(_camPosition);
                iterationIndex--;
                counter--;
            }
        }
    }

    private void OnDrawGizmos() {
        if (octree != null) {
            octree.DebugDraw();
        }
    }

    #endregion
}
