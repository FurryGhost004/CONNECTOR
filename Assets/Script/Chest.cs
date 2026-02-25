using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private Item itemInside ;
    bool isOpened = false;
    bool playerInRange = false;
    bool requireKey = false;


    private void Update()
    {
        if (isOpened || !playerInRange || !Input.GetKeyDown(KeyCode.F))
        {
            return;
        }
        if (requireKey )
        {
            if (InventoryManager.Instance.HasKeyItem())
            {
                InventoryManager.Instance.UseKeyItem();
                OpenChest();
            }
            else
            {
                Debug.Log("Chest is locked. You need a key to open it.");
            }
        }
        else if (!isOpened && playerInRange && !requireKey && Input.GetKeyDown(KeyCode.F))
        {
            OpenChest();
        }

    }
    public void Initialize(Item item, bool needsKey)
    {
        this.itemInside = item;
        this.requireKey = needsKey;
        this.isOpened = false;
        Debug.Log($"{gameObject.name} initialized. Locked: {needsKey}");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void OpenChest()
    {
        if (isOpened)
        {
            return;
        }
        if (itemInside != null)
        {
            InventoryManager.Instance.AddItem(itemInside);
        }
        isOpened = true;

    }
}
