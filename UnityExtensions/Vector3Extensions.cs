using UnityEngine;

namespace UnityExtensions
{
    public static class Vector3Extensions
    {
        public static float DistanceTo(this Vector3 from, Vector3 to) => Vector3.Distance(from, to);

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

        public static Quaternion RotationTo(this Vector3 from, Vector3 to) => Quaternion.FromToRotation(from, to);
    }
}