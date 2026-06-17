using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PullEntityInRange : MonoBehaviour
{
    [Header("--- SETUP ---")]
    public LayerMask LayerTarget;
    public float PullForce;
    public float TickPull;

    private Dictionary<Rigidbody, PullTarget> _targetDict = new Dictionary<Rigidbody, PullTarget>();
    private SkillVFXController controller;
    private float _tickTimer;

    private class PullTarget
    {
        public Rigidbody Rb;
        public NavMeshAgent Agent;

        public PullTarget(Rigidbody rb, NavMeshAgent agent)
        {
            Rb = rb;
            Agent = agent;
        }
    }

    private void Awake()
    {
        controller = GetComponentInParent<SkillVFXController>();
    }

    private void OnEnable()
    {
        _tickTimer = TickPull;
        if (controller != null)
            controller.EndDuration += EndPullEffect;
    }

    private void OnDisable()
    {
        if (controller != null)
            controller.EndDuration -= EndPullEffect;
    }

    private void FixedUpdate()
    {
        if (TickPull <= 0) return;

        _tickTimer -= Time.fixedDeltaTime;
        if (_tickTimer <= 0)
        {
            if (_targetDict.Count <= 0) return;

            foreach (var kvp in _targetDict)
            {
                ApplyPullEffect(kvp.Value.Agent, kvp.Value.Rb, transform.parent.position);
            }
            _tickTimer = TickPull;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & LayerTarget) != 0)
        {
            Rigidbody rb = other.GetComponentInParent<Rigidbody>();
            if (rb != null && !_targetDict.ContainsKey(rb))
            {
                NavMeshAgent agent = rb.GetComponent<NavMeshAgent>();
                _targetDict.Add(rb, new PullTarget(rb, agent));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.GetComponentInParent<Rigidbody>();
        if (rb != null)
        {
            if (_targetDict.TryGetValue(rb, out PullTarget target))
            {
                Debug.Log("Sao exit ?");
                RestoreAgent(target);
                _targetDict.Remove(rb);
            }
        }
    }

    private void ApplyPullEffect(NavMeshAgent agent, Rigidbody rb, Vector3 targetPos)
    {
        if (rb == null) return;
        if (agent != null && agent.enabled)
        {
            agent.enabled = false;
            rb.isKinematic = false;
        }

        Vector3 dir = (targetPos - rb.transform.position).normalized;
        rb.AddForce(dir * PullForce, ForceMode.Force);
    }

    private void EndPullEffect()
    {
        foreach (var target in _targetDict.Values)
        {
            RestoreAgent(target);
        }
        _targetDict.Clear();
    }

    private void RestoreAgent(PullTarget target)
    {
        if (target.Agent != null)
        {
            target.Agent.enabled = true;
            target.Agent.Warp(target.Rb.transform.position);
        }
        if (target.Rb != null)
        {
            target.Rb.isKinematic = true;
        }
    }
}