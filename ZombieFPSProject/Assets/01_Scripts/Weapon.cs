using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _muzzle;

    public float rayDistance = 10f;
    public float _fireRate = 0.2f;

    private bool _isAttack = false;

    private Ray _camRay;

    void Update()
    {
        _camRay = Camera.main.ScreenPointToRay(Vector3.forward * rayDistance);
        if (Input.GetKey(KeyCode.Mouse0) && _isAttack == false)
        {
            StartCoroutine(Shoot());
        }
    }

    private IEnumerator Shoot()
    {
        _isAttack = true;

        GameObject bulletObj = Instantiate(_bulletPrefab);
        bulletObj.transform.position = _muzzle.position;
        bulletObj.transform.rotation = _muzzle.rotation;
        yield return new WaitForSeconds(_fireRate);

        _isAttack = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(_camRay);
    }
}
