using UnityEngine;
using System.Collections.Generic;
using Fungus;

public class TerrainTreeColliders : MonoBehaviour
{
    [Header("References")]
    public Terrain terrain;
    public Transform player;

    [Header("Collider Settings")]
    public float colliderRadius = 20f;
    public float checkInterval = 0.2f;

    [Header("Tree Drops")]
    public TreeDrops[] palmTreeDrops;
    public TreeDrops[] smallpalmTreeDrops;
    public TreeDrops[] oakTreeDrops;

    public TreeDrops[] RockDrops;

    public TreeDrops[] SmallRockDrops;

    public TreeDrops[] BananaTreeDrops;

    // ─────────────────────────────────────────────────────────────

    private struct TreeColliderData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public int treeIndex;

        public string treeName;
    }

    private List<TreeColliderData> _treeData = new();
    private Dictionary<int, GameObject> _activeColliders = new();

    private float _timer;

    // ─────────────────────────────────────────────────────────────

    void Start()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain not assigned!");
            return;
        }

        TerrainData data = terrain.terrainData;

        Vector3 terrainPos =
            terrain.transform.position;

        TreePrototype[] prototypes =
            data.treePrototypes;

        // Read all terrain trees
        for (int t = 0; t < data.treeInstances.Length; t++)
        {
            TreeInstance tree =
                data.treeInstances[t];

            GameObject prefab =
                prototypes[tree.prototypeIndex].prefab;

            Vector3 worldPos =
                Vector3.Scale(tree.position, data.size)
                + terrainPos;

            string treeName =
                prefab.name;

            _treeData.Add(new TreeColliderData
            {
                position = worldPos,

                rotation =
                    Quaternion.Euler(
                        0,
                        tree.rotation * Mathf.Rad2Deg,
                        0),

                scale =
                    new Vector3(
                        tree.widthScale,
                        tree.heightScale,
                        tree.widthScale),

                treeIndex = t,

                treeName = treeName
            });
        }

        Debug.Log(
            $"Loaded {_treeData.Count} terrain trees");
    }

    // ─────────────────────────────────────────────────────────────

    void Update()
    {
        if (player == null)
            return;

        _timer += Time.deltaTime;

        if (_timer < checkInterval)
            return;

        _timer = 0f;

        float radiusSq =
            colliderRadius * colliderRadius;

        Vector3 playerPos =
            player.position;

        HashSet<int> shouldBeActive = new();

        // Find nearby trees
        for (int i = 0; i < _treeData.Count; i++)
        {
            float distSq =
                (playerPos - _treeData[i].position)
                .sqrMagnitude;

            if (distSq <= radiusSq)
            {
                shouldBeActive.Add(i);
            }
        }


        foreach (int i in shouldBeActive)
        {
            if (_activeColliders.ContainsKey(i))
                continue;

            TreeColliderData d =
                _treeData[i];

            GameObject go =
                new GameObject("TreeCollider");

            go.transform.SetPositionAndRotation(
                d.position,
                d.rotation);

            go.transform.localScale =
                d.scale;

            go.layer =
                LayerMask.NameToLayer("Tree");



            CapsuleCollider col =
                go.AddComponent<CapsuleCollider>();

            float colliderHeight = 10f * d.scale.y;
            float colliderRadius = 1f * d.scale.x;
            Vector3 colliderCenter =
                new Vector3(
                    0f,
                    colliderHeight / 2f,
                    0f);

            if (d.treeName.Contains("Rock1"))
            {
                colliderHeight = 6f * d.scale.y;
                colliderRadius = 4f * d.scale.x;

                colliderCenter =
                    new Vector3(
                        0f,
                        colliderHeight / 2f,
                        0f);
            }

            col.height = colliderHeight;
            col.radius = colliderRadius;
            col.center = colliderCenter;


            Tree treeScript =
                go.AddComponent<Tree>();

            treeScript.terrain =
                terrain;

            treeScript.treeIndex =
                d.treeIndex;

            if (d.treeName.Contains("PalmTree4"))
            {
                treeScript.treeType = "Tree";
                treeScript.health = 2;
                treeScript.drops = smallpalmTreeDrops;
            }
            else if (d.treeName.Contains("Palm"))
            {
                treeScript.treeType = "Tree";
                treeScript.health = 5;
                treeScript.drops = palmTreeDrops;
            }
            else if (d.treeName.Contains("Rock1"))
            {
                treeScript.treeType = "Rock";
                treeScript.Name = "Rock";
                treeScript.health = 20;
                treeScript.drops = RockDrops;
            }
            else if (d.treeName.Contains("Rock2"))
            {
                treeScript.treeType = "Rock";
                treeScript.health = 5;
                treeScript.drops = SmallRockDrops;
            }
            else if (d.treeName.Contains("BananaTree"))
            {
                treeScript.treeType = "Tree";
                treeScript.health = 10;
                treeScript.drops = BananaTreeDrops;
            }
            else
            {
                treeScript.treeType = "Tree";
                treeScript.health = 5;
                treeScript.drops =
                    oakTreeDrops;
            }

            _activeColliders[i] = go;
        }


        List<int> toRemove = new();

        foreach (var kvp in _activeColliders)
        {
            if (!shouldBeActive.Contains(kvp.Key))
            {
                Destroy(kvp.Value);

                toRemove.Add(kvp.Key);
            }
        }

        foreach (int i in toRemove)
        {
            _activeColliders.Remove(i);
        }
    }

    // ─────────────────────────────────────────────────────────────

    void OnDestroy()
    {
        foreach (var kvp in _activeColliders)
        {
            Destroy(kvp.Value);
        }
    }
}