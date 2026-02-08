using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class InventoryManager : MonoBehaviour
{
    [SerializeField] List<Item> itemsList = new List<Item>();
    [SerializeField] Image[] itemSlotImage;
    [SerializeField] Transform playerPos;
    [SerializeField] float cooldown = 1f;
    [SerializeField] float minThrowForce = 1f;
    [SerializeField] float maxThrowForce = 15f;
    [SerializeField] float minRange = 2f;
    [SerializeField] float maxRange = 8f;
    [SerializeField] float chargeTime = 1.5f;

    [SerializeField] Transform tutorialSpawnPoint1;
    [SerializeField] Transform tutorialSpawnPoint2;

    private float lastTimeUse = -Mathf.Infinity;
    float chargeStart = 0f;
    bool isCharge = false;
    int chargeItemIndex;

    public static InventoryManager Instance;
    public int maxSlots = 4;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        UseItemInput();
    }
    public bool AddItem(Item newItem)
    {
        if (itemsList.Count >= maxSlots)
        {
            return false;
        }
        else
        {
            itemsList.Add(newItem);
            UpdateUI();
            return true;
        }
    }

    public void UseItemInput()
    {

        for (int i = 0; i < 4; i++)
        {

            if (i < itemsList.Count)
            {
                Item item = itemsList[i];
                if (Time.time - lastTimeUse < cooldown)
                {
                    continue;
                }
                if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + i)))
                {
                    Debug.Log(i);
                    if (item.type == ItemType.SetUpItem)
                    {
                        UseSetupItem(i);
                        lastTimeUse = Time.time;
                    }
                    else if (item.type == ItemType.ConsumeableItem)
                    {
                        UseConsumableItem(i);
                        lastTimeUse = Time.time;
                    }
                    else if (item.type == ItemType.ThrowItem)
                    {
                        Charge(i);
                    }
                }
                if (Input.GetKeyUp((KeyCode)((int)KeyCode.Alpha1 + i)))
                {
                    if (item.type == ItemType.ThrowItem)
                    {
                        Release(i);
                        lastTimeUse = Time.time;
                    }
                }
            }
        }
    }
    void UpdateUI()
    {
        for (int i = 0; i < itemSlotImage.Length; i++)
        {
            if (i < itemsList.Count)
            {
                itemSlotImage[i].sprite = itemsList[i].icon;
                itemSlotImage[i].GetComponent<ItemUI>().item = itemsList[i];
                itemSlotImage[i].color = Color.white;
            }
            else
            {
                itemSlotImage[i].sprite = null;
                itemSlotImage[i].GetComponent<ItemUI>().item = null;
                itemSlotImage[i].color = new Color(1, 1, 1, 0);
            }
        }
    }

    void UseSetupItem(int index)
    {
        GameObject obj = Instantiate(itemsList[index].prefab, playerPos.position, Quaternion.identity);
        itemsList.RemoveAt(index);
        UpdateUI();
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnItemSet();
        }
    }
    void UseConsumableItem(int index)
    {
        itemsList.RemoveAt(index);
        UpdateUI();
    }

    void Charge(int index)
    {
        isCharge = true;
        chargeItemIndex = index;
        chargeStart = Time.time;
    }
    void Release(int index)
    {
        if (!isCharge || index != chargeItemIndex)
        {
            return;
        }
        isCharge = false;
        float chargeDuration = Time.time - chargeStart;
        Item item = itemsList[index];
        float throwForce = minThrowForce + Mathf.Clamp01(chargeDuration / chargeTime) * (maxThrowForce - minThrowForce);
        ThrowItem(item, throwForce, chargeDuration);
        itemsList.RemoveAt(index);
        UpdateUI();

    }

    void ThrowItem(Item item, float force, float chargeDuration)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector2 dir = (mousePos - playerPos.position).normalized;
        GameObject throwItem = Instantiate(item.prefab, playerPos.position, Quaternion.identity);
        Rigidbody2D rb = throwItem.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(dir * force, ForceMode2D.Impulse);
            float ChargePercentage = Mathf.Clamp01(chargeDuration / chargeTime);
            float Range = Mathf.Lerp(minRange, maxRange, ChargePercentage);
            StartCoroutine(LimitRange(rb, playerPos.position, Range));
        }
    }
    private IEnumerator LimitRange(Rigidbody2D rb, Vector2 startPos, float maxRange)
    {
        while (rb != null)
        {
            if (Vector2.Distance(rb.position, startPos) > maxRange)
            {
                rb.linearVelocity = Vector2.zero;
                break;
            }
            yield return null;
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerPos = player.transform;
        }
        GameObject tutorialItem1 = GameObject.Find("TutorialSpawnPoint1");
        if (tutorialItem1 != null)
        {
            tutorialSpawnPoint1 = tutorialItem1.transform;
        }
        GameObject tutorialItem2 = GameObject.Find("TutorialSpawnPoint2");
        if (tutorialItem2 != null)
        {
            tutorialSpawnPoint2 = tutorialItem2.transform;
        }
        RefreshUIReferences();
    }
    private void RefreshUIReferences()
    {

        Transform slotRoot = GameObject.Find("InventorySlot")?.transform;
        if (slotRoot == null)
        {
            return;
        }
        itemSlotImage = new Image[slotRoot.childCount];
        for (int i = 0; i < slotRoot.childCount; i++)
        {
            itemSlotImage[i] = slotRoot.GetChild(i).GetComponent<Image>();
        }
        UpdateUI();
    }
    public void SpawnTutorialItem(GameObject prefab, int pointNumber)
    {
        Transform targetPoint = (pointNumber == 1) ? tutorialSpawnPoint1 : tutorialSpawnPoint2;
        if (targetPoint != null)
        {
            Instantiate(prefab, targetPoint.position, Quaternion.identity);
        }


    }
    public bool HasKeyItem()
    {
        foreach (Item item in itemsList)
        {
            if (item.type == ItemType.KeyItem)
            {
                return true;
            }
        }
        return false;
    }
    public void UseKeyItem()
    {
        for (int i = 0; i < itemsList.Count; i++)
        {
            if (itemsList[i].type == ItemType.KeyItem)
            {
                itemsList.RemoveAt(i);
                UpdateUI();
                return;
            }
        }
    }

}

