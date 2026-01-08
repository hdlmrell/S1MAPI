using S1MAPI.Gltf.Data;
using UnityEngine;

namespace S1MAPI.Gltf.Processing
{
    /// <summary>
    /// Processes GLTF animations and converts them to Unity AnimationClips.
    /// Supports translation, rotation, scale, and morph target animations.
    /// </summary>
    internal static class GltfAnimationProcessor
    {
        #region Public API

        /// <summary>
        /// Process all GLTF animations and convert them to Unity AnimationClips.
        /// </summary>
        /// <param name="context">The GLTF load context</param>
        /// <returns>List of Unity animation clips</returns>
        public static List<AnimationClip> ProcessAnimations(GltfLoadContext context)
        {
            List<AnimationClip> clips = new List<AnimationClip>();
            GltfRoot gltf = context.Root;

            if (gltf.animations == null || !context.Options.ImportAnimations)
            {
                return clips;
            }

            for (int i = 0; i < gltf.animations.Count; i++)
            {
                GltfAnimation gltfAnim = gltf.animations[i];
                AnimationClip? clip = CreateAnimationClip(context, gltfAnim, i);

                if (clip != null)
                {
                    clips.Add(clip);
                    context.RegisterAnimation(clip);
                }
            }

            return clips;
        }

        #endregion

        #region Private Methods

        private static AnimationClip? CreateAnimationClip(GltfLoadContext context, GltfAnimation gltfAnim, int index)
        {
            if (gltfAnim.channels == null || gltfAnim.samplers == null)
            {
                return null;
            }

            AnimationClip clip = new AnimationClip();
            clip.name = gltfAnim.name ?? $"animation_{index}";
            clip.legacy = true; // Use legacy for compatibility

            foreach (GltfAnimationChannel channel in gltfAnim.channels)
            {
                if (channel.target?.node == null || channel.target.path == null)
                {
                    continue;
                }

                int samplerIndex = channel.sampler;
                if (samplerIndex < 0 || samplerIndex >= gltfAnim.samplers.Count)
                {
                    continue;
                }

                GltfAnimationSampler sampler = gltfAnim.samplers[samplerIndex];
                int nodeIndex = channel.target.node.Value;
                string path = channel.target.path;

                // Get the node path for Unity
                string? nodePath = GetNodePath(context, nodeIndex);
                if (string.IsNullOrEmpty(nodePath))
                {
                    continue;
                }

                // Read keyframe data
                float[]? times = ReadFloatArray(context, sampler.input);
                if (times == null || times.Length == 0)
                {
                    continue;
                }

                switch (path)
                {
                    case GltfAnimationPath.Translation:
                        ApplyTranslationAnimation(context, clip, sampler, times, nodePath);
                        break;

                    case GltfAnimationPath.Rotation:
                        ApplyRotationAnimation(context, clip, sampler, times, nodePath);
                        break;

                    case GltfAnimationPath.Scale:
                        ApplyScaleAnimation(context, clip, sampler, times, nodePath);
                        break;

                    case GltfAnimationPath.Weights:
                        ApplyMorphTargetAnimation(context, clip, sampler, times, nodePath, nodeIndex);
                        break;
                }
            }

            clip.EnsureQuaternionContinuity();
            return clip;
        }

        private static void ApplyTranslationAnimation(
            GltfLoadContext context,
            AnimationClip clip,
            GltfAnimationSampler sampler,
            float[] times,
            string nodePath)
        {
            Vector3[]? values = ReadVector3Array(context, sampler.output);
            if (values == null || values.Length == 0)
            {
                return;
            }

            bool isCubic = sampler.interpolation == GltfInterpolation.CubicSpline;
            int keyCount = times.Length;

            AnimationCurve curveX = new AnimationCurve();
            AnimationCurve curveY = new AnimationCurve();
            AnimationCurve curveZ = new AnimationCurve();

            for (int i = 0; i < keyCount; i++)
            {
                float time = times[i];
                Vector3 value;

                if (isCubic)
                {
                    // Cubic spline: each keyframe has in-tangent, value, out-tangent (3 vec3 per keyframe)
                    int idx = i * 3 + 1;
                    value = idx < values.Length ? values[idx] : Vector3.zero;
                }
                else
                {
                    int idx = Math.Min(i, values.Length - 1);
                    value = values[idx];
                }

                // Convert GLTF to Unity coordinate system (negate X)
                curveX.AddKey(time, -value.x);
                curveY.AddKey(time, value.y);
                curveZ.AddKey(time, value.z);
            }

#if IL2CPP
            clip.SetCurve(nodePath, Il2CppInterop.Runtime.Il2CppType.Of<Transform>(), "localPosition.x", curveX);
            clip.SetCurve(nodePath, Il2CppInterop.Runtime.Il2CppType.Of<Transform>(), "localPosition.y", curveY);
            clip.SetCurve(nodePath, Il2CppInterop.Runtime.Il2CppType.Of<Transform>(), "localPosition.z", curveZ);
#else
            clip.SetCurve(nodePath, typeof(Transform), "localPosition.x", curveX);
            clip.SetCurve(nodePath, typeof(Transform), "localPosition.y", curveY);
            clip.SetCurve(nodePath, typeof(Transform), "localPosition.z", curveZ);
#endif
        }

        private static void ApplyRotationAnimation(
            GltfLoadContext context,
            AnimationClip clip,
            GltfAnimationSampler sampler,
            float[] times,
            string nodePath)
        {
            Vector4[]? values = ReadVector4Array(context, sampler.output);
            if (values == null || values.Length == 0)
            {
                return;
            }

            bool isCubic = sampler.interpolation == GltfInterpolation.CubicSpline;
            int keyCount = times.Length;

            AnimationCurve curveX = new AnimationCurve();
            AnimationCurve curveY = new AnimationCurve();
            AnimationCurve curveZ = new AnimationCurve();
            AnimationCurve curveW = new AnimationCurve();

            for (int i = 0; i < keyCount; i++)
            {
                float time = times[i];
                Vector4 value;

                if (isCubic)
                {
                    int idx = i * 3 + 1;
                    value = idx < values.Length ? values[idx] : Vector4.zero;
                }
                else
                {
                    int idx = Math.Min(i, values.Length - 1);
                    value = values[idx];
                }

                // GLTF quaternion [x, y, z, w] to Unity [x, -y, -z, w]
                curveX.AddKey(time, value.x);
                curveY.AddKey(time, -value.y);
                curveZ.AddKey(time, -value.z);
                curveW.AddKey(time, value.w);
            }

#if IL2CPP
            clip.SetCurve(nodePath, Il2CppInterop.Runtime.Il2CppType.Of<Transform>(), "localRotation.x", curveX);
            clip.SetCurve(nodePath, Il2CppInterop.Runtime.Il2CppType.Of<Transform>(), "localRotation.y", curveY);
            clip.SetCurve(nodePath, Il2CppInterop.Runtime.Il2CppType.Of<Transform>(), "localRotation.z", curveZ);
            clip.SetCurve(nodePath, Il2CppInterop.Runtime.Il2CppType.Of<Transform>(), "localRotation.w", curveW);
#else
            clip.SetCurve(nodePath, typeof(Transform), "localRotation.x", curveX);
            clip.SetCurve(nodePath, typeof(Transform), "localRotation.y", curveY);
            clip.SetCurve(nodePath, typeof(Transform), "localRotation.z", curveZ);
            clip.SetCurve(nodePath, typeof(Transform), "localRotation.w", curveW);
#endif
        }

        private static void ApplyScaleAnimation(
            GltfLoadContext context,
            AnimationClip clip,
            GltfAnimationSampler sampler,
            float[] times,
            string nodePath)
        {
            Vector3[]? values = ReadVector3Array(context, sampler.output);
            if (values == null || values.Length == 0)
            {
                return;
            }

            bool isCubic = sampler.interpolation == GltfInterpolation.CubicSpline;
            int keyCount = times.Length;

            AnimationCurve curveX = new AnimationCurve();
            AnimationCurve curveY = new AnimationCurve();
            AnimationCurve curveZ = new AnimationCurve();

            for (int i = 0; i < keyCount; i++)
            {
                float time = times[i];
                Vector3 value;

                if (isCubic)
                {
                    int idx = i * 3 + 1;
                    value = idx < values.Length ? values[idx] : Vector3.one;
                }
                else
                {
                    int idx = Math.Min(i, values.Length - 1);
                    value = values[idx];
                }

                // Scale doesn't need coordinate conversion
                curveX.AddKey(time, value.x);
                curveY.AddKey(time, value.y);
                curveZ.AddKey(time, value.z);
            }

#if IL2CPP
            clip.SetCurve(nodePath, Il2CppInterop.Runtime.Il2CppType.Of<Transform>(), "localScale.x", curveX);
            clip.SetCurve(nodePath, Il2CppInterop.Runtime.Il2CppType.Of<Transform>(), "localScale.y", curveY);
            clip.SetCurve(nodePath, Il2CppInterop.Runtime.Il2CppType.Of<Transform>(), "localScale.z", curveZ);
#else
            clip.SetCurve(nodePath, typeof(Transform), "localScale.x", curveX);
            clip.SetCurve(nodePath, typeof(Transform), "localScale.y", curveY);
            clip.SetCurve(nodePath, typeof(Transform), "localScale.z", curveZ);
#endif
        }

        private static void ApplyMorphTargetAnimation(
            GltfLoadContext context,
            AnimationClip clip,
            GltfAnimationSampler sampler,
            float[] times,
            string nodePath,
            int nodeIndex)
        {
            if (!context.Options.ImportBlendShapes)
            {
                return;
            }

            float[]? weights = ReadFloatArray(context, sampler.output);
            if (weights == null || weights.Length == 0)
            {
                return;
            }

            // Determine number of morph targets from the mesh
            GltfRoot gltf = context.Root;
            if (gltf.nodes == null || nodeIndex < 0 || nodeIndex >= gltf.nodes.Count)
            {
                return;
            }

            GltfNode node = gltf.nodes[nodeIndex];
            if (!node.mesh.HasValue || gltf.meshes == null || node.mesh.Value >= gltf.meshes.Count)
            {
                return;
            }

            GltfMesh mesh = gltf.meshes[node.mesh.Value];
            int targetCount = 0;

                if (mesh.primitives != null && mesh.primitives.Count > 0 && mesh.primitives[0].targets != null)
            {
                targetCount = mesh.primitives[0].targets!.Count;
            }

            if (targetCount == 0)
            {
                return;
            }

            bool isCubic = sampler.interpolation == GltfInterpolation.CubicSpline;
            int keyCount = times.Length;
            int weightsPerKey = isCubic ? targetCount * 3 : targetCount;

            AnimationCurve[] curves = new AnimationCurve[targetCount];
            for (int t = 0; t < targetCount; t++)
            {
                curves[t] = new AnimationCurve();
            }

            for (int i = 0; i < keyCount; i++)
            {
                float time = times[i];

                for (int t = 0; t < targetCount; t++)
                {
                    int idx;
                    if (isCubic)
                    {
                        idx = i * weightsPerKey + targetCount + t; // Skip in-tangent
                    }
                    else
                    {
                        idx = i * targetCount + t;
                    }

                    float weight = idx < weights.Length ? weights[idx] * 100f : 0f; // Unity uses 0-100
                    curves[t].AddKey(time, weight);
                }
            }

            for (int t = 0; t < targetCount; t++)
            {
                string property = $"blendShape.{t}";
#if IL2CPP
                clip.SetCurve(nodePath, Il2CppInterop.Runtime.Il2CppType.Of<SkinnedMeshRenderer>(), property, curves[t]);
#else
                clip.SetCurve(nodePath, typeof(SkinnedMeshRenderer), property, curves[t]);
#endif
            }
        }

        private static string? GetNodePath(GltfLoadContext context, int nodeIndex)
        {
            Transform? transform = context.GetNodeTransform(nodeIndex);
            if (transform == null)
            {
                return null;
            }

            // Build path from root
            List<string> pathParts = new List<string>();
            Transform current = transform;

            // Find the root (where parent is the model root or null)
            while (current != null && current.parent != null)
            {
                pathParts.Insert(0, current.name);
                current = current.parent;

                // Stop if we hit the model root
                if (current.name == context.Options.RootName)
                {
                    break;
                }
            }

            return string.Join("/", pathParts);
        }

        private static float[]? ReadFloatArray(GltfLoadContext context, int accessorIndex)
        {
            GltfAccessor? accessor = context.GetAccessor(accessorIndex);
            if (accessor == null)
            {
                return null;
            }

            byte[]? data = context.GetAccessorData(accessorIndex);
            if (data == null)
            {
                return null;
            }

            GltfBufferView? view = context.GetBufferView(accessor.bufferView ?? 0);
            int stride = view?.byteStride ?? 4;
            int count = accessor.count;

            float[] result = new float[count];
            for (int i = 0; i < count; i++)
            {
                int offset = accessor.byteOffset + i * stride;
                if (offset + 4 <= data.Length)
                {
                    result[i] = BitConverter.ToSingle(data, offset);
                }
            }

            return result;
        }

        private static Vector3[]? ReadVector3Array(GltfLoadContext context, int accessorIndex)
        {
            GltfAccessor? accessor = context.GetAccessor(accessorIndex);
            if (accessor == null)
            {
                return null;
            }

            byte[]? data = context.GetAccessorData(accessorIndex);
            if (data == null)
            {
                return null;
            }

            GltfBufferView? view = context.GetBufferView(accessor.bufferView ?? 0);
            int stride = view?.byteStride ?? 12;
            int count = accessor.count;

            Vector3[] result = new Vector3[count];
            for (int i = 0; i < count; i++)
            {
                int offset = accessor.byteOffset + i * stride;
                if (offset + 12 <= data.Length)
                {
                    float x = BitConverter.ToSingle(data, offset);
                    float y = BitConverter.ToSingle(data, offset + 4);
                    float z = BitConverter.ToSingle(data, offset + 8);
                    result[i] = new Vector3(x, y, z);
                }
            }

            return result;
        }

        private static Vector4[]? ReadVector4Array(GltfLoadContext context, int accessorIndex)
        {
            GltfAccessor? accessor = context.GetAccessor(accessorIndex);
            if (accessor == null)
            {
                return null;
            }

            byte[]? data = context.GetAccessorData(accessorIndex);
            if (data == null)
            {
                return null;
            }

            GltfBufferView? view = context.GetBufferView(accessor.bufferView ?? 0);
            int stride = view?.byteStride ?? 16;
            int count = accessor.count;

            Vector4[] result = new Vector4[count];
            for (int i = 0; i < count; i++)
            {
                int offset = accessor.byteOffset + i * stride;
                if (offset + 16 <= data.Length)
                {
                    float x = BitConverter.ToSingle(data, offset);
                    float y = BitConverter.ToSingle(data, offset + 4);
                    float z = BitConverter.ToSingle(data, offset + 8);
                    float w = BitConverter.ToSingle(data, offset + 12);
                    result[i] = new Vector4(x, y, z, w);
                }
            }

            return result;
        }

        #endregion
    }
}
