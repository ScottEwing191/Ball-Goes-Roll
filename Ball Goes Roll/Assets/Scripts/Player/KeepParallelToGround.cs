using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepParallelToGround : MonoBehaviour {
    [SerializeField] private PlayerController player;

    private void Update() {
        if (player.IsGrounded) {
            transform.position = player.transform.position;
            RaycastHit hit;
            Physics.Raycast(transform.position, Vector3.down, out hit, 5f, 1 << LayerMask.NameToLayer("Ground"));
            Vector3 perpendicularToGround = Vector3.Cross(Camera.main.transform.right, hit.normal);
            transform.LookAt(transform.position + perpendicularToGround, hit.normal);
        }
    }



    /*Vector3 MakeVectorParallelToGround(Vector3 vector) {
        RaycastHit hit;
        if (true) {
            //Physics.Raycast(transform.position, Vector3.down, out hit, (playerCollider.bounds.extents.y + 1f), 1 << LayerMask.NameToLayer("Ground"));
            print("Raycast Hit");
        }
        Vector3 perpendicularToGround = Vector3.Cross(Camera.main.transform.right, hit.normal);
        //Vector3 startOfLine = hit.point;
        Debug.DrawLine(transform.position, transform.position + (perpendicularToGround * 10), Color.black);
        Vector3 forceDirection = ((transform.position + perpendicularToGround) - transform.position).normalized;
        //Debug.DrawLine(transform.position, (transform.position + forceDirection) * 10, Color.blue);

        return forceDirection;
    }*/
}
