using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private int _GridPositionIndex = 0;
    public int Index { get { return _GridPositionIndex; } set { _GridPositionIndex = value; } }
    private GridBehaviour _GridObject;

    [System.Serializable]
    public class SpawnEntry
    {
        public GameObject itemPrefab;
    }

    [Header("Item")]
    [SerializeField] private List<SpawnEntry> _possibleItems = new List<SpawnEntry>();


    public void SetGridObject(GridBehaviour gridObject) {_GridObject = gridObject; }

    public GameObject HandleUnboxing() 
    { 
        _GridObject.RemoveGridObject(_GridPositionIndex);

        int randomIndex = Random.Range(0, _possibleItems.Count);
        GameObject toSpawn = _possibleItems.ElementAt(randomIndex).itemPrefab;

        return toSpawn;
    }
}
