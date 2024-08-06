using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ARFurniturePlacementTrigger : MonoBehaviour
{
    public enum SpawnTriggerType
    {
        SelectAttempt,
        InputAction,
    }

    public XRRayInteractor arInteractor;
    public FurnitureSpawner furnitureSpawner;
    public bool requireHorizontalUpSurface;
    public SpawnTriggerType spawnTriggerType;
    public XRInputButtonReader spawnFurnitureInput = new XRInputButtonReader("Spawn Furniture");
    public bool blockSpawnWhenInteractorHasSelection = true;

    private bool attemptSpawn;
    private bool everHadSelection;

    void OnEnable()
    {
        spawnFurnitureInput.EnableDirectActionIfModeUsed();
    }

    void OnDisable()
    {
        spawnFurnitureInput.DisableDirectActionIfModeUsed();
    }

    void Start()
    {
        if (furnitureSpawner == null)
            furnitureSpawner = FindObjectOfType<FurnitureSpawner>();

        if (arInteractor == null)
        {
            Debug.LogError("Missing AR Interactor reference, disabling component.", this);
            enabled = false;
        }
    }

    void Update()
    {
        if (attemptSpawn)
        {
            attemptSpawn = false;
            var isPointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(-1);
            if (!isPointerOverUI && arInteractor.TryGetCurrentARRaycastHit(out var arRaycastHit))
            {
                if (arRaycastHit.trackable is ARPlane arPlane)
                {
                    if (requireHorizontalUpSurface && arPlane.alignment != PlaneAlignment.HorizontalUp)
                        return;

                    furnitureSpawner.TrySpawnFurniture(arRaycastHit.pose.position, arPlane.normal);
                }
            }
            return;
        }

        var selectState = arInteractor.logicalSelectState;

        if (blockSpawnWhenInteractorHasSelection)
        {
            if (selectState.wasPerformedThisFrame)
                everHadSelection = arInteractor.hasSelection;
            else if (selectState.active)
                everHadSelection |= arInteractor.hasSelection;
        }

        attemptSpawn = false;
        switch (spawnTriggerType)
        {
            case SpawnTriggerType.SelectAttempt:
                if (selectState.wasCompletedThisFrame)
                    attemptSpawn = !arInteractor.hasSelection && !everHadSelection;
                break;

            case SpawnTriggerType.InputAction:
                if (spawnFurnitureInput.ReadWasPerformedThisFrame())
                    attemptSpawn = !arInteractor.hasSelection && !everHadSelection;
                break;
        }
    }
}

