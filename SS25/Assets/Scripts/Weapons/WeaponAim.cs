using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponAim : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;    // assign your Player
    public Animator weaponAnimator;      // assign your Weapon’s Animator

    [Header("Settings")]
    public float offsetDistance = 1f;    // how far out from the player

    Vector3 lastDir = Vector3.right;

    void LateUpdate()
    {
        // check if we're in the Swing state
        var st = weaponAnimator.GetCurrentAnimatorStateInfo(0);
        bool isSwinging = st.IsName("SwordSwingUp");

        if (!isSwinging)
        {
            // recompute aim only when not swinging
            Vector3 m = Mouse.current.position.ReadValue();
            Vector3 world = Camera.main.ScreenToWorldPoint(new Vector3(m.x, m.y, 0));
            world.z = 0;

            lastDir = (world - playerTransform.position).normalized;
        }

        // always follow the player at the (potentially frozen) direction
        transform.position = playerTransform.position + lastDir * offsetDistance;

        if (!isSwinging)
        {
            // only rotate when not swinging (Animation will drive rotation during Swing)
            float ang = Mathf.Atan2(lastDir.y, lastDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, ang - 90);
        }
    }
}
