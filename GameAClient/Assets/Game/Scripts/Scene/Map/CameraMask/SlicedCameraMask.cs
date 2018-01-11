/********************************************************************
** Filename : SlicedCameraMask  
** Author : ake
** Date : 5/4/2016 4:26:06 PM
** Summary : SlicedCameraMask  
***********************************************************************/


using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
	public class SlicedCameraMask:MonoBehaviour
	{
		public Transform CameraMaskTran;
		public SpriteRenderer LayerMaskSprite;

		public float ValidRectangleWidth = 1;
		public float ValidRectangleHeight = 1;

		public float FadeAreaWidth = 0.1f;
		public float FadeAreaHeight = 0.1f;

		public Color HiddenCololr;
		public float StartAlpha;

		private Mesh _cachedMesh;
		private Vector3[] _vertices;
		private int[] _trangles;
		private Color[] _colors;

		private MeshFilter _filter;
		private Transform _trans;


		private float _cachedWholeWidth;
		private float _cachedWholeHeight;

		private float _broderWidth;
		private float _broderHeight;

		private float _offsetX1;
		private float _offsetX2;
		private float _offsetY1;
		private float _offsetY2;


		public const int OuterStartIndex = 0;
		public const int MiddleStartIndex = 12;
		public const int InerStartIndex= 24;

		private Vector3 _scale = Vector3.one;

		private float _realFadeAreaWidth;
		private float _realFadeAreaHeight;

		public enum ERectPoint
		{
			A =0,
			B,
			C,
			D,
			Ar,
			Br,
			Cr,
			Dr,
			Al,
			Bl,
			Cl,
			Dl,

			A2,
			B2,
			C2,
			D2,
			Ar2,
			Br2,
			Cr2,
			Dr2,
			Al2,
			Bl2,
			Cl2,
			Dl2,
			A3,
			B3,
			C3,
			D3,
		}

		public Vector3[] Vertices
		{
			get
			{
				return _vertices;
			}
		}

		public Transform Trans
		{
			get
			{
				return _trans;
			}
		}


		void Awake()
		{
			InitMesh();
			_trans = transform;
			HideLayerMask();
		}

		void OnDestroy()
		{
			if (_cachedMesh != null)
			{
				Destroy(_cachedMesh);
			}
		}

		public void Show()
		{
			gameObject.SetActive(true);
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}

		public void ShowLayerMask()
		{
			LayerMaskSprite.SetActiveEx(true);
		}

		public void HideLayerMask()
		{
			LayerMaskSprite.SetActiveEx(false);
		}

		public void SetCameraMaskSortOrder(int value)
		{
			Renderer render = CameraMaskTran.GetComponent<Renderer>();
			if (render != null)
			{
				render.sortingOrder = value;
			}
			else
			{
				LogHelper.Error("SetCameraMaskSortOrder called but GetComponent<Renderer>() is null! value is {0}",value);
			}
		}

		public void SetLayerMaskSortOrder(int value)
		{
			LayerMaskSprite.sortingOrder = value;
		}

		public void SetLocalScale(float x, float y)
		{
			_scale.x = x;
			_scale.y = y;
			_trans.localScale = _scale;
			UpdateRealFadeAreaValue();
			UpdateShow();
		}

		public void SetValidMapWorldRect(Rect rect)
		{
			_trans.localPosition = new Vector3(rect.center.x, rect.center.y, -30);
			SetLocalScale(rect.width, rect.height);
		}



		#region private

		private void UpdateRealFadeAreaValue()
		{
			float x = _scale.x<=1 ? 1: _scale.x;
			float y = _scale.y <= 1 ? 1 : _scale.y;
			_realFadeAreaWidth = FadeAreaWidth/x;
			_realFadeAreaHeight = FadeAreaHeight/y;
		}

		private void UpdateShow()
		{
			UpdateMeshData();
			_cachedMesh.vertices = _vertices;
			_cachedMesh.colors = _colors;
		}

		private void InitMesh()
		{
			_cachedMesh = new Mesh();
			_colors = new Color[28];
			_vertices = new Vector3[28];
			_trangles = new int[32 * 3];
			UpdateRealFadeAreaValue();
			UpdateMeshData();
			InitTrangles();
			_cachedMesh.vertices = _vertices;
			_cachedMesh.colors = _colors;
			_cachedMesh.triangles = _trangles;
			_filter = CameraMaskTran.GetComponent<MeshFilter>();
			_filter.mesh = _cachedMesh;
		}

		private void UpdateMeshData()
		{
			_cachedWholeWidth = ValidRectangleWidth*5;
			_cachedWholeHeight = ValidRectangleHeight * 5;
			_offsetX1 = (_cachedWholeWidth - ValidRectangleWidth)/2f - _realFadeAreaWidth;
			_offsetX2 = _realFadeAreaWidth;
			_offsetY1 = (_cachedWholeHeight - ValidRectangleHeight)/2f - _realFadeAreaHeight;
			_offsetY2 = _realFadeAreaHeight;
			_broderWidth = ValidRectangleWidth + _realFadeAreaWidth * 2;
			_broderHeight = ValidRectangleHeight + _realFadeAreaHeight * 2;

			UpdateVectors();
		}


		private void SetVector(int index, float x, float y, Color color)
		{
			_vertices[index].x = x;
			_vertices[index].y = y;
			_vertices[index].z = 0;
			_colors[index] = color;
		}

		private void SetVector(ERectPoint index, float x, float y, Color color)
		{
			SetVector((int) index + OuterStartIndex, x, y, color);
		}

		private void SetMiddleVector(ERectPoint index, float x, float y, Color color)
		{
			SetVector((int) index + MiddleStartIndex, x, y, color);
		}

		private void SetInerVector(ERectPoint index, float x, float y, Color color)
		{
			SetVector((int)index + InerStartIndex, x, y, color);
		}

		private void SetVectorOffset(int index, float offsetX, float offsetY)
		{
			_vertices[index].x += offsetX;
			_vertices[index].y += offsetY;
		}

		private void SetTrangle(int index, ERectPoint v1, ERectPoint v2, ERectPoint v3)
		{
			_trangles[index * 3] = (int)v1;
			_trangles[index *3+ 1] = (int)v3;
			_trangles[index*3 + 2] = (int)v2;
		}

		private void UpdateVectors()
		{
			Color blendColor = HiddenCololr;
			blendColor.a = Mathf.Clamp01(StartAlpha);
			SetVector(ERectPoint.A, 0, 0, HiddenCololr);
			SetVector(ERectPoint.B, _cachedWholeWidth, 0, HiddenCololr);
			SetVector(ERectPoint.C, _cachedWholeWidth, _cachedWholeHeight, HiddenCololr);
			SetVector(ERectPoint.D, 0, _cachedWholeHeight, HiddenCololr);

			SetVector(ERectPoint.Ar, _offsetX1, 0, HiddenCololr);
			SetVector(ERectPoint.Br, _cachedWholeWidth, _offsetY1, HiddenCololr);
			SetVector(ERectPoint.Cr, _cachedWholeWidth - _offsetX1, _cachedWholeHeight, HiddenCololr);
			SetVector(ERectPoint.Dr, 0, _cachedWholeHeight - _offsetY1, HiddenCololr);

			SetVector(ERectPoint.Al, 0, _offsetY1, HiddenCololr);
			SetVector(ERectPoint.Bl, _cachedWholeWidth - _offsetX1, 0, HiddenCololr);
			SetVector(ERectPoint.Cl, _cachedWholeWidth, _cachedWholeHeight - _offsetY1, HiddenCololr);
			SetVector(ERectPoint.Dl, _offsetX1, _cachedWholeHeight, HiddenCololr);

			{
				SetMiddleVector(ERectPoint.A, 0, 0, HiddenCololr);
				SetMiddleVector(ERectPoint.B, _broderWidth, 0, HiddenCololr);
				SetMiddleVector(ERectPoint.C, _broderWidth, _broderHeight, HiddenCololr);
				SetMiddleVector(ERectPoint.D, 0, _broderHeight, HiddenCololr);

				SetMiddleVector(ERectPoint.Ar, _offsetX2, 0, HiddenCololr);
				SetMiddleVector(ERectPoint.Br, _broderWidth, _offsetY2, HiddenCololr);
				SetMiddleVector(ERectPoint.Cr, _broderWidth - _offsetX2, _broderHeight, HiddenCololr);
				SetMiddleVector(ERectPoint.Dr, 0, _broderHeight - _offsetY2, HiddenCololr);

				SetMiddleVector(ERectPoint.Al, 0, _offsetY2, HiddenCololr);
				SetMiddleVector(ERectPoint.Bl, _broderWidth - _offsetX2, 0, HiddenCololr);
				SetMiddleVector(ERectPoint.Cl, _broderWidth, _broderHeight - _offsetY2, HiddenCololr);
				SetMiddleVector(ERectPoint.Dl, _offsetX2, _broderHeight, HiddenCololr);

				{
					SetInerVector(ERectPoint.A, 0, 0, blendColor);
					SetInerVector(ERectPoint.B, ValidRectangleWidth, 0, blendColor);
					SetInerVector(ERectPoint.C, ValidRectangleWidth, ValidRectangleHeight, blendColor);
					SetInerVector(ERectPoint.D, 0, ValidRectangleHeight, blendColor);

					for (int i = 24; i < _vertices.Length; i++)
					{
						SetVectorOffset(i, _offsetX2, _offsetY2);
					}
				}

				for (int i = 12; i < _vertices.Length; i++)
				{
					SetVectorOffset(i, _offsetX1, _offsetY1);
				}
			}

			for (int i = 0; i < _vertices.Length; i++)
			{
				SetVectorOffset(i, -_cachedWholeWidth/2, -_cachedWholeHeight/2);
			}
		}

		private void InitTrangles()
		{
			int i = 0;
			SetTrangle(i++, ERectPoint.Al, ERectPoint.A, ERectPoint.Ar);
			SetTrangle(i++, ERectPoint.Al, ERectPoint.Ar, ERectPoint.A2);
			SetTrangle(i++, ERectPoint.A2, ERectPoint.Ar, ERectPoint.Bl);
			SetTrangle(i++, ERectPoint.A2, ERectPoint.Bl, ERectPoint.B2);

			SetTrangle(i++, ERectPoint.B2, ERectPoint.Bl, ERectPoint.B);
			SetTrangle(i++, ERectPoint.B2, ERectPoint.B, ERectPoint.Br);
			SetTrangle(i++, ERectPoint.C2, ERectPoint.B2, ERectPoint.Br);
			SetTrangle(i++, ERectPoint.C2, ERectPoint.Br, ERectPoint.Cl);

			SetTrangle(i++, ERectPoint.Cr, ERectPoint.C2, ERectPoint.Cl);
			SetTrangle(i++, ERectPoint.Cr, ERectPoint.Cl, ERectPoint.C);
			SetTrangle(i++, ERectPoint.Dl, ERectPoint.D2, ERectPoint.C2);
			SetTrangle(i++, ERectPoint.Dl, ERectPoint.C2, ERectPoint.Cr);

			SetTrangle(i++, ERectPoint.D, ERectPoint.Dr, ERectPoint.D2);
			SetTrangle(i++, ERectPoint.D, ERectPoint.D2, ERectPoint.Dl);
			SetTrangle(i++, ERectPoint.Dr, ERectPoint.Al, ERectPoint.A2);
			SetTrangle(i++, ERectPoint.Dr, ERectPoint.A2, ERectPoint.D2);

			{
				SetTrangle(i++, ERectPoint.Al2, ERectPoint.A2, ERectPoint.A3);
				SetTrangle(i++, ERectPoint.A2, ERectPoint.Ar2, ERectPoint.A3);
				SetTrangle(i++, ERectPoint.A3, ERectPoint.Ar2, ERectPoint.Bl2);
				SetTrangle(i++, ERectPoint.A3, ERectPoint.Bl2, ERectPoint.B3);

				SetTrangle(i++, ERectPoint.B3, ERectPoint.Bl2, ERectPoint.B2);
				SetTrangle(i++, ERectPoint.B3, ERectPoint.B2, ERectPoint.Br2);
				SetTrangle(i++, ERectPoint.C3, ERectPoint.B3, ERectPoint.Br2);
				SetTrangle(i++, ERectPoint.C3, ERectPoint.Br2, ERectPoint.Cl2);

				SetTrangle(i++, ERectPoint.Cr2, ERectPoint.C3, ERectPoint.C2);
				SetTrangle(i++, ERectPoint.C3, ERectPoint.Cl2, ERectPoint.C2);
				SetTrangle(i++, ERectPoint.Dl2, ERectPoint.D3, ERectPoint.C3);
				SetTrangle(i++, ERectPoint.Dl2, ERectPoint.C3, ERectPoint.Cr2);

				SetTrangle(i++, ERectPoint.D2, ERectPoint.Dr2, ERectPoint.D3);
				SetTrangle(i++, ERectPoint.D2, ERectPoint.D3, ERectPoint.Dl2);
				SetTrangle(i++, ERectPoint.Dr2, ERectPoint.Al2, ERectPoint.A3);
				SetTrangle(i, ERectPoint.Dr2, ERectPoint.A3, ERectPoint.D3);
			}
		}


		#endregion
	}
}
