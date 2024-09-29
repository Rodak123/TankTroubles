using UnityEngine;

public static class RectTransformExtensions
{
    public static Bounds GetWorldSpaceBounds(this RectTransform rectTransform)
    {
        Vector3[] worldCorners = new Vector3[4];
        rectTransform.GetWorldCorners(worldCorners);

        Vector3 center = (worldCorners[0] + worldCorners[2]) * 0.5f;

        Vector3 size = worldCorners[2] - worldCorners[0];

        return new Bounds(center, size);
    }

    public static Bounds OrthographicBounds(this Camera camera)
    {
        float screenAspect = (float)Screen.width / Screen.height;
        float cameraHeight = camera.orthographicSize * 2;
        Bounds bounds = new(
            camera.transform.position,
            new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
        return bounds;
    }
}