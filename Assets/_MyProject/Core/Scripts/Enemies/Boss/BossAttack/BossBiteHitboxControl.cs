using UnityEngine;

public class BossBiteHitboxControl : MonoBehaviour
{
    private BoxCollider _hitbox;
    private BossStatsManager _bossStatsManager;
    private DmgInfo _dmgInfo;
    private float _dmgMultiplier;
    private void Awake()
    {
        _hitbox = GetComponent<BoxCollider>();
        _bossStatsManager = GetComponentInParent<BossStatsManager>();
    }
    private void OnEnable()
    {
        if (_hitbox.enabled)
        {
            _hitbox.enabled = false;
        }
        _dmgInfo = new DmgInfo
        {
            Amount = _bossStatsManager.AttackPower.GetValue(),
            Dealer = this.transform.parent,
            PoiseDamage = _bossStatsManager.PoiseDamage.GetValue(),
            IsCritical = false
        };
    }
    private void OnDisable()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagConstant.TagPlayer))
        {
            HealthSystem playerHealth = other.GetComponent<HealthSystem>();

            if (playerHealth != null)
            {
                Debug.Log("Deal: " + _dmgInfo.Amount);
                SetCurrentDmgInfo();
                playerHealth.TakeDmg(_dmgInfo);
            }
        }
    }
    public void OpenHitbox(BossCombatInfo combatInfo)
    {
        if (_hitbox != null) _hitbox.enabled = true;
        _dmgMultiplier = combatInfo.DmgHitMultiple;
    }
    public void CloseHitbox()
    {
        if (_hitbox != null) _hitbox.enabled = false;
    }
    private void SetCurrentDmgInfo()
    {
        _dmgInfo.Amount = _bossStatsManager.AttackPower.GetValue() * _dmgMultiplier;
        _dmgInfo.PoiseDamage = _bossStatsManager.PoiseDamage.GetValue();
    }
}
