using UnityEngine;

public interface ICapturable
{
    void Captured();
    void Thrown(Vector3 direction);
    GameObject GetRoot();
    Rigidbody GetRigidbody();
    void DisableCollision();
    void EnableCollision();
    bool CanCapture();
}
