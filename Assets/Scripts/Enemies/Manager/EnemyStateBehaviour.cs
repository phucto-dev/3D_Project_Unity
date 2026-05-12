using UnityEngine;

public class EnemyStateBehaviour : StateMachineBehaviour
{
    public float DissolveTime = 2f;

    private readonly int _dissolveID = Shader.PropertyToID("_DissolveAmount");

    private SkinnedMeshRenderer _skinRenderer;
    private Material _mat;
    private float _elapsedTime;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _elapsedTime = 0f;

        if (_skinRenderer == null)
        {
            _skinRenderer = animator.GetComponentInChildren<SkinnedMeshRenderer>();
            if (_skinRenderer != null)
            {
                _mat = _skinRenderer.material;
            }
        }

        if (_mat != null)
        {
            _mat.SetFloat(_dissolveID, 0f);
        }
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_mat == null) return;
        if (stateInfo.normalizedTime <= 2f) return;

        _elapsedTime += Time.deltaTime;
        float dissolveProgress = _elapsedTime / DissolveTime;
        dissolveProgress = Mathf.Clamp01(dissolveProgress);
        _mat.SetFloat(_dissolveID, dissolveProgress);

        if (dissolveProgress >= 1f)
        {
            _mat.SetFloat(_dissolveID, 0f);
            animator.transform.parent.gameObject.SetActive(false);
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_mat != null)
        {
            _mat.SetFloat(_dissolveID, 0f);
        }
    }
}
