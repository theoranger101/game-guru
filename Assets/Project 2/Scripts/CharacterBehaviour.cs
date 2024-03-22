using Core;
using DG.Tweening;
using Events;
using UnityEngine;

public class CharacterBehaviour : MonoBehaviour
{
    [SerializeField]
    private Rigidbody m_Rigidbody;
    

#if UNITY_EDITOR
    private void OnValidate()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }
#endif
    
    private void OnEnable()
    {
        GEM.AddListener<GameEvent>(OnStartLevel, channel:(int)GameEventType.Start);
        
        GEM.AddListener<GameEvent>(OnStartMovement, channel: (int)GameEventType.Success);
        GEM.AddListener<GameEvent>(OnStartMovement, channel: (int)GameEventType.Fail);
    }
    
    private void OnDisable()
    {
        GEM.RemoveListener<GameEvent>(OnStartLevel, channel:(int)GameEventType.Start);
        
        GEM.RemoveListener<GameEvent>(OnStartMovement, channel: (int)GameEventType.Success);
        GEM.RemoveListener<GameEvent>(OnStartMovement, channel: (int)GameEventType.Fail);
    }

    private void OnStartLevel(GameEvent evt)
    {
        transform.position = evt.CharacterStartPosition;
        m_Rigidbody.useGravity = true;
    }
    
    private void OnStartMovement(GameEvent evt)
    {
        m_Rigidbody.DOPath(evt.CharacterPath, evt.PathDuration).SetEase(Ease.Linear);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != 6)
            return;

        Debug.Log("Start dancing");
        
    }

    private void OnTriggerEnter(Collider other)
    {
    }
}