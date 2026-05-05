using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat
{
    [Header("--- DO NOT EDIT ---")]
    [SerializeField] private float _baseValue;

    private List<float> _modifiers = new List<float>();

    public event Action OnStatChanged;

    public Stat(float value)
    {
        _baseValue = value;
    }

    public float GetValue()
    {
        float finalValue = _baseValue;
        foreach (float mod in _modifiers)
        {
            finalValue += mod;
        }

        return finalValue;
    }

    public void AddModifier(float mod)
    { 
        _modifiers.Add(mod);
        OnStatChanged?.Invoke();
    }
    public void RemoveModifier(float mod)
    {
        _modifiers.Remove(mod);
        OnStatChanged?.Invoke();
    }
}
