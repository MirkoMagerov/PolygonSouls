using UnityEngine;

public class LootVisual : MonoBehaviour
{
    public int lootID;
    private bool isPlayerInRange = false;
    public GameObject collectPrompt;
    public GameObject lootPanel;
    private bool looted = false;

    void Start()
    {
        collectPrompt.SetActive(false);
        lootPanel.SetActive(false);
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!looted)
            {
                looted = true;
                CollectItem();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !looted)
        {
            isPlayerInRange = true;
            collectPrompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            collectPrompt.SetActive(false);

            if (looted)
            {
                Destroy(gameObject);
            }
        }
    }

    private void CollectItem()
    {
        LootableItemSO lootableItem = LootManager.Instance.GetLoot(lootID);
        collectPrompt.SetActive(false);

        CanvasLootableItem canvasItem = lootPanel.GetComponent<CanvasLootableItem>();
        canvasItem.InitializeCanvas(lootableItem);
        lootPanel.SetActive(true);

        lootableItem.OnLoot(GameObject.FindGameObjectWithTag("Player"));
    }
}