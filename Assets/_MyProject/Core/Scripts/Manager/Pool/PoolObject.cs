using UnityEngine;

public class PoolObject : MonoBehaviour
{
    private string _myPoolID;

    public void Setup(string poolID)
    {
        _myPoolID = poolID;
    }

    public void ReturnToPool()
    {
        PoolManager.Instance.Release(_myPoolID, this.gameObject);
    }
}
