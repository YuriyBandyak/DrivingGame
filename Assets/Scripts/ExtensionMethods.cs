using UnityEngine;

public static class ExtensionMethods
{
    public static int GetInverseLayerMask(string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        if (layer == -1)
        {
            Debug.LogError($"Layer '{layerName}' not found.");
            return 0;
        }
        return ~(1 << layer);
    }
}
