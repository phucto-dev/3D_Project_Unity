using UnityEngine;

public class LootDropSystem : MonoBehaviour
{
    [Header("--- REF ---")]
    [SerializeField] private LootAnimConfigSO _config;

    [Header("--- SETTINGS ---")]
    [SerializeField] private float _placeCastRayHeight = 5f; // Distance between ray and transform is 5 units
    [SerializeField] private float _distanceBetweenItemAndGround = 3.2f;
    [SerializeField] private LayerMask _groundLayer;

    private Vector3 _landedPosition;
    private bool _hasLanded;
    private bool _isDropping;
    private float _landedY;

    private void Start()
    {
        _hasLanded = false;
        _isDropping = true;
        _landedPosition = CalSafeDropPos(transform.position); // test
        Debug.Log(_landedPosition);
    }
    public void SpawnDrop(Vector3 startPos)
    {

    }

    private Vector3 CalSafeDropPos(Vector3 startPos)
    {
        Vector3 randCircleField = Random.insideUnitCircle * _config.DropRadius;
        Vector3 randPosDrop = startPos + new Vector3(randCircleField.x, 0, randCircleField.y);
        Vector3 rayOrigin = randPosDrop + Vector3.up * _placeCastRayHeight;
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 1000f, _groundLayer))
        {
            Debug.Log("In");
            return hit.point + Vector3.up * _distanceBetweenItemAndGround;
        }

        return randPosDrop;
    }

    private void Update()
    {
        if (_isDropping)
        {
            transform.position = Vector3.MoveTowards(transform.position, _landedPosition, Time.deltaTime * _config.DropSpeed);
            {
                if (transform.position == _landedPosition)
                {
                    _isDropping = false;
                    _hasLanded = true;
                    _landedY = transform.position.y;
                }
            }
        }
        else if (_hasLanded)
        {
            //transform.Rotate(Vector3.up, _config.RotationSpeed * Time.deltaTime);
            float newY = _landedY + Mathf.Sin(_config.BobbingSpeed * Time.time) * _config.BobbingHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
}
