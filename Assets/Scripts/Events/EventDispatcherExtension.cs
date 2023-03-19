using System;
using UnityEngine;

namespace Events
{
	public static class EventDispatcherExtension
	{
		/// Use for registering with EventsManager
		public static void SuscribeEvent (this MonoBehaviour listener, EventID eventID, Action<object> callback)
		{
			EventDispatcher.Instance.AddListener(eventID, callback);
		}
		
		public static void UnsubscribeEvent (this MonoBehaviour listener, EventID eventID, Action<object> callback)
		{
			EventDispatcher.Instance.RemoveListener(eventID, callback);
		}

		/// Post event with param
		public static void PublishEvent (this MonoBehaviour listener, EventID eventID, object param = null)
		{
			EventDispatcher.Instance.PublishEvent(eventID, param);
		}
	}
}
