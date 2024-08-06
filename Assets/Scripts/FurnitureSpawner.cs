using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureSpawner : MonoBehaviour
{
    public Camera cameraToFace;
    public List<GameObject> furniturePrefabs = new List<GameObject>();
    public GameObject spawnVisualizationPrefab;
    public int spawnOptionIndex = -1;
    public bool onlySpawnInView = true;
    public float viewportPeriphery = 0.15f;
    public bool applyRandomAngleAtSpawn = true;
    public float spawnAngleRange = 45f;
    public bool spawnAsChildren;

    public event Action<GameObject> furnitureSpawned;

    void Awake()
    {
        EnsureFacingCamera();
    }

    void EnsureFacingCamera()
    {
        if (cameraToFace == null)
            cameraToFace = Camera.main;
    }

    public void RandomizeSpawnOption()
    {
        spawnOptionIndex = -1;
    }

    public bool TrySpawnFurniture(Vector3 spawnPoint, Vector3 spawnNormal)
    {
        if (onlySpawnInView)
        {
            var inViewMin = viewportPeriphery;
            var inViewMax = 1f - viewportPeriphery;
            var pointInViewportSpace = cameraToFace.WorldToViewportPoint(spawnPoint);
            if (pointInViewportSpace.z < 0f || pointInViewportSpace.x > inViewMax || pointInViewportSpace.x < inViewMin ||
                pointInViewportSpace.y > inViewMax || pointInViewportSpace.y < inViewMin)
            {
                return false;
            }
        }

        var furnitureIndex = spawnOptionIndex < 0 || spawnOptionIndex >= furniturePrefabs.Count
                          ? UnityEngine.Random.Range(0, furniturePrefabs.Count)
                          : spawnOptionIndex;
        var newFurniture = Instantiate(furniturePrefabs[furnitureIndex]);
        if (spawnAsChildren)
            newFurniture.transform.parent = transform;

        newFurniture.transform.position = spawnPoint;
        EnsureFacingCamera();

        var facePosition = cameraToFace.transform.position;
        var forward = facePosition - spawnPoint;
        var projectedForward = Vector3.ProjectOnPlane(forward, spawnNormal);
        newFurniture.transform.rotation = Quaternion.LookRotation(projectedForward, spawnNormal);

        if (applyRandomAngleAtSpawn)
        {
            var randomRotation = UnityEngine.Random.Range(-spawnAngleRange, spawnAngleRange);
            newFurniture.transform.Rotate(Vector3.up, randomRotation);
        }

        if (spawnVisualizationPrefab != null)
        {
            var visualizationTrans = Instantiate(spawnVisualizationPrefab).transform;
            visualizationTrans.position = spawnPoint;
            visualizationTrans.rotation = newFurniture.transform.rotation;
        }

        furnitureSpawned?.Invoke(newFurniture);
        return true;
    }
}
