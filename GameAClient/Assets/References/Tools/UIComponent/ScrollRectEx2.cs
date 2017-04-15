using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ScrollRectEx2 : ScrollRectEx
{
	private bool _forceReleased = false;


	public bool isForceReleased {
		get { return _forceReleased; }
	}

	public void ForceReleaseAndDisableDrag () {
		_forceReleased = true;
	}
	public void EnableDrag () {
		_forceReleased = false;
	}
    
	public override void OnDrag (PointerEventData eventData)
	{
		if (_forceReleased) {
			eventData.Use ();
			return;
		}
		base.OnDrag (eventData);
		_onDragEvent.Invoke(eventData);
	}
}
