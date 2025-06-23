using UnityEngine;
using UnityEngine.InputSystem;    // new Input System

/*

This is now a script on the sword1 weapon, could be useful to rename this in the future. can't be bothered with it rn

*/

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject attackPrefab;
    public float cooldown = 0.5f;
    public float spawnOffset = 1f;
    public Transform swordObj;


    [Header("Animation")]
    public Animator weaponAnimator;

    Camera cam;
    float nextAttackTime = 0f;

     
    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // Left-click
        if (Mouse.current.leftButton.wasPressedThisFrame && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + cooldown;
            // autoAttack();
            weaponAnimator.SetTrigger("AutoAttack");
        }
    }

    void SpawnHitbox()
    {
        // 1) World-space mouse position (at pivot’s Z)
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = cam.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, swordObj.position.z));

        // 2) Aim direction from the pivot
        Vector3 dir = (mouseWorld - swordObj.position).normalized;
        if (dir == Vector3.zero) dir = Vector3.right;

        // 3) Instantiate *under* the pivot so it moves with it
        var swish = Instantiate(attackPrefab, swordObj);

        // 4) Place it at the correct local offset & rotate
        swish.transform.localPosition = new Vector3();
        // float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        swish.transform.localRotation = Quaternion.Euler(0, 0, transform.rotation.z + 80);
    }
}
