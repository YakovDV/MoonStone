using System.Collections;
using UnityEngine;

public class ResourceUnloader : MonoBehaviour
{
    [SerializeField] private ResourceStorage _storage;
    [SerializeField] private float _checkFrequency = 0.2f;
    [SerializeField] private LayerMask _receiverMask;
    [SerializeField] private BoxCollider _checkZone;

    private readonly Collider[] _hits = new Collider[8];

    private Coroutine _findReceiverCoroutine;

    private void OnEnable()
    {
        if (_findReceiverCoroutine != null)
        {
            StopCoroutine(_findReceiverCoroutine);
            _findReceiverCoroutine = null;
        }

        _findReceiverCoroutine = StartCoroutine(UnloadRoutine());
    }

    private void OnDisable()
    {
        if (_findReceiverCoroutine != null)
        {
            StopCoroutine(_findReceiverCoroutine);
            _findReceiverCoroutine = null;
        }
    }

    private IEnumerator UnloadRoutine()
    {
        WaitForSeconds wait = new(_checkFrequency);

        while (enabled)
        {
            if (_storage.CurrentResourcesMass <= 0)
            {
                yield return wait;
                continue;
            }

            if (TryFindReceiver(out IResourceReceiver receiver) == true)
                if (receiver.Receive(_storage))
                {
                    //Place for VFX or smthn
                }

            yield return wait;
        }
    }

    private bool TryFindReceiver(out IResourceReceiver receiver)
    {
        receiver = null;

        Vector3 center = _checkZone.transform.TransformPoint(_checkZone.center);
        Vector3 scale = _checkZone.transform.lossyScale;
        Vector3 halfExtents = Vector3.Scale(_checkZone.size * 0.5f, new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z)));

        int count = Physics.OverlapBoxNonAlloc(center, halfExtents, _hits, _checkZone.transform.rotation, _receiverMask, QueryTriggerInteraction.Collide);

        for (int i = 0; i < count; i++)
        {
            if (_hits[i].TryGetComponent(out IResourceReceiver resourceReceiver) == false)
                continue;

            receiver = resourceReceiver;
            return true;
        }

        return false;
    }
}