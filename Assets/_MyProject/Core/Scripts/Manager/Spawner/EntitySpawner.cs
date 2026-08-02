using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EntitySpawnInfo
{
    public Vector3 SpawnPos;
    public float PatrolRadius;
    public float LeashRadius;

    public EntitySpawnInfo()
    {

    }

    public EntitySpawnInfo(Vector3 pos, float patrolRadius, float leashRadius)
    {
        SpawnPos = pos;
        PatrolRadius = patrolRadius;
        LeashRadius = leashRadius;
    }
}

[Serializable]
public class EntitySpawnData
{
    public PoolItemSO Target;
    public int MaxNumber;
}

[RequireComponent(typeof(Collider))]
public class EntitySpawner : MonoBehaviour
{
    [Header("--- SPAWNER SETTINGS ---")]
    [SerializeField] private List<EntitySpawnData> _poolList;

    [Header("--- ZONES SETTINGS ---")]
    [SerializeField] private float _patrolRadius;
    [SerializeField] private float _limitRadius;

    private List<KeyValuePair<string, GameObject>> _activeEntities = new List<KeyValuePair<string, GameObject>>();
    private Collider _triggerCollider; // also range spawn
    private EntitySpawnInfo _spawnInfo;
    private bool _isPlayerInZone;
    private bool _despawnAble;

    private void Reset()
    {
        _triggerCollider = GetComponent<Collider>();

        if (_triggerCollider != null)
        {
            _triggerCollider.isTrigger = true;
            // _triggerCollider.radius = _monsterData.SpawnRadius;
        }
    }

    private void Awake()
    {
        _triggerCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        _spawnInfo = new EntitySpawnInfo(transform.position, _patrolRadius, _limitRadius);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagConstant.TagPlayer))
        {
            _isPlayerInZone = true;
            if (_activeEntities.Count == 0)
            {
                SpawnEntities();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(TagConstant.TagPlayer))
        {
            _isPlayerInZone = false;
            TryDespawn();
        }
    }
    private void SpawnEntities()
    {
        if (_poolList == null || _poolList.Count == 0) return;
        foreach (EntitySpawnData poolEntity in _poolList)
        {
            if (poolEntity == null || poolEntity.Target == null || poolEntity.MaxNumber == 0) continue;
            for (var i = 0; i < poolEntity.MaxNumber; i++)
            {
                
                GameObject entityObj = PoolManager.Instance.Get(poolEntity.Target.poolID);

                var agent = entityObj.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (agent != null) agent.enabled = false;

                entityObj.transform.position = transform.position;

                if (agent != null)
                {
                    agent.Warp(entityObj.transform.position);
                    agent.enabled = true;
                }

                EnemyStateManager state = entityObj.GetComponent<EnemyStateManager>();

                if (state != null)
                {
                    state.SetSpawnInfo(_spawnInfo);
                    state.PingDespawnSignal += SetDespawnAble;
                    state.DoTryDespawn += TryDespawn;
                    state.PingDeath += HandleEntityDeath;
                }

                entityObj.SetActive(true);
                ResetEntity(entityObj);

                KeyValuePair<string, GameObject> entityDict = new KeyValuePair<string, GameObject>(poolEntity.Target.poolID, entityObj);
                _activeEntities.Add(entityDict);
            }
        }
    }
    public void TryDespawn(bool force = false)
    {
        if (force)
        {
            DespawnEntities();
            return;
        }

        if (!_despawnAble) return;
        if (_isPlayerInZone) return;

        bool allEntitiesHome = true;

        foreach (KeyValuePair<string, GameObject> entity in _activeEntities)
        {
            if (entity.Value.activeInHierarchy)
            {
                float distance = Vector3.Distance(transform.position, entity.Value.transform.position);
                if (distance > _patrolRadius + 1.5f)
                {
                    allEntitiesHome = false;
                    break;
                }
            }
        }
        if (allEntitiesHome)
        {
            DespawnEntities();
        }
    }
    private void DespawnEntities()
    {
        foreach (var entity in _activeEntities)
        {
            if (entity.Value.activeInHierarchy)
            {
                EnemyStateManager state = entity.Value.GetComponent<EnemyStateManager>();

                if (state != null)
                {
                    state.PingDespawnSignal -= SetDespawnAble;
                    state.DoTryDespawn -= TryDespawn;
                    state.PingDeath -= HandleEntityDeath;
                }
                PoolManager.Instance.Release(entity.Key, entity.Value);
            }
        }
        _activeEntities.Clear();
    }
    private void ResetEntity(GameObject entityObj)
    {
        EnemyStateManager state = entityObj.GetComponent<EnemyStateManager>();
        Animator animator = entityObj.GetComponentInChildren<Animator>();
        HealthSystem health = entityObj.GetComponentInChildren<HealthSystem>();

        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }
        if (health != null)
        {
            health.ResetHP();
        }
        if (state != null)
        {
            state.ChangeState(new PatrolState());
        }
    }
    private void SetDespawnAble(bool value)
    {
        _despawnAble = value;
    }
    private void HandleEntityDeath(GameObject target)
    {
        foreach (KeyValuePair<string, GameObject> activeEntity in _activeEntities)
        {
            if (activeEntity.Value == target)
            {
                _activeEntities.Remove(activeEntity);
                break;
            }
        }
    }
}
