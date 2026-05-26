using UnityEngine;
using UnityEngine.InputSystem;

public class Raycasttrees : MonoBehaviour
{
    public Camera playerCamera;
    public float interactDistance = 4f;
    public LayerMask treeLayer;

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
        Vector3 origin =
            playerCamera.transform.position +
            playerCamera.transform.forward * 0.5f;

        Ray ray = new Ray(
            origin,
            playerCamera.transform.forward);

        Debug.DrawRay(
            origin,
            playerCamera.transform.forward * interactDistance,
            Color.red,
            1f);

            Debug.Log ("Ray Cast");

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            interactDistance,
            treeLayer))
        {
            Tree tree = hit.collider.GetComponent<Tree>();
            Debug.Log("Ray hit something!");
            if (tree != null)
            {
                // Damage tree
                tree.Chop();
            }
        }
    }
}