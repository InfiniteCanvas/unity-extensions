using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace UnityExtensions
{
    public static class Vector3Extensions
    {
        public static float DistanceTo(this Vector3 from, Vector3 to) => Vector3.Distance(from, to);

        public static float[] DistanceTo(this Vector3         from,
                                         IEnumerable<Vector3> to,
                                         bool                 useJobs = false,
                                         int                  batch   = 16)
        {
            if (!useJobs) return to.Select(v => v.DistanceTo(from)).ToArray();
            // allocate
            Vector3[] targets = to.ToArray();
            var lenght = targets.Length;
            var input = new NativeArray<Vector3>(lenght, Allocator.TempJob);
            for (var i = 0; i < lenght; i++) input[i] = targets[i];
            var output = new NativeArray<float>(lenght, Allocator.TempJob);
            var job = new DistanceJob { _input = input, _output = output, _referencePoint = from };

            // schedule and wait for completion
            job.Schedule(output.Length, batch).Complete();

            // save result and dispose allocations
            var result = output.ToArray();
            input.Dispose();
            output.Dispose();

            return result;
        }

        public static Vector3 DirectionTo(this Vector3 from, Vector3 to, bool normalize = false) =>
            normalize switch
            {
                true  => Vector3.Normalize(to - from),
                false => to - from,
            };

        public static float AngleTo(this Vector3 from, Vector3 to) => Vector3.Angle(from, to);

        public static bool InRangeOf(this Vector3 center, Vector3 pointToCheck, float radius) =>
            center.DistanceTo(pointToCheck) <= radius;

        public static bool InViewAngle(this Vector3 viewDirection, Vector3 pointToCheck, float angle) =>
            Vector3.Angle(viewDirection, pointToCheck) <= Mathf.Clamp(angle, 0, 180);

        public static bool InView(this Vector3 viewDirection, Vector3 pointToCheck, float angle, float radius) =>
            InViewAngle(viewDirection, pointToCheck, angle) && InRangeOf(viewDirection, pointToCheck, radius);

        public static float SignedAngle(this Vector3 from, Vector3 to, Vector3 normal) =>
            Vector3.SignedAngle(from, to, normal);

        public static Vector3 RandomBetween(this Vector3 a, Vector3 b) =>
            new(Random.Range(a.x, b.x),
                Random.Range(a.y, b.y),
                Random.Range(a.z, b.z));

        public static Vector3 RandomOnLineBetween(this Vector3 a, Vector3 b) => a + (b - a) * Random.value;

        public static Quaternion RotationTo(this Vector3 from, Vector3 to) => Quaternion.FromToRotation(from, to);

        private struct DistanceJob : IJobParallelFor
        {
            [ReadOnly]  internal NativeArray<Vector3> _input;
            [ReadOnly]  internal Vector3              _referencePoint;
            [WriteOnly] internal NativeArray<float>   _output;

            public void Execute(int index) => _output[index] = Vector3.Distance(_input[index], _referencePoint);
        }
    }
}