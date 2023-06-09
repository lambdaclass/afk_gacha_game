using UnityEngine;

public class CoordinatesUtils
{
    public static Vector3 transformBackendPositionToFrontendPosition(Position position)
    {
        var x = (long)position.Y / 10f - 50.0f;
        var y = (-((long)position.X)) / 10f + 50.0f;
        return new Vector3(x, 1f, y);
    }
}
