using UnityEngine;

enum DropState
{
    DROPPING,
    IDLE,
    PICKUP
}

public class Drop : MonoBehaviour
{
    public Transform slot;

    public float rotationSpeed;
    public float bounceSpeed;
    public float bounceSize;
    public float bounceOffset;
    
    public float pickupRange = 1f;

    Weapon droppedWeaponPrefab;
    Weapon droppedWeapon;

    DropState state;
    
    const float gravity = 18f;
    const float scaleRate = 2f;
    const float bounceDecay = 0.4f;
    const float relaxationThreshold = 0.5f;

    Vector3 velocity;
    float scale;

    void Start()
    {
        var deviation = new Vector3(Random.Range(0f, 2f), 0f, Random.Range(0f, 2f));
        var strongUp = Vector3.up * 9f;
        velocity = strongUp + deviation;

        state = DropState.DROPPING;
    }

    public void Set(Weapon weapon)
    {
        if (droppedWeapon != null)
            Destroy(droppedWeapon.gameObject);

        droppedWeaponPrefab = weapon;
        droppedWeapon = Instantiate(weapon, slot);
    }

    public Weapon GetLootPrefab()
    {
        return droppedWeaponPrefab;
    }
    
    void Update()
    {
        slot.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        slot.localPosition = Vector3.up * bounceOffset + Vector3.up * Mathf.Sin(Time.time * bounceSpeed) * bounceSize;
        transform.localScale = Vector3.one * scale;
    }

    void FixedUpdate()
    {
        if (state == DropState.DROPPING)
            FlyOutAndDropPhysics();

        if (state == DropState.IDLE)
        {
            var distanceToPlayer = Vector3.Distance(transform.position, Main.Get<Player>().GetPosition());
            if (distanceToPlayer < pickupRange)
                state = DropState.PICKUP;
        }
        
        if (state == DropState.PICKUP)
            FollowPlayerAndDie();
    }

    void FollowPlayerAndDie()
    {
        var playerPosition = Main.Get<Player>().GetPosition();
        transform.position = Vector3.Lerp(transform.position, playerPosition, 0.1f);
        scale *= 0.9f;
        
        const float threshold = 0.5f;
        if (Vector3.Distance(playerPosition, transform.position) < threshold)
        {
            Main.Get<GameEvents>().LootPickedUp?.Invoke(this);
            Destroy(gameObject);
        }
    }

    void FlyOutAndDropPhysics()
    {
        velocity += Vector3.down * gravity * Time.fixedDeltaTime;

        var pos = transform.position;

        pos += velocity * Time.fixedDeltaTime;

        if (pos.y < 0)
        {
            pos = new Vector3(pos.x, 0, pos.z);
            velocity.y = -velocity.y * bounceDecay;

            if (Mathf.Abs(velocity.y) < relaxationThreshold)
                state = DropState.IDLE;
        }

        transform.position = pos;

        if (scale < 1f)
            scale += Time.fixedDeltaTime * scaleRate;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}