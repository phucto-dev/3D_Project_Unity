using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObjectMove : MonoBehaviour
{
    public GameObject m_gameObjectMain; // Phần hiển thị chính của viên đạn (Mesh/Sprite)
    public GameObject m_gameObjectTail; // Phần đuôi
    public float maxLength;
    public bool isDestroy;
    public float ObjectDestroyTime;
    public float TailDestroyTime;
    public float HitObjectDestroyTime;
    public float maxTime = 1;
    public float MoveSpeed = 10;
    public bool isCheckHitTag;
    public string mtag;
    public bool isShieldActive = false;
    public bool isHitMake = true;
    public PoolItemSO MyPoolVFX;
    public PoolItemSO HitPoolVFX;

    private string _myPoolID;
    private string _hitPoolID;
    private GameObject m_makedObject;
    private float _startTime;
    private bool _isHit;
    private float m_scalefactor;

    private void Start()
    {
        if (MyPoolVFX != null) _myPoolID = MyPoolVFX.poolID;
        if (HitPoolVFX != null) _hitPoolID = HitPoolVFX.poolID;
    }

    private void OnEnable()
    {
        _startTime = Time.time;
        _isHit = false;
        m_scalefactor = VariousEffectsScene.m_gaph_scenesizefactor;

        // 1. Đảm bảo phần hiển thị chính bật lên khi tái sử dụng
        if (m_gameObjectMain != null)
        {
            m_gameObjectMain.SetActive(true);
        }

        // 2. Gom Tail về lại và khởi động lại
        if (m_gameObjectTail != null)
        {
            m_gameObjectTail.transform.SetParent(this.transform);
            m_gameObjectTail.transform.localPosition = Vector3.zero;
            m_gameObjectTail.SetActive(true);
        }
    }

    private void LateUpdate()
    {
        if (_isHit) return; // Nếu đã chạm mục tiêu, ngưng update di chuyển

        transform.Translate(Vector3.forward * Time.deltaTime * MoveSpeed * m_scalefactor);

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxLength))
        {
            HitObj(hit);
        }
        else if (isDestroy && Time.time > _startTime + ObjectDestroyTime)
        {
            // Trường hợp bay hết tầm mà không trúng ai (Hết hạn)
            _isHit = true;
            MakeHitObject(transform);
            StartCoroutine(CleanupAndReleaseRoutine());
        }
    }

    private void MakeHitObject(RaycastHit hit)
    {
        if (!isHitMake || string.IsNullOrEmpty(_hitPoolID)) return;

        m_makedObject = PoolManager.Instance.Get(_hitPoolID);
        if (m_makedObject != null)
        {
            m_makedObject.transform.position = hit.point;
            m_makedObject.transform.rotation = Quaternion.LookRotation(hit.normal);
            m_makedObject.transform.parent = transform.parent;
            m_makedObject.transform.localScale = Vector3.one;
        }
    }

    private void MakeHitObject(Transform point)
    {
        if (!isHitMake || string.IsNullOrEmpty(_hitPoolID)) return;

        m_makedObject = PoolManager.Instance.Get(_hitPoolID);
        if (m_makedObject != null)
        {
            m_makedObject.transform.position = point.position;
            m_makedObject.transform.rotation = point.rotation;
            m_makedObject.transform.parent = transform.parent;
            m_makedObject.transform.localScale = Vector3.one;
        }
    }

    private void HitObj(RaycastHit hit)
    {
        if (isCheckHitTag && !hit.transform.CompareTag(mtag)) return;

        _isHit = true; // Khóa di chuyển

        if (m_gameObjectTail != null)
        {
            m_gameObjectTail.transform.parent = null; // Tách đuôi để đuôi không bị đi theo nếu cha di chuyển
        }

        MakeHitObject(hit);

        if (isShieldActive)
        {
            ShieldActivate m_sc = hit.transform.GetComponent<ShieldActivate>();
            if (m_sc != null) m_sc.AddHitObject(hit.point);
        }

        // Bắt đầu quy trình dọn dẹp thay vì ném vào pool ngay
        StartCoroutine(CleanupAndReleaseRoutine());
    }

    // Quy trình dọn rác và Pooling toàn diện
    private IEnumerator CleanupAndReleaseRoutine()
    {
        // 1. Tắt phần hiển thị chính (Mesh) -> Đánh lừa thị giác người chơi là đạn đã nổ/biến mất
        if (m_gameObjectMain != null)
        {
            m_gameObjectMain.SetActive(false);
        }

        // 2. Tính toán thời gian chờ lớn nhất để tất cả VFX chạy xong
        float waitTime = Mathf.Max(TailDestroyTime, HitObjectDestroyTime);
        yield return new WaitForSeconds(waitTime);

        // 3. Thu hồi hiệu ứng nổ (Hit Object) về Pool
        if (m_makedObject != null)
        {
            PoolManager.Instance.Release(_hitPoolID, m_makedObject);
            m_makedObject = null;
        }

        // 4. Thu hồi cái Đuôi (Tail) về làm con của viên đạn
        if (m_gameObjectTail != null)
        {
            m_gameObjectTail.transform.SetParent(this.transform);
            m_gameObjectTail.transform.localPosition = Vector3.zero;
        }

        // 5. Cuối cùng, trả toàn bộ viên đạn gốc này về Pool
        PoolManager.Instance.Release(_myPoolID, this.gameObject);
    }
    private void OnDisable()
    {
        // BẤT KỂ coroutine chạy xong hay bị script khác cắt ngang,
        // hễ viên đạn này bị tắt (thu hồi về pool), nó sẽ chạy hàm này.

        // 1. Dọn dẹp Hit Object nếu nó chưa được dọn
        if (m_makedObject != null)
        {
            PoolManager.Instance.Release(_hitPoolID, m_makedObject);
            m_makedObject = null;
        }

        // 2. Kéo lại đuôi đạn về nếu nó đang bị rớt ra ngoài
        if (m_gameObjectTail != null && m_gameObjectTail.transform.parent != this.transform)
        {
            m_gameObjectTail.transform.SetParent(this.transform);
            m_gameObjectTail.transform.localPosition = Vector3.zero;
            m_gameObjectTail.SetActive(false); // Đảm bảo đuôi tắt chờ lần bắn sau
        }
    }
}