using UnityEngine;

public class CharacterConroller2 : MonoBehaviour
{
    [SerializeField] GameObject character;
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] GameObject questPanel;
    [SerializeField] GameObject shopPanel;

    private Animator animator;
    private PickupItem pickupItem;
    Rigidbody2D rb;
    bool isNearNPC = false;
    bool isNearShop = false;
    bool isNearQuestBoard = false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        Move();
        if (isNearQuestBoard && Input.GetKey(KeyCode.F))
        {
            if (TutorialSystem.IsCompleted(TutorialType.Finish))
            {
                questPanel.SetActive(true);
            }
        }
        if (isNearShop && Input.GetKey(KeyCode.F))
        {
            shopPanel.SetActive(true);
        }
        CheckItem();
    }
    void Move()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Di chuyển
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, verticalInput * moveSpeed);

        // Lật mặt nhân vật
        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1f, 1f, 1f);

        // Animation bằng GetKey
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }
    }
    void CheckItem()
    {
        if (pickupItem != null && pickupItem.isInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (InventoryManager.Instance.AddItem(pickupItem.itemData))
            {
                Destroy(pickupItem.gameObject);
                pickupItem = null;
                if (TutorialManager.Instance != null)
                {
                    TutorialManager.Instance.OnItemPicked();
                }
            }
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("NPC"))
        {
            isNearNPC = true;
            Debug.Log(isNearNPC);
        }
        if (collision.gameObject.CompareTag("Shop"))
        {
            isNearShop = true;
        }
        if (collision.CompareTag("item"))
        {
            pickupItem = collision.GetComponent<PickupItem>();
        }
        if (collision.gameObject.CompareTag("QuestBoard"))
        {
            isNearQuestBoard = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("NPC")) { isNearNPC = false; }
        if (collision.CompareTag("Shop")) { isNearShop = false; }
        if (collision.CompareTag("QuestBoard")) { isNearQuestBoard = false; }
        if (collision.CompareTag("item"))
        {
            if (pickupItem != null && pickupItem.gameObject == collision.gameObject)
            {
                pickupItem = null;
            }
        }
    }
}
