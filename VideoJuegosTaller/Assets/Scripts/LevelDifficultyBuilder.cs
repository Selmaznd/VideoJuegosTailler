using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Runtime builder that enriches lvl3 and lvl4 with additional tiles and hazards.
/// It reuses existing scene content so we can author levels without touching prefabs manually.
/// </summary>
public class LevelDifficultyBuilder : MonoBehaviour
{
    [Header("Generation Settings")]
    [Tooltip("Optional explicit parent for the ground colour tiles.")]
    public Transform tileRootOverride;

    [Tooltip("Optional explicit parent for hazards; defaults to the tile root parent.")]
    public Transform hazardRootOverride;

    [Tooltip("Extra ground tiles to append for lvl3.")]
    public int level3ExtraTiles = 6;

    [Tooltip("Extra ground tiles to append for lvl4.")]
    public int level4ExtraTiles = 9;

    private readonly string[] _tileTags = { "PlatformPink", "PlatformYellow", "PlatformBlue" };
    private readonly Dictionary<string, GameObject> _tilePrototypes = new();

    private GameObject _hammerPrototype;
    private GameObject _winZone;

    private void Start()
    {
        var sceneName = SceneManager.GetActiveScene().name.ToLowerInvariant();

        CacheTilePrototypes();
        CacheHammerPrototype();
        CacheWinZone();

        switch (sceneName)
        {
            case "lvl3":
                BuildLevel3();
                break;
            case "lvl4":
                BuildLevel4();
                break;
            case "test":
                BuildTest();
                break;
        }
    }

    private void CacheTilePrototypes()
    {
        foreach (var tag in _tileTags)
        {
            if (_tilePrototypes.ContainsKey(tag) && _tilePrototypes[tag] != null)
                continue;

            var candidate = GameObject.FindGameObjectsWithTag(tag)
                .OrderBy(go => go.transform.position.y)
                .FirstOrDefault();

            if (candidate != null)
            {
                _tilePrototypes[tag] = candidate;
            }
            else
            {
                Debug.LogWarning($"[LevelDifficultyBuilder] Unable to locate prototype for tag {tag}.");
            }
        }
    }

    private void CacheHammerPrototype()
    {
        if (_hammerPrototype != null) return;

        _hammerPrototype = GameObject.FindGameObjectsWithTag("Hammer")
            .OrderByDescending(go => go.transform.position.y)
            .FirstOrDefault();

        if (_hammerPrototype == null)
        {
            Debug.LogWarning("[LevelDifficultyBuilder] Unable to locate hammer prototype in scene.");
        }
    }

    private void CacheWinZone()
    {
        if (_winZone != null) return;

        var candidates = GameObject.FindGameObjectsWithTag("Win");
        if (candidates != null && candidates.Length > 0)
        {
            _winZone = candidates[0];
        }
        else
        {
            Debug.LogWarning("[LevelDifficultyBuilder] Unable to locate Win zone in scene.");
        }
    }

    private Transform ResolveTileRoot()
    {
        if (tileRootOverride != null)
        {
            return tileRootOverride;
        }

        var candidate = GameObject.Find("rect_couleurs");
        return candidate != null ? candidate.transform : null;
    }

    private Transform ResolveHazardRoot(Transform tileRoot)
    {
        if (hazardRootOverride != null) return hazardRootOverride;
        if (tileRoot != null && tileRoot.parent != null) return tileRoot.parent;
        return tileRoot != null ? tileRoot : null;
    }

    private void BuildLevel3()
    {
        var tileRoot = ResolveTileRoot();
        if (tileRoot == null)
        {
            Debug.LogWarning("[LevelDifficultyBuilder] Missing tile root, lvl3 generation skipped.");
            return;
        }

        var walkway = CollectGroundTiles();
        if (walkway.Count < 2)
        {
            Debug.LogWarning("[LevelDifficultyBuilder] Not enough reference tiles to extend lvl3 walkway.");
            return;
        }

        var meta = AnalyseWalkway(walkway);
        var hazardRoot = ResolveHazardRoot(tileRoot);

        DecorateLevel3Path(walkway, tileRoot, hazardRoot, meta);

        var pattern = new[]
        {
            "PlatformYellow", "PlatformPink", "PlatformBlue",
            "PlatformPink", "PlatformYellow", "PlatformBlue"
        };

        var appendedTiles = ExtendWalkway(meta, pattern, level3ExtraTiles, tileRoot);

        for (int i = Mathf.Max(0, appendedTiles.Count - 2); i < appendedTiles.Count; i++)
        {
            var tile = appendedTiles[i];
            if (tile == null) continue;
            var pos = tile.transform.position;
            pos.y += 0.65f;
            tile.transform.position = pos;
        }

        if (appendedTiles.Count > 0)
        {
            SpawnSideTiles(tileRoot, meta, meta.TotalLength + meta.Spacing * 0.5f,
                Mathf.Min(appendedTiles.Count, 4), 2.1f,
                new[] { "PlatformBlue", "PlatformPink", "PlatformYellow" },
                "lvl3_ext");
        }

        var allTiles = new List<GameObject>(walkway);
        allTiles.AddRange(appendedTiles.Where(t => t != null));

        var extendedLength = meta.TotalLength + appendedTiles.Count * meta.Spacing;

        if (appendedTiles.Count > 0)
        {
            PlaceHammer(meta, hazardRoot, extendedLength - meta.Spacing * 0.4f, 1.25f);

            var finalLaser = CreateLaser(meta, hazardRoot, extendedLength - meta.Spacing * 0.15f, -1.6f, 1.2f, 4.8f);
            if (finalLaser != null)
            {
                finalLaser.onDuration = 1.4f;
                finalLaser.offDuration = 1.1f;
                finalLaser.startDelay = 0.4f;
            }
        }

        var lastTile = allTiles.LastOrDefault();
        if (lastTile != null)
        {
            MoveWinZone(lastTile.transform.position, meta.Direction, 0.45f);
        }
    }

    private void BuildLevel4()
    {
        var tileRoot = ResolveTileRoot();
        if (tileRoot == null)
        {
            Debug.LogWarning("[LevelDifficultyBuilder] Missing tile root, lvl4 generation skipped.");
            return;
        }

        var walkway = CollectGroundTiles();
        if (walkway.Count < 2)
        {
            Debug.LogWarning("[LevelDifficultyBuilder] Not enough reference tiles to extend lvl4 walkway.");
            return;
        }

        var meta = AnalyseWalkway(walkway);
        var hazardRoot = ResolveHazardRoot(tileRoot);

        DecorateLevel4Path(walkway, tileRoot, hazardRoot, meta);

        var pattern = new List<string>();
        for (int i = 0; i < level4ExtraTiles; i++)
        {
            pattern.Add(_tileTags[i % _tileTags.Length]);
        }

        var appended = ExtendWalkway(meta, pattern.ToArray(), level4ExtraTiles, tileRoot);

        for (int i = 0; i < appended.Count; i++)
        {
            var tile = appended[i];
            if (tile == null) continue;

            if (i % 2 == 1)
            {
                var timed = tile.AddComponent<TimedPlatform>();
                timed.visibleDuration = 2.5f;
                timed.hiddenDuration = 1.4f;
            }

            var pos = tile.transform.position;
            pos.y += 0.4f * (i + 1);
            tile.transform.position = pos;
        }

        if (appended.Count > 0)
        {
            SpawnSideTiles(tileRoot, meta, meta.TotalLength + meta.Spacing * 0.4f,
                Mathf.Min(appended.Count + 1, 5), -2.6f,
                new[] { "PlatformYellow", "PlatformPink", "PlatformBlue" },
                "lvl4_ext");
        }

        var allTiles = new List<GameObject>(walkway);
        allTiles.AddRange(appended.Where(t => t != null));

        var extendedLength = meta.TotalLength + appended.Count * meta.Spacing;

        if (appended.Count > 0)
        {
            var gateLaser = CreateLaser(meta, hazardRoot, extendedLength - meta.Spacing * 0.35f, 2.2f, 1.5f, 5.5f);
            if (gateLaser != null)
            {
                gateLaser.onDuration = 1.0f;
                gateLaser.offDuration = 0.9f;
                gateLaser.startDelay = 0.5f;
            }

            PlaceHammer(meta, hazardRoot, extendedLength - meta.Spacing * 0.75f, 1.8f);
            PlaceHammer(meta, hazardRoot, extendedLength - meta.Spacing * 0.1f, 2.2f);
        }

        var lastTile = allTiles.LastOrDefault();
        if (lastTile != null)
        {
            MoveWinZone(lastTile.transform.position, meta.Direction, 0.6f);
        }
    }

    private void BuildTest()
    {
        var tileRoot = ResolveTileRoot();
        if (tileRoot == null)
        {
            Debug.LogWarning("[LevelDifficultyBuilder] Missing tile root, test generation skipped.");
            return;
        }

        var walkway = CollectGroundTiles();
        if (walkway.Count < 1)
        {
            Debug.LogWarning("[LevelDifficultyBuilder] No base tiles found for test level.");
            return;
        }

        var meta = AnalyseWalkway(walkway);
        var forward = meta.Direction;
        var right = Vector3.Cross(Vector3.up, forward).normalized;
        if (right == Vector3.zero)
        {
            right = Vector3.right;
        }

        var startPosition = walkway[0].transform.position;
        var nodes = GenerateTestPath(startPosition, forward, right, meta.Spacing);
        if (nodes.Count == 0)
        {
            Debug.LogWarning("[LevelDifficultyBuilder] Test path generation produced no nodes.");
            return;
        }

        HideExistingWalkway(walkway);

        var createdTiles = new List<GameObject>();
        foreach (var node in nodes)
        {
            if (!_tilePrototypes.TryGetValue(node.Tag, out var prototype) || prototype == null)
            {
                Debug.LogWarning($"[LevelDifficultyBuilder] Missing prototype for tag {node.Tag} in test level.");
                continue;
            }

            var clone = Instantiate(prototype, node.Position, Quaternion.LookRotation(node.Forward, Vector3.up), tileRoot);
            clone.name = $"{prototype.name}_test_{createdTiles.Count}";
            clone.SetActive(true);

            if (node.Timed)
            {
                AddTimedPlatform(clone, node.VisibleDuration, node.HiddenDuration);
            }

            createdTiles.Add(clone);
        }

        var hazardRoot = ResolveHazardRoot(tileRoot);

        if (nodes.Count > 6)
        {
            CreateCrossLaser(nodes[6], hazardRoot, meta.Spacing * 4.2f, 1.3f, 1.4f, 1.0f, 0.25f);
        }

        if (nodes.Count > 12)
        {
            CreateCrossLaser(nodes[12], hazardRoot, meta.Spacing * 4.6f, 1.6f, 1.2f, 0.8f, 0.85f, invert: true);
        }

        if (nodes.Count > 18)
        {
            CreateAlongLaser(nodes[18], hazardRoot, meta.Spacing * 3.5f, meta.Spacing * 0.6f, 1.45f, 0.9f, 1.2f, 0.4f);
        }

        if (nodes.Count > 8)
        {
            SpawnHammerNear(nodes[8], hazardRoot, meta.Spacing, 0.3f, 0.9f, 1.4f);
        }

        if (nodes.Count > 20)
        {
            SpawnHammerNear(nodes[20], hazardRoot, meta.Spacing, 0.0f, -1.1f, 1.8f);
        }

        if (nodes.Count > 4)
        {
            SpawnDecorTile(nodes[4], tileRoot, meta.Spacing, 1.6f, 0f, "PlatformBlue");
            SpawnDecorTile(nodes[4], tileRoot, meta.Spacing, -1.6f, 0f, "PlatformPink");
        }

        if (nodes.Count > 10)
        {
            SpawnDecorTile(nodes[10], tileRoot, meta.Spacing, 1.2f, -0.25f, "PlatformYellow");
            SpawnDecorTile(nodes[10], tileRoot, meta.Spacing, -1.2f, -0.25f, "PlatformBlue", timed: true, visibleDuration: 2.2f, hiddenDuration: 1.0f);
        }

        if (nodes.Count > 16)
        {
            SpawnDecorTile(nodes[16], tileRoot, meta.Spacing, 1.8f, 0.15f, "PlatformPink", timed: true, visibleDuration: 1.9f, hiddenDuration: 0.8f);
            SpawnDecorTile(nodes[16], tileRoot, meta.Spacing, -1.8f, 0.15f, "PlatformYellow");
        }

        if (nodes.Count > 22)
        {
            SpawnDecorTile(nodes[22], tileRoot, meta.Spacing, 0f, 0.6f, "PlatformBlue");
            SpawnDecorTile(nodes[22], tileRoot, meta.Spacing, 1.2f, 0.35f, "PlatformPink");
            SpawnDecorTile(nodes[22], tileRoot, meta.Spacing, -1.2f, 0.35f, "PlatformYellow");
        }

        var lastNode = nodes[nodes.Count - 1];
        MoveWinZone(lastNode.Position, lastNode.Forward, 0.75f);
    }

    private List<GameObject> CollectGroundTiles()
    {
        var tiles = new List<GameObject>();
        foreach (var tag in _tileTags)
        {
            tiles.AddRange(GameObject.FindGameObjectsWithTag(tag));
        }

        if (tiles.Count == 0) return tiles;

        var minY = tiles.Min(t => t.transform.position.y);
        var groundTiles = tiles
            .Where(t => Mathf.Abs(t.transform.position.y - minY) < 1.0f)
            .OrderBy(t => t.transform.position.x)
            .ThenBy(t => t.transform.position.z)
            .ToList();

        // Sort along the main progression axis (projected on XZ).
        groundTiles.Sort((a, b) =>
        {
            var da = a.transform.position;
            var db = b.transform.position;
            if (Mathf.Abs(da.z - db.z) > 0.5f)
                return da.z.CompareTo(db.z);
            return da.x.CompareTo(db.x);
        });

        return groundTiles;
    }

    private WalkwayMeta AnalyseWalkway(IReadOnlyList<GameObject> tiles)
    {
        var start = tiles.First().transform.position;
        var end = tiles.Last().transform.position;

        var direction = end - start;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.01f)
        {
            direction = Vector3.forward;
        }
        else
        {
            direction.Normalize();
        }

        var spacings = new List<float>();
        for (int i = 1; i < tiles.Count; i++)
        {
            var delta = tiles[i].transform.position - tiles[i - 1].transform.position;
            delta.y = 0f;
            var projected = Vector3.Dot(delta, direction);
            if (projected > 0.1f)
            {
                spacings.Add(projected);
            }
        }

        var spacing = spacings.Count > 0 ? spacings.Average() : 3f;
        var totalLength = Vector3.Dot(end - start, direction);

        return new WalkwayMeta(start, direction, spacing, totalLength);
    }

    private List<GameObject> ExtendWalkway(WalkwayMeta meta, string[] pattern, int count, Transform tileRoot)
    {
        var created = new List<GameObject>();
        if (pattern == null || pattern.Length == 0) return created;

        var basePosition = meta.Start + meta.Direction * meta.TotalLength;
        var parent = tileRoot != null ? tileRoot : ResolveTileRoot();

        for (int i = 0; i < count; i++)
        {
            var tag = pattern[i % pattern.Length];
            if (!_tilePrototypes.TryGetValue(tag, out var prototype) || prototype == null)
            {
                Debug.LogWarning($"[LevelDifficultyBuilder] Skipping tile creation, no prototype for {tag}.");
                continue;
            }

            var offset = meta.Direction * meta.Spacing * (i + 1);
            var spawnPos = basePosition + offset;
            spawnPos.y = prototype.transform.position.y;

            var clone = Instantiate(prototype, spawnPos, prototype.transform.rotation, parent);
            clone.name = $"{prototype.name}_dynamic_{i}";
            created.Add(clone);
        }

        return created;
    }

    private void PlaceHammer(WalkwayMeta meta, Transform hazardRoot, float distanceAlong, float heightOffset)
    {
        if (_hammerPrototype == null) return;

        var spawnPos = meta.Start + meta.Direction * distanceAlong;
        spawnPos.y = _hammerPrototype.transform.position.y + heightOffset;

        var parent = hazardRoot != null ? hazardRoot : _hammerPrototype.transform.parent;
        var clone = Instantiate(_hammerPrototype, spawnPos, _hammerPrototype.transform.rotation, parent);
        clone.name = $"{_hammerPrototype.name}_dynamic";
    }

    private LaserHazard CreateLaser(WalkwayMeta meta, Transform hazardRoot, float distanceAlong, float sidewaysOffset, float height, float length)
    {
        var parent = hazardRoot != null ? hazardRoot : ResolveHazardRoot(null);
        if (parent == null) parent = transform;

        var lateral = Vector3.Cross(Vector3.up, meta.Direction).normalized;
        if (lateral == Vector3.zero)
        {
            lateral = Vector3.right;
        }

        var spawnPos = meta.Start + meta.Direction * distanceAlong + lateral * sidewaysOffset;
        spawnPos.y += height;

        var go = new GameObject("LaserHazard_dynamic");
        go.transform.SetPositionAndRotation(spawnPos, Quaternion.LookRotation(meta.Direction, Vector3.up));
        go.transform.SetParent(parent);

        var laser = go.AddComponent<LaserHazard>();
        laser.length = length;

        return laser;
    }

    private void DecorateLevel3Path(IReadOnlyList<GameObject> walkway, Transform tileRoot, Transform hazardRoot, WalkwayMeta meta)
    {
        if (walkway.Count >= 2)
        {
            ElevateTile(walkway[1], 0.45f);
        }

        if (walkway.Count >= 3)
        {
            var timedIndex = Mathf.Min(walkway.Count - 2, 3);
            AddTimedPlatform(walkway[timedIndex], 2.2f, 1.1f);
        }

        var midLaser = CreateLaser(meta, hazardRoot, meta.TotalLength * 0.45f, 1.7f, 1.1f, 4.2f);
        if (midLaser != null)
        {
            midLaser.onDuration = 1.5f;
            midLaser.offDuration = 1.2f;
        }

        SpawnSideTiles(tileRoot, meta, meta.TotalLength * 0.15f, 4, -2.0f,
            new[] { "PlatformPink", "PlatformYellow", "PlatformBlue", "PlatformYellow" },
            "lvl3_side");

        PlaceHammer(meta, hazardRoot, meta.TotalLength * 0.55f, 0.9f);
    }

    private void DecorateLevel4Path(IReadOnlyList<GameObject> walkway, Transform tileRoot, Transform hazardRoot, WalkwayMeta meta)
    {
        if (walkway.Count >= 3)
        {
            ElevateTile(walkway[2], 0.65f);
            AddTimedPlatform(walkway[1], 1.9f, 0.9f);
        }

        if (walkway.Count >= 5)
        {
            AddTimedPlatform(walkway[4], 2.5f, 1.2f);
        }

        var laser1 = CreateLaser(meta, hazardRoot, meta.TotalLength * 0.3f, 1.9f, 1.2f, 4.5f);
        if (laser1 != null)
        {
            laser1.onDuration = 1.1f;
            laser1.offDuration = 0.9f;
        }

        var laser2 = CreateLaser(meta, hazardRoot, meta.TotalLength * 0.6f, -1.8f, 1.4f, 4.8f);
        if (laser2 != null)
        {
            laser2.onDuration = 0.9f;
            laser2.offDuration = 1.6f;
            laser2.startDelay = 0.4f;
        }

        PlaceHammer(meta, hazardRoot, meta.TotalLength * 0.45f, 1.2f);

        SpawnSideTiles(tileRoot, meta, meta.TotalLength * 0.25f, 5, 2.7f,
            new[] { "PlatformBlue", "PlatformPink", "PlatformYellow", "PlatformBlue", "PlatformPink" },
            "lvl4_side");
    }

    private void ElevateTile(GameObject tile, float deltaY)
    {
        if (tile == null) return;

        var tileTransform = tile.transform;
        var pos = tileTransform.position;
        pos.y += deltaY;
        tileTransform.position = pos;
    }

    private void AddTimedPlatform(GameObject tile, float visibleDuration, float hiddenDuration)
    {
        if (tile == null) return;

        var timed = tile.GetComponent<TimedPlatform>() ?? tile.AddComponent<TimedPlatform>();
        timed.visibleDuration = visibleDuration;
        timed.hiddenDuration = hiddenDuration;
    }

    private void SpawnSideTiles(Transform parent, WalkwayMeta meta, float startDistance, int count, float lateralOffset,
        IReadOnlyList<string> pattern, string namePrefix = "side")
    {
        if (parent == null || count <= 0) return;

        IReadOnlyList<string> palette = (pattern != null && pattern.Count > 0)
            ? pattern
            : new List<string>(_tileTags);

        var lateral = Vector3.Cross(Vector3.up, meta.Direction).normalized;
        if (lateral == Vector3.zero)
        {
            lateral = Vector3.right;
        }

        for (int i = 0; i < count; i++)
        {
            var tag = palette[i % palette.Count];
            if (!_tilePrototypes.TryGetValue(tag, out var prototype) || prototype == null)
            {
                continue;
            }

            var distance = Mathf.Max(0f, startDistance + meta.Spacing * i);
            var spawnPos = meta.Start + meta.Direction * distance + lateral * lateralOffset;
            spawnPos.y = prototype.transform.position.y;

            var clone = Instantiate(prototype, spawnPos, prototype.transform.rotation, parent);
            clone.name = $"{prototype.name}_{namePrefix}_{i}";
        }
    }

    private List<PathNode> GenerateTestPath(Vector3 start, Vector3 forward, Vector3 right, float spacing)
    {
        var nodes = new List<PathNode>();
        Vector3 current = start;

        void AddStep(float forwardUnits, float rightUnits, float heightDelta, string tag,
            bool timed = false, float visible = 2.4f, float hidden = 1.1f)
        {
            current += forward * (spacing * forwardUnits) + right * (spacing * rightUnits) + Vector3.up * heightDelta;
            nodes.Add(new PathNode
            {
                Tag = tag,
                Position = current,
                Timed = timed,
                VisibleDuration = timed ? visible : 0f,
                HiddenDuration = timed ? hidden : 0f,
                Forward = forward
            });
        }

        nodes.Add(new PathNode
        {
            Tag = "PlatformPink",
            Position = current,
            Timed = false,
            VisibleDuration = 0f,
            HiddenDuration = 0f,
            Forward = forward
        });

        AddStep(1f, 0f, 0f, "PlatformYellow");
        AddStep(1f, 0f, 0f, "PlatformBlue");
        AddStep(1f, 0f, 0f, "PlatformPink");
        AddStep(1f, 0f, 0f, "PlatformYellow");

        AddStep(0.8f, 0.6f, 0.35f, "PlatformBlue", timed: true, visible: 2.6f, hidden: 1.2f);
        AddStep(0.7f, 0.6f, 0.25f, "PlatformPink");
        AddStep(0.7f, 0.6f, 0.2f, "PlatformYellow", timed: true, visible: 2.2f, hidden: 1.0f);

        AddStep(0.9f, 0.1f, 0.2f, "PlatformBlue");
        AddStep(0.8f, -0.6f, -0.05f, "PlatformPink");
        AddStep(0.8f, -0.6f, -0.05f, "PlatformYellow");
        AddStep(0.8f, -0.5f, -0.05f, "PlatformBlue", timed: true, visible: 2.0f, hidden: 0.9f);

        AddStep(0.6f, 0.9f, 0.35f, "PlatformPink");
        AddStep(0.6f, 0.8f, 0f, "PlatformYellow");
        AddStep(0.6f, -1.4f, -0.4f, "PlatformBlue", timed: true, visible: 1.8f, hidden: 0.8f);

        AddStep(1.0f, -0.2f, 0.2f, "PlatformPink");
        AddStep(1.0f, 0f, 0.45f, "PlatformYellow", timed: true, visible: 2.3f, hidden: 0.95f);
        AddStep(0.8f, 0.4f, 0.35f, "PlatformBlue");

        AddStep(0.8f, -0.6f, -0.2f, "PlatformPink");
        AddStep(0.8f, -0.5f, -0.2f, "PlatformYellow");

        AddStep(1.2f, 0.2f, 0.6f, "PlatformBlue", timed: true, visible: 2.1f, hidden: 0.9f);
        AddStep(0.9f, 0.6f, 0.1f, "PlatformPink");
        AddStep(0.9f, 0.6f, -0.15f, "PlatformYellow");
        AddStep(0.7f, 0f, 0.35f, "PlatformBlue");

        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 direction;
            if (i < nodes.Count - 1)
            {
                direction = (nodes[i + 1].Position - nodes[i].Position).normalized;
            }
            else if (i > 0)
            {
                direction = (nodes[i].Position - nodes[i - 1].Position).normalized;
            }
            else
            {
                direction = forward;
            }

            if (direction.sqrMagnitude < 0.001f)
            {
                direction = forward;
            }

            var node = nodes[i];
            node.Forward = direction;
            nodes[i] = node;
        }

        return nodes;
    }

    private void HideExistingWalkway(IEnumerable<GameObject> walkway)
    {
        foreach (var tile in walkway)
        {
            if (tile == null) continue;
            tile.SetActive(false);
        }
    }

    private LaserHazard CreateLaserGeneric(Vector3 origin, Vector3 forward, float length, Transform parent)
    {
        forward = forward.normalized;
        if (forward.sqrMagnitude < 0.001f)
        {
            forward = Vector3.forward;
        }

        var go = new GameObject("LaserHazard_dynamic");
        go.transform.SetParent(parent != null ? parent : transform);
        go.transform.position = origin;
        go.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);

        var laser = go.AddComponent<LaserHazard>();
        laser.length = length;
        return laser;
    }

    private void CreateCrossLaser(PathNode node, Transform hazardRoot, float length, float height,
        float onDuration, float offDuration, float delay = 0f, bool invert = false)
    {
        var lateral = Vector3.Cross(Vector3.up, node.Forward).normalized;
        if (lateral == Vector3.zero)
        {
            lateral = Vector3.right;
        }

        var direction = invert ? -lateral : lateral;
        var origin = node.Position - direction * (length * 0.5f);
        origin.y = node.Position.y + height;

        var laser = CreateLaserGeneric(origin, direction, length, hazardRoot);
        if (laser == null) return;

        laser.onDuration = onDuration;
        laser.offDuration = offDuration;
        laser.startDelay = Mathf.Max(0f, delay);
    }

    private void CreateAlongLaser(PathNode node, Transform hazardRoot, float length, float backOffset,
        float height, float onDuration, float offDuration, float delay = 0f)
    {
        var direction = node.Forward.normalized;
        if (direction.sqrMagnitude < 0.001f)
        {
            direction = Vector3.forward;
        }

        var origin = node.Position - direction * backOffset;
        origin.y = node.Position.y + height;

        var laser = CreateLaserGeneric(origin, direction, length, hazardRoot);
        if (laser == null) return;

        laser.onDuration = onDuration;
        laser.offDuration = offDuration;
        laser.startDelay = Mathf.Max(0f, delay);
    }

    private void SpawnHammerNear(PathNode node, Transform hazardRoot, float spacing, float forwardOffset,
        float lateralOffset, float heightOffset)
    {
        if (_hammerPrototype == null) return;

        var forwardDir = node.Forward.normalized;
        if (forwardDir.sqrMagnitude < 0.001f)
        {
            forwardDir = Vector3.forward;
        }

        var lateral = Vector3.Cross(Vector3.up, forwardDir).normalized;
        if (lateral == Vector3.zero)
        {
            lateral = Vector3.right;
        }

        var spawnPos = node.Position
                       + forwardDir * (spacing * forwardOffset)
                       + lateral * (spacing * lateralOffset);
        spawnPos.y = node.Position.y + heightOffset;

        var parent = hazardRoot != null ? hazardRoot : _hammerPrototype.transform.parent;
        var clone = Instantiate(_hammerPrototype, spawnPos, _hammerPrototype.transform.rotation, parent);
        clone.name = $"{_hammerPrototype.name}_test";
    }

    private void SpawnDecorTile(PathNode node, Transform parent, float spacing, float lateralOffsetUnits,
        float heightOffset, string colorTag, bool timed = false, float visibleDuration = 2f, float hiddenDuration = 1f)
    {
        if (!_tilePrototypes.TryGetValue(colorTag, out var prototype) || prototype == null) return;
        if (parent == null) return;

        var forwardDir = node.Forward.normalized;
        if (forwardDir.sqrMagnitude < 0.001f)
        {
            forwardDir = Vector3.forward;
        }

        var lateral = Vector3.Cross(Vector3.up, forwardDir).normalized;
        if (lateral == Vector3.zero)
        {
            lateral = Vector3.right;
        }

        var position = node.Position + lateral * (spacing * lateralOffsetUnits) + Vector3.up * heightOffset;
        var rotation = Quaternion.LookRotation(forwardDir, Vector3.up);

        var clone = Instantiate(prototype, position, rotation, parent);
        clone.name = $"{prototype.name}_testDecor";
        clone.SetActive(true);

        if (timed)
        {
            AddTimedPlatform(clone, visibleDuration, hiddenDuration);
        }
    }

    private void MoveWinZone(Vector3 targetPosition, Vector3 forward, float heightOffset)
    {
        if (_winZone == null) return;

        forward.y = 0f;
        if (forward.sqrMagnitude < 0.001f)
        {
            forward = Vector3.forward;
        }

        targetPosition.y += heightOffset;

        _winZone.transform.position = targetPosition;
        _winZone.transform.rotation = Quaternion.LookRotation(forward.normalized, Vector3.up);
    }

    private struct WalkwayMeta
    {
        public Vector3 Start;
        public Vector3 Direction;
        public float Spacing;
        public float TotalLength;

        public WalkwayMeta(Vector3 start, Vector3 direction, float spacing, float totalLength)
        {
            Start = start;
            Direction = direction;
            Spacing = spacing;
            TotalLength = totalLength;
        }
    }

    private struct PathNode
    {
        public string Tag;
        public Vector3 Position;
        public bool Timed;
        public float VisibleDuration;
        public float HiddenDuration;
        public Vector3 Forward;
    }
}
