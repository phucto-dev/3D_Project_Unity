using System.Collections;
using UnityEngine;

public class SummonLightning : MonoBehaviour
{
    [Header("--- SETUP ---")]
    public GameObject LightningSkill;
    public SkillDataSO LightningData;
    [SerializeField] private float _delay;

    private Vector3 _spawnPosition;
    private PlayerStatsManager _playerStats;
    private PlayerSkill _playerSkill;
    private SkillVFXController _mainskill;

    private void Awake()
    {
        _mainskill = GetComponent<SkillVFXController>();
    }
    private void OnEnable()
    {
        _spawnPosition = this.transform.position;
        _mainskill.EndDuration += Summon;
        _mainskill.OnInit += GetPlayerSkillInfo;
    }
    private void OnDisable()
    {
        _mainskill.EndDuration -= Summon;
        _mainskill.OnInit -= GetPlayerSkillInfo;
    }
    public void Summon()
    {
        StartCoroutine(LightningDelay(_delay));
    }
    private IEnumerator LightningDelay(float time)
    {
        _playerSkill.ChangeState(SkillState.Smash);
        yield return new WaitForSeconds(time);
        GameObject vfxAOEInstance = Instantiate(LightningData.VFXPrefab, _spawnPosition, transform.rotation);
        vfxAOEInstance.GetComponent<SkillVFXController>().Initialize(LightningData, _playerStats, _playerSkill);
    }
    public void GetPlayerSkillInfo(PlayerSkill playerSkill, PlayerStatsManager playerStats)
    {
        _playerSkill = playerSkill;
        _playerStats = playerStats;
    }
}
