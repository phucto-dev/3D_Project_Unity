using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SlotStatUIController : MonoBehaviour
{
    public StatsValue TextValue;
    public PlayerStatsSO playerStats;

    private TMP_Text _text;
    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        if (playerStats != null) playerStats.OnStatsChanged += UpdateText;
    }
    private void OnDisable()
    {
        if (playerStats != null) playerStats.OnStatsChanged -= UpdateText;
    }

    private void UpdateText(Dictionary<StatsValue, float> dictStats)
    {
        float textValue;
        textValue = dictStats[TextValue];
        _text.SetText(textValue.ToString());
    }
}
