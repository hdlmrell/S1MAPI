using System.Collections.Generic;
using UnityEngine;
using MAPI.Core;
using MAPI.Utils;

namespace MAPI.ProceduralMesh
{
    /// <summary>
    /// Static builder for creating organic shapes like segmented bodies and articulated limbs.
    /// Based on patterns from Mrs. Ming's Authentic Pets MeshGenerator.
    /// </summary>
    public static class OrganicShapeBuilder
    {
        #region Public Members

        /// <summary>
        /// Create a segmented body mesh using a body profile.
        /// </summary>
        /// <param name="size">Overall size of the body (x=width, y=height, z=length)</param>
        /// <param name="profile">Body profile defining the shape</param>
        /// <param name="name">Name for the mesh</param>
        /// <returns>The created mesh</returns>
        public static Mesh CreateSegmentedBody(Vector3 size, BodyProfile profile, string name = "SegmentedBody")
        {
            Mesh mesh = new Mesh { name = name };
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            int segments = profile.RingSegments;

            // Add rings based on profile
            foreach (RingProfile ring in profile.Rings)
            {
                Vector4 shape = new Vector4(
                    ring.WidthScale * size.x / 2f,
                    ring.HeightScaleTop * size.y / 2f,
                    ring.HeightScaleBottom * size.y / 2f,
                    ring.Asymmetry * size.y
                );

                AddRing(vertices, ring.ZPosition * size.z, shape, segments);
            }

            // Connect rings with triangles
            for (int i = 0; i < profile.Rings.Length - 1; i++)
            {
                int currentRing = i * segments;
                int nextRing = (i + 1) * segments;

                for (int j = 0; j < segments; j++)
                {
                    int v0 = currentRing + j;
                    int v1 = currentRing + (j + 1) % segments;
                    int v2 = nextRing + j;
                    int v3 = nextRing + (j + 1) % segments;

                    // Two triangles per quad
                    triangles.Add(v0);
                    triangles.Add(v2);
                    triangles.Add(v1);

                    triangles.Add(v1);
                    triangles.Add(v2);
                    triangles.Add(v3);
                }
            }

            // Cap front
            int frontCenter = vertices.Count;
            vertices.Add(new Vector3(0f, 0f, profile.Rings[0].ZPosition * size.z));
            for (int i = 0; i < segments; i++)
            {
                triangles.Add(frontCenter);
                triangles.Add((i + 1) % segments);
                triangles.Add(i);
            }

            // Cap back
            int backCenter = vertices.Count;
            int lastRingStart = (profile.Rings.Length - 1) * segments;
            vertices.Add(new Vector3(0f, 0f, profile.Rings[profile.Rings.Length - 1].ZPosition * size.z));
            for (int i = 0; i < segments; i++)
            {
                triangles.Add(backCenter);
                triangles.Add(lastRingStart + i);
                triangles.Add(lastRingStart + (i + 1) % segments);
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            ResourceTracker.Register(mesh);
            DebugLog.Info($"Created segmented body mesh: {name} ({vertices.Count} vertices)");

            return mesh;
        }

        /// <summary>
        /// Create an articulated limb mesh (leg, arm, tail, etc.).
        /// </summary>
        /// <param name="profile">Limb profile defining joint positions and radii</param>
        /// <param name="name">Name for the mesh</param>
        /// <returns>The created mesh</returns>
        public static Mesh CreateArticulatedLimb(LimbProfile profile, string name = "ArticulatedLimb")
        {
            Mesh mesh = new Mesh { name = name };
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            int segments = profile.Segments;
            int jointCount = profile.JointPositions.Length;

            // Add rings at each joint
            for (int i = 0; i < jointCount; i++)
            {
                AddLegRing(
                    vertices,
                    profile.JointPositions[i],
                    profile.JointRadii[i],
                    segments,
                    i < profile.ForwardPush.Length ? profile.ForwardPush[i] : 0f
                );
            }

            // Connect rings with triangles
            for (int i = 0; i < jointCount - 1; i++)
            {
                int currentRing = i * segments;
                int nextRing = (i + 1) * segments;

                for (int j = 0; j < segments; j++)
                {
                    int v0 = currentRing + j;
                    int v1 = currentRing + (j + 1) % segments;
                    int v2 = nextRing + j;
                    int v3 = nextRing + (j + 1) % segments;

                    triangles.Add(v0);
                    triangles.Add(v2);
                    triangles.Add(v1);

                    triangles.Add(v1);
                    triangles.Add(v2);
                    triangles.Add(v3);
                }
            }

            // Cap top
            int topCenter = vertices.Count;
            vertices.Add(profile.JointPositions[0]);
            for (int i = 0; i < segments; i++)
            {
                triangles.Add(topCenter);
                triangles.Add((i + 1) % segments);
                triangles.Add(i);
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            ResourceTracker.Register(mesh);
            DebugLog.Info($"Created articulated limb mesh: {name} ({vertices.Count} vertices)");

            return mesh;
        }

        /// <summary>
        /// Create a low-poly paw mesh with main pad and toe pads.
        /// </summary>
        /// <param name="width">Width of the paw</param>
        /// <param name="height">Height of the paw</param>
        /// <param name="length">Length of the paw</param>
        /// <param name="name">Name for the mesh</param>
        /// <returns>The created mesh</returns>
        public static Mesh CreatePaw(float width, float height, float length, string name = "Paw")
        {
            Mesh mesh = new Mesh { name = name };
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            // Main pad
            float mainPadLength = length * 0.6f;
            float mainPadWidth = width * 0.95f;
            float mainPadHeight = height * 0.4f;

            Vector3 mainPadCenter = new Vector3(0f, -height / 2f + mainPadHeight / 2f, -length / 2f + mainPadLength / 2f);
            AddBox(vertices, triangles, mainPadCenter, new Vector3(mainPadWidth, mainPadHeight, mainPadLength));

            // Toe pads
            float toeLength = length * 0.4f;
            float toeWidth = width * 0.22f;
            float toeHeight = height * 0.5f;
            float toeSpacing = width * 0.05f;

            float toeStartX = -(4f * toeWidth + 3f * toeSpacing) / 2f + toeWidth / 2f;
            float toeZ = length / 2f - toeLength / 2f;

            for (int i = 0; i < 4; i++)
            {
                float toeX = toeStartX + i * (toeWidth + toeSpacing);
                float toeOffsetZ = Mathf.Abs(i - 1.5f) * -0.1f * length;

                Vector3 toeCenter = new Vector3(toeX, -height / 2f + toeHeight / 2f, toeZ + toeOffsetZ);
                AddBox(vertices, triangles, toeCenter, new Vector3(toeWidth, toeHeight, toeLength));
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            ResourceTracker.Register(mesh);
            DebugLog.Info($"Created paw mesh: {name}");

            return mesh;
        }

        /// <summary>
        /// Create a simple ear mesh.
        /// </summary>
        /// <param name="radius">Size of the ear</param>
        /// <param name="name">Name for the mesh</param>
        /// <returns>The created mesh</returns>
        public static Mesh CreateEar(float radius, string name = "Ear")
        {
            Mesh mesh = new Mesh { name = name };
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            // Triangular ear with slight thickness
            vertices.Add(new Vector3(0f, 0f, 0f));
            vertices.Add(new Vector3(radius * 0.2f, radius * 1.5f, 0f));
            vertices.Add(new Vector3(radius, 0f, 0f));
            vertices.Add(new Vector3(0f, 0f, -radius * 0.2f));
            vertices.Add(new Vector3(radius * 0.2f, radius * 1.5f, -radius * 0.2f));
            vertices.Add(new Vector3(radius, 0f, -radius * 0.2f));

            // Front and back faces
            triangles.AddRange(new int[] { 0, 1, 2 });
            triangles.AddRange(new int[] { 5, 4, 3 });

            // Side faces
            triangles.AddRange(new int[] { 0, 3, 1, 1, 3, 4 });
            triangles.AddRange(new int[] { 1, 4, 2, 2, 4, 5 });
            triangles.AddRange(new int[] { 2, 5, 0, 0, 5, 3 });

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            ResourceTracker.Register(mesh);
            return mesh;
        }

        #endregion

        #region Private Members

        /// <summary>
        /// INTERNAL: Add a ring of vertices with asymmetric scaling.
        /// Based on MeshGenerator.AddRing from the original mod.
        /// </summary>
        private static void AddRing(List<Vector3> vertices, float z, Vector4 shape, int segments)
        {
            float widthRadius = shape.x;
            float heightRadiusTop = shape.y;
            float heightRadiusBottom = shape.z;
            float asymmetry = shape.w;

            for (int i = 0; i < segments; i++)
            {
                float angle = (float)i / segments * Mathf.PI * 2f;
                float x = Mathf.Cos(angle) * widthRadius;
                float sinAngle = Mathf.Sin(angle);
                float y;

                if (sinAngle > 0f)
                {
                    // Upper half - apply asymmetry
                    y = sinAngle * heightRadiusTop;
                    y *= 1f - asymmetry * Mathf.Abs(Mathf.Cos(angle));
                }
                else
                {
                    // Lower half
                    y = sinAngle * heightRadiusBottom;
                }

                vertices.Add(new Vector3(x, y, z));
            }
        }

        /// <summary>
        /// INTERNAL: Add a ring of vertices for a limb segment (elliptical cross-section).
        /// Based on MeshGenerator.AddLegRing from the original mod.
        /// </summary>
        private static void AddLegRing(List<Vector3> vertices, Vector3 offset, float radius, int segments, float forwardPush)
        {
            for (int i = 0; i < segments; i++)
            {
                float angle = (float)i / segments * 2f * Mathf.PI;
                float x = Mathf.Cos(angle) * radius * 0.8f;  // Elliptical: narrower in X
                float z = Mathf.Sin(angle) * radius * 1.2f;  // Elliptical: wider in Z

                vertices.Add(new Vector3(
                    offset.x + x,
                    offset.y,
                    offset.z + z + forwardPush
                ));
            }
        }

        /// <summary>
        /// INTERNAL: Add a box to the vertices and triangles lists.
        /// </summary>
        private static void AddBox(List<Vector3> vertices, List<int> triangles, Vector3 center, Vector3 size)
        {
            int startIndex = vertices.Count;
            Vector3 halfSize = size / 2f;

            // 8 corners
            vertices.Add(center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z));
            vertices.Add(center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z));
            vertices.Add(center + new Vector3(halfSize.x, -halfSize.y, halfSize.z));
            vertices.Add(center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z));
            vertices.Add(center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z));
            vertices.Add(center + new Vector3(halfSize.x, halfSize.y, -halfSize.z));
            vertices.Add(center + new Vector3(halfSize.x, halfSize.y, halfSize.z));
            vertices.Add(center + new Vector3(-halfSize.x, halfSize.y, halfSize.z));

            // 12 triangles (6 faces x 2 triangles)
            int[] boxTriangles = new int[]
            {
                0, 2, 1, 0, 3, 2,  // Bottom
                4, 5, 6, 4, 6, 7,  // Top
                0, 1, 5, 0, 5, 4,  // Front
                2, 3, 7, 2, 7, 6,  // Back
                1, 2, 6, 1, 6, 5,  // Right
                3, 0, 4, 3, 4, 7   // Left
            };

            foreach (int index in boxTriangles)
            {
                triangles.Add(startIndex + index);
            }
        }

        #endregion
    }
}
