using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public const float SMALL_FLOAT = 0.001f;

    public static Color ModifyAlpha(Color _color, float _alpha)
    {
        _color.a = _alpha;
        return _color;
    }

    public static void ModifyAlpha(Renderer _renderer, float _alpha)
    {
        _renderer.material.color = ModifyAlpha(_renderer.material.color, _alpha);
    }

    public static Ray GetBlockDownwardRay(Block _block, Vector3 _impactPoint, float _gapWidth)
    {
        var tempVector = Camera.main.transform.position - _impactPoint;
        var rayOrigin = _impactPoint + SMALL_FLOAT * tempVector;
        var dirList = new List<Vector3>() { Vector3.right, -Vector3.right, Vector3.forward, -Vector3.forward, Vector3.up, -Vector3.up };

        RaycastHit raycastHit;
        foreach (var dir in dirList) {
            if (Physics.Raycast(new Ray(rayOrigin, dir), out raycastHit)) {
                var target = raycastHit.collider.gameObject;
                if (target == _block.gameObject)
                    return new Ray(rayOrigin += _gapWidth * -dir, -Vector3.up);
            }
        }
        return new Ray();
    }

    public static string GetBlockManipulationModeText(BlockManager.EnumMode _mode)
    {
        if (_mode == BlockManager.EnumMode.placing)
            return "Block Placing Mode";
        if (_mode == BlockManager.EnumMode.cloning)
            return "Block Cloning Mode";
        return "Invalid Mode";
    }
}
