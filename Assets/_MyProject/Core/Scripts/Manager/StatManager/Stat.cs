using System;
using System.Collections.Generic;
using UnityEngine;

public enum ModifyType
{
    Cheat,
    Positive,
    Negative,
}
[Serializable]
public class Stat
{
    [Header("--- DO NOT EDIT ---")]
    [SerializeField] private float _baseValue;

    private Dictionary<ModifyType, float> _modifiers = new Dictionary<ModifyType, float>();

    private Action _onValueChanged;

    public Stat(float value, Action onValueChanged = null)
    {
        _baseValue = value;
        _onValueChanged = onValueChanged;
    }

    public float GetValue()
    {
        float finalValue = _baseValue;
        if (_modifiers != null && _modifiers.Count > 0)
        {
            foreach (var mod in _modifiers)
            {
                finalValue += mod.Value;
            }
        }
        return finalValue;
    }

    public void AddModifier(ModifyType type, float value)
    {
        if (type == ModifyType.Cheat)
        {
            Debug.Log("_baseValue: " + _baseValue);
            _modifiers[type] = value;
        }
        else
        {
            _modifiers[type] += _modifiers.GetValueOrDefault(type, 0f) + value;
        }
        _onValueChanged?.Invoke();
    }
    public void RemoveModifier(ModifyType type, float value)
    {
        if (!_modifiers.ContainsKey(type)) return;
        if (type == ModifyType.Positive || type == ModifyType.Cheat) _modifiers[type] -= value;
        if (type == ModifyType.Negative) _modifiers[type] += Math.Abs(value);
        if (_modifiers[type] <= 0) _modifiers.Remove(type);
        _onValueChanged?.Invoke();
    }
}
