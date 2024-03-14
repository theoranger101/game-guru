using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Events
{
	public class EventListener : MonoBehaviour
	{
		public string listenerName = "Unnamed Listener";

		public List<EventContext> events;

		public UnityEvent response;
		public float responseDelay = -1f;

		public void OnEnable()
		{
			if (Application.isPlaying)
			{
				foreach (var ev in events)
				{
					ev.AddListener<VoidEvent>(OnEventFired);
				}
			}
		}

		public void OnDisable()
		{
			foreach (var ev in events)
			{
				ev.RemoveListener<VoidEvent>(OnEventFired);
			}
		}

		public void OnEventFired(VoidEvent evt)
		{
			if (responseDelay <= 1e-2f)
			{
				response?.Invoke();
			}
			else
			{
				StartCoroutine(WaitForDelayAndFire(responseDelay));
			}
		}

		private IEnumerator WaitForDelayAndFire(float delay)
		{
			yield return new WaitForSeconds(delay);
			
			response?.Invoke();
		}

		public string GetName() => listenerName;
	}
}
