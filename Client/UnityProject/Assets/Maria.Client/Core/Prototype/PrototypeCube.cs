using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Maria.Client.Core.Prototype
{
	[ExecuteInEditMode]
	public class PrototypeCube : MonoBehaviour
	{
		public enum Resolution
		{
			m1x1,
			m4x4,
			m10x10,
			m100x100
		}

		public class ResolutionConfig
		{
			public ResolutionConfig(Resolution r)
			{
				_Resolution = r;
			}

			public void Load()
			{
				var dir = "Assets/Maria.Client/Core/Prototype/Asb/Materials/";
				string path;
				switch (_Resolution)
				{
					case Resolution.m1x1:
					{
						path = dir + "Maria_Prototype_1mx1m.mat";
						break;
					}
					case Resolution.m4x4:
					{
						path = dir + "Maria_Prototype_4mx4m.mat";
						break;
					}
					case Resolution.m10x10:
					{
						path = dir + "Maria_Prototype_10mx10m.mat";
						break;
					}
					case Resolution.m100x100:
					{
						path = dir + "Maria_Prototype_100mx100m.mat";
						break;
					}
					default:
					{
						throw new NotImplementedException();
					}
				}
				
#if UNITY_EDITOR
				_Material = AssetDatabase.LoadAssetAtPath<Material>(path);
#else
				var handler = AssetManager.LoadAsset<Material>(path);
				handler.SyncLoad();
				_Material = handler.Asset;
#endif
			}

			public Material GetMaterial()
			{
				return _Material;
			}

			public float GetResolutionSize()
			{
				if (_Resolution == Resolution.m1x1)
				{
					return 1;
				}
				else if (_Resolution == Resolution.m4x4)
				{
					return 4;
				}
				else if (_Resolution == Resolution.m10x10)
				{
					return 10;
				}
				else if (_Resolution == Resolution.m100x100)
				{
					return 100;
				}

				throw new NotImplementedException();
			}

			private readonly Resolution _Resolution;
			private Material _Material;
		}

		private static readonly Dictionary<Resolution, ResolutionConfig> _CacheMaterials = new Dictionary<Resolution, ResolutionConfig>();

		private void Awake()
		{
			_LastResolution = _Resolution;
			_LastLocalScale = transform.localScale;

			var shared = GetComponent<MeshFilter>().sharedMesh;
			_Mesh = new Mesh
			{
				name = "Prototype Cube",
				vertices = shared.vertices,
				triangles = shared.triangles,
				normals = shared.normals,
				tangents = shared.tangents,
				uv = _CreateUV()
			};
			GetComponent<MeshFilter>().mesh = _Mesh;
			
			_UpdateMeshUV();
			_UpdateMaterial();
		}

		private Vector2[] _CreateUV(float sx = 1.0f, float sy = 1.0f, float sz = 1.0f)
		{
			var uv = new Vector2[24];
			uv[0] = new Vector2(0, 0) * new Vector2(sx, sy);
			uv[1] = new Vector2(1, 0) * new Vector2(sx, sy);
			uv[2] = new Vector2(0, 1) * new Vector2(sx, sy);
			uv[3] = new Vector2(1, 1) * new Vector2(sx, sy);
			
			uv[4] = new Vector2(0, 1) * new Vector2(sx, sy);
			uv[5] = new Vector2(1, 1) * new Vector2(sx, sy);
			uv[6] = new Vector2(0, 1) * new Vector2(sx, sy);
			uv[7] = new Vector2(1, 1) * new Vector2(sx, sy);
			
			uv[8] = new Vector2(0, 0) * new Vector2(sx, sz);
			uv[9] = new Vector2(1, 0) * new Vector2(sx, sz);
			uv[10] = new Vector2(0, 0) * new Vector2(sx, sz);
			uv[11] = new Vector2(1, 0) * new Vector2(sx, sz);
			
			uv[12] = new Vector2(0, 0) * new Vector2(sx, sz);
			uv[13] = new Vector2(0, 1) * new Vector2(sx, sz);
			uv[14] = new Vector2(1, 1) * new Vector2(sx, sz);
			uv[15] = new Vector2(1, 0) * new Vector2(sx, sz);
			
			uv[16] = new Vector2(0, 0) * new Vector2(sz, sy);
			uv[17] = new Vector2(0, 1) * new Vector2(sz, sy);
			uv[18] = new Vector2(1, 1) * new Vector2(sz, sy);
			uv[19] = new Vector2(1, 0) * new Vector2(sz, sy);
			
			uv[20] = new Vector2(0, 0) * new Vector2(sz, sy);
			uv[21] = new Vector2(0, 1) * new Vector2(sz, sy);
			uv[22] = new Vector2(1, 1) * new Vector2(sz, sy);
			uv[23] = new Vector2(1, 0) * new Vector2(sz, sy);
			return uv;
		}

		private void Update()
		{
			bool needUpdateUV = false;
			bool needUpdateMaterial = false;
			
			if (_LastResolution != _Resolution)
			{
				_LastResolution = _Resolution;
				needUpdateUV = true;
				needUpdateMaterial = true;
			}
			
			var localScale = transform.localScale;
			var dx = localScale.x - _LastLocalScale.x;
			var dy = localScale.y - _LastLocalScale.y;
			var dz = localScale.z - _LastLocalScale.z;
			if (Math.Abs(dx) > float.Epsilon || Math.Abs(dy) > float.Epsilon || Math.Abs(dz) > float.Epsilon)
			{
				_LastLocalScale = transform.localScale;
				needUpdateUV = true;
			}

			if (needUpdateUV)
			{
				_UpdateMeshUV();
			}
			if (needUpdateMaterial)
			{
				_UpdateMaterial();
			}
		}

		private void _UpdateMeshUV()
		{
			var config = _GetConfig(_Resolution);
			var size = config.GetResolutionSize();
			var uv = _CreateUV(_LastLocalScale.x / size , _LastLocalScale.y / size, _LastLocalScale.z / size);
			_Mesh.uv = uv;
		}

		private void _UpdateMaterial()
		{
			var config = _GetConfig(_Resolution);
			var meshRenderer = GetComponent<MeshRenderer>();
			meshRenderer.material = config.GetMaterial();
			_LastResolution = _Resolution;
		}

		private ResolutionConfig _GetConfig(Resolution r)
		{
			if (_CacheMaterials.TryGetValue(r, out var c))
			{
				return c;
			}
			var config = new ResolutionConfig(r);
			config.Load();
			
			_CacheMaterials[r] = config;
			return config;
		}
		
		public Resolution _Resolution = Resolution.m1x1;
		private Mesh _Mesh;
		private Vector3 _LastLocalScale;
		private Resolution _LastResolution = Resolution.m1x1;

	}
}