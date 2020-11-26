//
// ShaderGraphEssentials for Unity
// (c) 2019 PH Graphics
// Source code may be used and modified for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 
// *** A NOTE ABOUT PIRACY ***
// 
// If you got this asset from a pirate site, please consider buying it from the Unity asset store. This asset is only legally available from the Unity Asset Store.
// 
// I'm a single indie dev supporting my family by spending hundreds and thousands of hours on this and other assets. It's very offensive, rude and just plain evil to steal when I (and many others) put so much hard work into the software.
// 
// Thank you.
//
// *** END NOTE ABOUT PIRACY ***
//

using UnityEngine;

namespace ShaderGraphEssentials

{
    [System.Serializable]
    public class NoiseOctaveData
    {
        public Vector2 Direction;
        public float Amplitude = 1f;
        public float Speed = 1f;
        public float Frequency = 1f;
    }

    public class TesselatedWater : MonoBehaviour
    {
        // disable "not assigned" warning because it is assigned by the editor
#pragma warning disable 0649
        [SerializeField]
        private NoiseOctaveData[] _noiseOctaveData;

        [SerializeField] 
        private MeshFilter _waterGrid;
#pragma warning restore 0649

        private Mesh _waterMesh;

        private Vector3[] _vertices;

        void Start()
        {
            _waterMesh = _waterGrid.mesh;
            _vertices = _waterMesh.vertices;
        }

        void Update()
        {
            for (int i = 0; i < _vertices.Length; i++)
            {
                var position = _vertices[i];
                float height = 0f;
                foreach (var data in _noiseOctaveData)
                {
                    Vector2 direction = data.Direction.normalized;
                    height += data.Amplitude * Mathf.Sin(Time.time * data.Speed + data.Frequency * (position.x * direction.x + position.z * direction.y));
                }
                
                _vertices[i] = new Vector3(position.x, height, position.z);
            }

            _waterMesh.vertices = _vertices;
            _waterMesh.RecalculateNormals();

        }
    }

}