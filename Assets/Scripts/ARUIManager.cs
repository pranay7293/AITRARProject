using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.UI;

public class ARUIManager : MonoBehaviour
{
    [SerializeField] Button createButton;
    [SerializeField] Button deleteButton;
    [SerializeField] GameObject furnitureMenu;
    [SerializeField] Animator furnitureMenuAnimator;
    [SerializeField] FurnitureSpawner furnitureSpawner;
    [SerializeField] Button cancelButton;
    [SerializeField] XRInteractionGroup interactionGroup;
    [SerializeField] XRInputValueReader<Vector2> tapStartPositionInput = new XRInputValueReader<Vector2>("Tap Start Position");

    bool showFurnitureMenu;

    void OnEnable()
    {
        tapStartPositionInput.EnableDirectActionIfModeUsed();
        createButton.onClick.AddListener(ShowMenu);
        cancelButton.onClick.AddListener(HideMenu);
        deleteButton.onClick.AddListener(DeleteFocusedFurniture);
    }

    void OnDisable()
    {
        tapStartPositionInput.DisableDirectActionIfModeUsed();
        showFurnitureMenu = false;
        createButton.onClick.RemoveListener(ShowMenu);
        cancelButton.onClick.RemoveListener(HideMenu);
        deleteButton.onClick.RemoveListener(DeleteFocusedFurniture);
    }

    void Start()
    {
        HideMenu();
    }

    void Update()
    {
        if (showFurnitureMenu)
        {
            createButton.gameObject.SetActive(false);
            deleteButton.gameObject.SetActive(false);
            var isPointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(-1);
            if (!isPointerOverUI && tapStartPositionInput.TryReadValue(out _))
            {
                HideMenu();
            }
        }
        else if (interactionGroup != null)
        {
            var currentFocusedFurniture = interactionGroup.focusInteractable;
            if (currentFocusedFurniture != null && (!deleteButton.isActiveAndEnabled || createButton.isActiveAndEnabled))
            {
                createButton.gameObject.SetActive(false);
                deleteButton.gameObject.SetActive(true);
            }
            else if (currentFocusedFurniture == null && (!createButton.isActiveAndEnabled || deleteButton.isActiveAndEnabled))
            {
                createButton.gameObject.SetActive(true);
                deleteButton.gameObject.SetActive(false);
            }
        }
    }

    public void SetFurnitureToSpawn(int furnitureIndex)
    {
        if (furnitureSpawner == null)
        {
            Debug.LogWarning("UI Manager not configured correctly: no Furniture Spawner set.", this);
        }
        else
        {
            if (furnitureIndex < furnitureSpawner.furniturePrefabs.Count)
            {
                furnitureSpawner.spawnOptionIndex = furnitureIndex;
            }
            else
            {
                Debug.LogWarning("Furniture Spawner not configured correctly: furniture index larger than number of Furniture Prefabs.", this);
            }
        }

        HideMenu();
    }

    void ShowMenu()
    {
        showFurnitureMenu = true;
        furnitureMenu.SetActive(true);
        if (!furnitureMenuAnimator.GetBool("Show"))
        {
            furnitureMenuAnimator.SetBool("Show", true);
        }
    }

    public void HideMenu()
    {
        furnitureMenuAnimator.SetBool("Show", false);
        showFurnitureMenu = false;
    }

    void DeleteFocusedFurniture()
    {
        if (interactionGroup == null)
            return;

        var currentFocusedFurniture = interactionGroup.focusInteractable;
        if (currentFocusedFurniture != null)
        {
            Destroy(currentFocusedFurniture.transform.gameObject);
        }
    }
}

