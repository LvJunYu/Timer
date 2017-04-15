/********************************************************************
** Filename : PositionSpringbackEffect  
** Author : ake
** Date : 5/5/2016 7:37:46 PM
** Summary : PositionSpringbackEffect  
***********************************************************************/


using System;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
	public class PositionSpringbackEffect:MonoBehaviour
	{
		public float InertiaFactor = 1;

		public float Resistance = 30;
		public const float TimePreFrame = 1f/30;
		public const float SpringbackCheckPrecision = 0.01f;
		public const float SpringbackDuringTime = 0.3f;

		private Transform _cachedTrans; 
		private Rect _validRect;
		private Rect _outerRect;

		private Rect _validMoveRect;

		private Rect _cameraViewRect;

		
		private float _startTime;

		private Vector2 _curSpeed;
		private Vector2 _acceleration;

		private Action<Vector2> _setFinalPosAction;
		private ESpingState _curState;
		private Vector2 _springbackAimPos;


		public enum ESpingState
		{
			None,
			InertiaMoving,
			Springback,
		}

		public void Init(Transform trans, Action<Vector2> action)
		{
			_cachedTrans = trans;
			_setFinalPosAction = action;
			_curState = ESpingState.None;
		}

		public void SetCameraViewRect(Rect rect, IntRect mapRect)
		{
            //Debug.Log ("SetCameraViewRect, rect: " + rect + " mapRect: " + mapRect);
			_cameraViewRect = rect;
            Rect vaildMapRect = GM2DTools.TileRectToWorldRect(mapRect);
			Vector3 center = vaildMapRect.center;


			vaildMapRect.width = vaildMapRect.width - _cameraViewRect.width;
			vaildMapRect.height = vaildMapRect.height - _cameraViewRect.height;
			vaildMapRect.center = center;

			_validRect = vaildMapRect;

			{
				_outerRect.width = rect.width * ConstDefineGM2D.CameraMoveExceedValueX;
				_outerRect.height = rect.height * ConstDefineGM2D.CameraMoveExceedValueY;
				_outerRect.width = _validRect.width + _outerRect.width;
				_outerRect.height = _validRect.height + _outerRect.height;
				_outerRect.center = center;

				_validMoveRect.width = rect.width * ConstDefineGM2D.CameraMoveOutSizeX;
				_validMoveRect.height = rect.height * (ConstDefineGM2D.CameraMoveOutSizeYTop + ConstDefineGM2D.CameraMoveOutSizeYBottom);
				_validMoveRect.width = _validRect.width + _validMoveRect.width;
				_validMoveRect.height = _validRect.height + _validMoveRect.height;
				_validMoveRect.center = new Vector2(center.x,center.y - rect.height*( ConstDefineGM2D.CameraMoveOutSizeYBottom - ConstDefineGM2D.CameraMoveOutSizeYTop) /2);
			}

		}

		public void UpdatePosOffset(Vector2 offset)
		{
			Vector2 curPos = _cachedTrans.position;
			curPos -= offset;
			curPos.x = Mathf.Clamp(curPos.x, _outerRect.xMin, _outerRect.xMax);
			curPos.y = Mathf.Clamp(curPos.y, _outerRect.yMin, _outerRect.yMax);
			_cachedTrans.position = curPos;
			SetFinalPos(curPos); 
			_curState = ESpingState.None;
		}


		private float _inertialMotionTime;

		public void OnDragEnd(Vector2 lastOffset)
		{
			if (lastOffset.magnitude < 0.01f)
			{
				Vector2 delta = GetSpringbackPosition(_cachedTrans.position);
				if (!NeedSpringback(delta))
				{
					_curState = ESpingState.None;
					return;
				}
				InitSpringbackState(delta);
				return;
			}
			Vector2 rel = lastOffset/ TimePreFrame* InertiaFactor;
			_inertialMotionTime = rel.magnitude/Resistance;
			_acceleration = -rel.normalized*Resistance;

			_curState = ESpingState.InertiaMoving;
			_curSpeed = rel;
			_startTime = Time.realtimeSinceStartup;
		}

		private void Update()
		{
			if(_curState == ESpingState.None)
			{
				return;
			}
			switch (_curState)
			{
				case ESpingState.InertiaMoving:
				{
					DoUpdateInertiaMoving();
					break;
				}
				case ESpingState.Springback:
				{
					DoUpdateSpringback();
					break;
				}
			}

		}

		private bool UpdateSpeed()
		{
			float curTime = Time.realtimeSinceStartup;
			if (curTime - _startTime > _inertialMotionTime)
			{
				_curSpeed = Vector2.zero;
				return false;
			}
			else
			{
				_curSpeed += (_acceleration*Time.deltaTime);
				return true;
			}
		}

		private Vector2 ClampedByOuterRect(Vector2 value)
		{
			Vector2 res;
			res.x = Mathf.Clamp(value.x, _outerRect.xMin, _outerRect.xMax);
			res.y = Mathf.Clamp(value.y, _outerRect.yMin, _outerRect.yMax);
			return res;
		}

		private void SetFinalPos(Vector2 pos)
		{
			if (_setFinalPosAction != null)
			{
				_setFinalPosAction(pos);
			}
		}

		private void DoUpdateInertiaMoving()
		{
			if (UpdateSpeed())
			{
				Vector2 offset = _curSpeed * Time.deltaTime;
				Vector3 pos = _cachedTrans.position - new Vector3(offset.x, offset.y);
				pos = ClampedByOuterRect(pos);
				Vector3 tmp = _cachedTrans.position - pos;
				if(tmp.magnitude>0.01f)
				{
					_cachedTrans.position = ClampedByOuterRect(pos);
					SetFinalPos(pos);
					return;
				}
			}
			Vector2 delta = GetSpringbackPosition(_cachedTrans.position);
			if (!NeedSpringback(delta))
			{
				_curState = ESpingState.None;
				return;
			}
			InitSpringbackState(delta);

		}

		private void DoUpdateSpringback()
		{
			float curTime = Time.realtimeSinceStartup;
			if (curTime - _startTime > SpringbackDuringTime)
			{
				_curState = ESpingState.None;
				_cachedTrans.position = _springbackAimPos;
				SetFinalPos(_springbackAimPos);
				return;
			}
			else
			{
				Vector3 pos = _cachedTrans.position;
				Vector2 offset = _curSpeed*Time.deltaTime;
				pos = pos - new Vector3(offset.x, offset.y);
				_cachedTrans.position = pos;
				SetFinalPos(pos);
			}
		}


		private Vector2 GetSpringbackPosition(Vector2 pos)
		{
			Vector2 res = Vector2.zero;
			if (pos.x < _validMoveRect.xMin)
			{
				res.x = pos.x - _validMoveRect.xMin;
			}
			else if(pos.x > _validMoveRect.xMax)
			{
				res.x = pos.x - _validMoveRect.xMax;
			}


			if (pos.y < _validMoveRect.yMin)
			{
				res.y = pos.y - _validMoveRect.yMin; 
			}
			else if (pos.y > _validMoveRect.yMax)
			{
				res.y = pos.y - _validMoveRect.yMax;
			}
			return res;
		}

		private bool NeedSpringback(Vector2 delta)
		{
			return delta.magnitude > SpringbackCheckPrecision;
		}

		private void InitSpringbackState(Vector2 delta)
		{
			_curState = ESpingState.Springback;
			_curSpeed = delta/SpringbackDuringTime;
			_startTime = Time.realtimeSinceStartup;
			_springbackAimPos = _cachedTrans.position - new Vector3(delta.x, delta.y);
		}
	}
}
