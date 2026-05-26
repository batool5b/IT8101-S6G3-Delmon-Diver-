using Fungus;
using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField] public string treeType;

    [SerializeField] public string Name;

    [SerializeField] public int health = 3;

    [SerializeField] private int axeDamage = 3;

    [SerializeField] private int PickaxeDamage = 3;

    [SerializeField] private int handDamage = 1;

    [HideInInspector] public Terrain terrain;
    [HideInInspector] public int treeIndex;

    public TreeDrops[] drops;
    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager =
            FindFirstObjectByType<InventoryManager>();
    }

    public void Chop()
    {
        ItemClass selectedItem =
            inventoryManager.selectedItem;

        bool usingAxe =
            selectedItem != null &&
            selectedItem.itemName == "Axe";

        bool usingPickaxe =
            selectedItem != null &&
            selectedItem.itemName == "Pickaxe";

        if (usingAxe && treeType == "Tree")
            health -= axeDamage;
        else if (usingPickaxe && treeType == "Rock")
            health -= PickaxeDamage;
        else
            health -= handDamage;

        if (health <= 0)
        {
            DestroyTree();
        }
    }

    void DestroyTree()
    {
        RemoveTerrainTree();
        DropItems();
        Destroy(gameObject);
    }

    void RemoveTerrainTree()
    {
        TerrainData data = terrain.terrainData;

        TreeInstance[] trees = data.treeInstances;

        if (treeIndex < 0 || treeIndex >= trees.Length)
            return;

        TreeInstance tree = trees[treeIndex];

        tree.position = new Vector3(-9999, -9999, -9999);

        trees[treeIndex] = tree;

        data.treeInstances = trees;
    }

    void DropItems()
    {
        foreach (TreeDrops drop in drops)
        {
            int amount = Random.Range(
                drop.minAmount,
                drop.maxAmount + 1);

            for (int i = 0; i < amount; i++)
            {
                Vector3 randomOffset =
                    new Vector3(
                        Random.Range(-1f, 1f),
                        0.5f,
                        Random.Range(-1f, 1f));

                Instantiate(
                    drop.dropPrefab,
                    transform.position + randomOffset,
                    Quaternion.identity);
            }
        }
    }
}