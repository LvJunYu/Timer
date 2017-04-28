/********************************************************************
** Filename : CameraManager
** Author : Dong
** Date : 2015/7/8 星期三 下午 10:15:19
** Summary : CameraManager
***********************************************************************/

using System;
using DG.Tweening;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    // 编辑模式下的部分逻辑
    public partial class CameraManager : MonoBehaviour
    {
        // 编辑模式下拖拽摄像机回弹的动作
        private PositionSpringbackEffect _positionEffect;
        // 编辑模式下缩放摄像机回弹的动作
        private OrthoSizeSpringbackEffect _orthoEffect;


        public void SetEditorModeStartPos(Vector3 pos)
        {
            UpdateVaildMapRect();
            UpdateCameraViewRect();
            UpdateCameraMoveRect();
            UpdatePos(pos, true);
        }

        #region event

        private void OnValidMapRectChanged(IntRect changedTileSize)
        {
            SetMinMax(true);
        }

        private void OnScreenOperatorSuccess(EScreenOperator type)
        {
            float x, y;
            switch (type)
            {
                case EScreenOperator.LeftAdd:
                case EScreenOperator.LeftDelete:
                {
                    x = _cameraMoveRect.xMin;
                    y = _finalPos.y;
                    break;
                }
                case EScreenOperator.RightAdd:
                case EScreenOperator.RightDelete:
                {
                    x = _cameraMoveRect.xMax;
                    y = _finalPos.y;
                    break;
                }
                case EScreenOperator.UpDelete:
                case EScreenOperator.UpAdd:
                {
                    x = _finalPos.x;
                    y = _cameraMoveRect.yMax;
                    break;
                }
                default:
                {
                    LogHelper.Error("OnScreenOperatorSuccess called but {0} is unexpected!", type);
                    return;
                }
            }
            Action finishCallback = null;
            if (GM2DTools.CheckIsDeleteScreenOperator(type))
            {
                finishCallback = OnFinishTweenCameraPos; 
            }
            else
            {
                Messenger.Broadcast(EMessengerType.ForceUpdateCameraMaskSize);
            }
            UpdatePos(new Vector2(x, y), false, finishCallback,false);
        }


        private void UpdatePos(Vector2 pos, bool immediately = false, Action finishCallback = null,bool clampPos = true)
        {
            _finalPos = pos;
            if (clampPos)
            {
                _finalPos.x = Mathf.Clamp(_finalPos.x, _cameraMoveRect.xMin, _cameraMoveRect.xMax);
                _finalPos.y = Mathf.Clamp(_finalPos.y, _cameraMoveRect.yMin, _cameraMoveRect.yMax);
            }

            if (immediately)
            {
                RendererCameraPos = _finalPos;
            }
            else
            {
                const float duration = 1;
                DoCameraTweenPos(_finalPos, duration, finishCallback);
            }

            if (GM2DGame.Instance.CurrentMode == EMode.Edit)
            {
                UpdateCameraViewRect();
                Messenger.Broadcast(EMessengerType.OnEditorModeCameraMove);
            }
        }


        private void OnFinishTweenCameraPos()
        {
            Messenger.Broadcast(EMessengerType.ForceUpdateCameraMaskSize);
        }


        #endregion



        public void UpdateFadePostionOffset(Vector2 offset)
        {
            _positionEffect.UpdatePosOffset(offset);
        }

        public void OnDragEnd(Vector2 delta)
        {
            _positionEffect.OnDragEnd(delta);
        }

        public void OnPinchEnd()
        {
            _orthoEffect.OnPinchEnd();
        }

        public void UpdateFadeCameraOrthoSizeOffset(float offset)
        {
            _orthoEffect.UpdateOffset(offset);
            //   _finalOrthoSize += offset;
            //Tweener t = _rendererCamera.DOOrthoSize(_finalOrthoSize, ConstDefineGM2D.CameraOrthoSizeFadeTime);
            //t.SetEase(Ease.OutQuart);
        }

        public bool CheckReachLimitLeft(Vector3 pos)
        {
            if (_cameraViewRect.xMin - _validMapRect.xMin < ConstDefineGM2D.ScreenOperatorVisibleDiffer)
            {
                return true;
            }
            return false;
        }

        public bool CheckReachLimitRight(Vector3 pos)
        {
            if (_validMapRect.xMax - _cameraViewRect.xMax < ConstDefineGM2D.ScreenOperatorVisibleDiffer)
            {
                return true;
            }
            return false;
        }

        public bool CheckReachLimitTop(Vector3 pos)
        {
            if (_validMapRect.yMax - _cameraViewRect.yMax < ConstDefineGM2D.ScreenOperatorVisibleDiffer)
            {
                return true;
            }
            return false;
        }


        #region private

        private void DoCameraTweenPos(Vector3 finalPos, float duration, Action finisCallback = null)
        {
            if (_cameraPosTweener == null || !_cameraPosTweener.IsActive())
            {
                _cameraPosTweener = DOTween.To(()=>RendererCameraPos, (v)=>{RendererCameraPos = v;}, _finalPos, duration);
            }
            else
            {
                _cameraPosTweener.ChangeEndValue(finalPos, true);
            }
            _cameraPosTweener.SetUpdate(true);
            _cameraPosTweener.SetEase(Ease.OutCubic);
            if (finisCallback == null)
            {
                _cameraPosTweener.OnComplete(null);
            }
            else
            {
                _cameraPosTweener.OnComplete(new TweenCallback(finisCallback));
            }
        }
    }


    #endregion
}