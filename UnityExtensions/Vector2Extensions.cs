using UnityEngine;

namespace UnityExtensions;

public static class Vector2Extensions
{
    public static float DistanceTo(this Vector2 from, Vector2 to) => Vector2.Distance(from, to);

    public static Vector2 DirectionTo(this Vector2 from, Vector2 to, bool normalize = false) =>
        normalize switch
        {
            true  => (to - from).normalized,
            false => to - from,
        };

    public static float AngleTo(this Vector2 from, Vector2 to) => Vector2.Angle(from, to);

    public static bool InRangeOf(this Vector2 center, Vector2 pointToCheck, float radius) =>
        center.DistanceTo(pointToCheck) <= radius;

    public static bool InViewAngle(this Vector2 viewDirection, Vector2 pointToCheck, float angle) =>
        AngleTo(viewDirection, pointToCheck) <= Mathf.Clamp(angle, 0, 180);

    public static bool InView(this Vector2 viewDirection, Vector2 pointToCheck, float angle, float radius) =>
        InViewAngle(viewDirection, pointToCheck, angle) && InRangeOf(viewDirection, pointToCheck, radius);

    public static float SignedAngleTo(this Vector2 from, Vector2 to) => Vector2.SignedAngle(from, to);

    public static Vector2 RandomBetween(this Vector2 from, Vector2 to) =>
        new(Random.Range(from.x, to.x), Random.Range(from.y, to.y));
}