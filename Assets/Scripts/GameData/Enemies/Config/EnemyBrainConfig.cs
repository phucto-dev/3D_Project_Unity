using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyBrain", menuName = "GameData/Enemies/Config/BrainConfig")]
public class EnemyBrainConfig : ScriptableObject
{
    [Header("--- SENSORS ---")]
    [SerializeField] private float _sightRange = 15f;
    public float SightRange => _sightRange;
    [SerializeField] private Vector3 _sightOffset = new Vector3(0, 1.5f, 0);
    public Vector3 SightOffset => _sightOffset;
    [SerializeField, Range(0, 360)] private float _fieldOfViewAngle = 110f;
    public float FieldOfViewAngle => _fieldOfViewAngle;

    [Header("--- PATROL ---")]
    [SerializeField] private float _patrolRadius = 10f;
    public float PatrolRadius => _patrolRadius;
    [SerializeField] private float _waitTimeAtWaypoint = 2f;
    public float WaitTimeAtWayPoint => _waitTimeAtWaypoint;

    [Header("--- CHASE ---")]
    [SerializeField] private float _attackRange = 2f;
    public float AttackRange => _attackRange;
    [SerializeField] private float _limitChaseRange = 30f;
    public float LimitChaseRange => _limitChaseRange;
    [SerializeField] private float _attackCooldown = 1.5f;
    public float AttackCooldown => _attackCooldown;
}
