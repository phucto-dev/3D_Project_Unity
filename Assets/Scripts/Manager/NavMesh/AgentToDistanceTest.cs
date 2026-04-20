using UnityEngine;
using UnityEngine.AI;

public class AgentToDistanceTest : MonoBehaviour
{
    public Transform Target; // Nắm kéo GameObject mục tiêu vào đây trên Inspector
    public float TimeLoop;
    private NavMeshAgent _agent;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        if (_agent == null) return;
        // Kiểm tra xem mục tiêu có tồn tại không trước khi đi
        if (Target != null)
        {
            InvokeRepeating(nameof(SetDestinationF), -1, TimeLoop);
        }
    }

    private void SetDestinationF()
    {
        _agent.SetDestination(Target.position);
    }
}
