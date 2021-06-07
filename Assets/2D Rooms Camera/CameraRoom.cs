using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRoom : MonoBehaviour {
    public Vector2 XLimits;
    public Vector2 YLimits;

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.GetComponent<PlayerController>()) {
            CameraManager.Instance.SetRoom(this);
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if(collision.GetComponent<PlayerController>() && CameraManager.Instance.CurrentRoom != this) {
            CameraManager.Instance.SetRoom(this);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Vector2 corner1 = (Vector2)transform.position + new Vector2(XLimits.x, YLimits.y);
        Vector2 corner2 = (Vector2)transform.position + new Vector2(XLimits.x, YLimits.x);
        Vector2 corner3 = (Vector2)transform.position + new Vector2(XLimits.y, YLimits.x);
        Vector2 corner4 = (Vector2)transform.position + new Vector2(XLimits.y, YLimits.y);

        Gizmos.DrawLine(corner1, corner2);
        Gizmos.DrawLine(corner1, corner4);
        Gizmos.DrawLine(corner4, corner3);
        Gizmos.DrawLine(corner3, corner2);
    }
}