using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class CameraPositionSpringbackEffect
    {
		public const float InertiaFactor = 1;

		public const float Resistance = 30;
		public const float TimePreFrame = 1f/30;
		public const float SpringbackCheckPrecision = 0.01f;
        public const float SmallDistance = 0.01f;
		public const float SpringbackDuringTime = 0.3f;

	    private CameraManager _cameraManager;
        private Rect _validMapRect;
        private Rect _outerRect;
        private Rect _validMoveRect;
	    private Vector2 _cameraViewHalfSizeFactor;
	    private float _orthoSize;
		
		private float _startTime;
		private Vector2 _curSpeed;
		private Vector2 _acceleration;
	    
	    private float _inertialMotionTime;

		private ESpingState _curState;
		private Vector2 _springbackAimPos;

        /// <summary>
        /// 差值移动目标坐标
        /// </summary>
        private Vector2 _lerpTargetPos;

		public enum ESpingState
		{
			None,
			InertiaMove,
			Springback,
            LerpMove,
		}

		public void Init(CameraManager cameraManager)
		{
			_cameraManager = cameraManager;
			_curState = ESpingState.None;
			
            _validMapRect = GM2DTools.TileRectToWorldRect(DataScene2D.Instance.ValidMapRect);
			_cameraViewHalfSizeFactor = new Vector2(GM2DGame.Instance.GameScreenAspectRatio, 1f);
			_orthoSize = _cameraManager.RendererCamera.orthographicSize;
			CalcMoveRect();
		}

	    public void SetPos(Vector2 pos)
	    {
		    pos = ClampValidMoveRect(pos);
		    _cameraManager.MainCamaraPos = pos;
		    _curState = ESpingState.None;
	    }
	    
	    public void LerpPos (Vector2 pos) {
		    _lerpTargetPos = ClampValidMoveRect(pos);
		    _curState = ESpingState.LerpMove;
	    }

		public void MovePos(Vector2 offset)
		{
			Vector2 curPos = _cameraManager.MainCamaraPos;
			curPos -= offset;
			curPos.x = Mathf.Clamp(curPos.x, _outerRect.xMin, _outerRect.xMax);
			curPos.y = Mathf.Clamp(curPos.y, _outerRect.yMin, _outerRect.yMax);
			_cameraManager.MainCamaraPos = curPos;
			_curState = ESpingState.None;
		}

		public void MovePosEnd(Vector2 lastOffset)
		{
            if (lastOffset.magnitude < SmallDistance)
			{
				TrySpringback();
				return;
			}
			Vector2 rel = lastOffset/ TimePreFrame* InertiaFactor;
			_inertialMotionTime = rel.magnitude/Resistance;
			_acceleration = -rel.normalized*Resistance;

			_curState = ESpingState.InertiaMove;
			_curSpeed = rel;
			_startTime = Time.realtimeSinceStartup;
		}

	    public void OnOrthoSizeChange(float orthoSize)
	    {
		    _orthoSize = orthoSize;
		    CalcMoveRect();
		    if (ESpingState.None == _curState)
		    {
			    Vector2 pos = _cameraManager.MainCamaraPos;
			    pos = ClampValidMoveRect(pos);
			    _cameraManager.MainCamaraPos = pos;
		    }
		    else if (ESpingState.LerpMove == _curState)
		    {
			    _lerpTargetPos = ClampValidMoveRect(_lerpTargetPos);
		    }
		    else if (ESpingState.Springback == _curState)
		    {
			    _springbackAimPos = ClampValidMoveRect(_springbackAimPos);
		    }
	    }

		public void Update()
		{
			if(_curState == ESpingState.None)
			{
				return;
			}
			switch (_curState)
			{
				case ESpingState.InertiaMove:
				{
					DoUpdateInertiaMove();
					break;
				}
				case ESpingState.Springback:
				{
					DoUpdateSpringback();
					break;
				}
            case ESpingState.LerpMove:
                {
                    DoUpdateLerpMove ();
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
				_curSpeed += (_acceleration * Time.deltaTime);
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

        private void DoUpdateLerpMove () {
            Vector2 v = _lerpTargetPos - (Vector2)_cameraManager.MainCamaraPos;
            if (v.sqrMagnitude < SmallDistance * 0.25f) {
	            _cameraManager.MainCamaraPos = _lerpTargetPos;
                _curState = ESpingState.None;
                return;
            }
	        _cameraManager.MainCamaraPos = Vector3.Lerp(_cameraManager.MainCamaraPos, _lerpTargetPos, Time.deltaTime * 6);
        }

		private void DoUpdateInertiaMove()
		{
			if (UpdateSpeed())
			{
				Vector2 offset = _curSpeed * Time.deltaTime;
				Vector3 pos = _cameraManager.MainCamaraPos - new Vector3(offset.x, offset.y);
				pos = ClampedByOuterRect(pos);
				Vector3 tmp = _cameraManager.MainCamaraPos - pos;
                if(tmp.magnitude>SmallDistance)
				{
					_cameraManager.MainCamaraPos = ClampedByOuterRect(pos);
					return;
				}
			}
			TrySpringback();
		}

		private void DoUpdateSpringback()
		{
			float curTime = Time.realtimeSinceStartup;
			if (curTime - _startTime > SpringbackDuringTime)
			{
				_curState = ESpingState.None;
				_cameraManager.MainCamaraPos = _springbackAimPos;
			}
			else
			{
				Vector3 pos = _cameraManager.MainCamaraPos;
				Vector2 offset = _curSpeed*Time.deltaTime;
				pos = pos - new Vector3(offset.x, offset.y);
				_cameraManager.MainCamaraPos = pos;
			}
		}


		private Vector2 GetSpringbackDelta(Vector2 pos)
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

        private Vector2 ClampValidMoveRect (Vector2 pos) {
	        pos.x = Mathf.Clamp(pos.x, _validMoveRect.xMin, _validMoveRect.xMax);
	        pos.y = Mathf.Clamp(pos.y, _validMoveRect.yMin, _validMoveRect.yMax);
            return pos;
        }

	    private void TrySpringback()
	    {
		    Vector2 delta = GetSpringbackDelta(_cameraManager.MainCamaraPos);
		    if (!NeedSpringback(delta))
		    {
			    _curState = ESpingState.None;
			    return;
		    }
		    InitSpringbackState(delta);
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
			_springbackAimPos = _cameraManager.MainCameraTrans.position - new Vector3(delta.x, delta.y);
		}

	    private void CalcMoveRect()
	    {
		    Vector2 cameraHalfSize = _cameraViewHalfSizeFactor * _orthoSize;
		    _outerRect.width = _validMapRect.width + cameraHalfSize.x * 2 * (ConstDefineGM2D.CameraMoveExceedValueX - 1);
		    _outerRect.height = _validMapRect.height + cameraHalfSize.y * 2 * (ConstDefineGM2D.CameraMoveExceedValueY - 1);
		    _outerRect.center = _validMapRect.center;

		    _validMoveRect.width = _validMapRect.width + cameraHalfSize.x * 2 * (ConstDefineGM2D.CameraMoveOutSizeX - 1);
		    _validMoveRect.height = _validMapRect.height + cameraHalfSize.y * 2 *
		                            (ConstDefineGM2D.CameraMoveOutSizeYTop + ConstDefineGM2D.CameraMoveOutSizeYBottom - 1);
		    _validMoveRect.center = new Vector2(_validMapRect.center.x,
			    _validMapRect.center.y - cameraHalfSize.y * 2 *
			    (ConstDefineGM2D.CameraMoveOutSizeYBottom - ConstDefineGM2D.CameraMoveOutSizeYTop) / 2);
	    }
	}
}