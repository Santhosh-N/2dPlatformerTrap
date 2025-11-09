using UnityEngine;

public class PlayerGun : TargetFinder
{
    [Header("Gun Settings")]
    public float fireRate = 0.3f;
    public float rotationSpeed = 720f;
    public float bulletSpeed = 10f;
    public int bulletDamage = 1;

    [Header("References")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public Player player;

    [Header("Visuals")]
    public Transform rangeCircle;
    public Color rangeColor = new Color(1f, 1f, 1f, 0.15f);
    [SerializeField] GameObject muzzleFlash;

    [Header("Debug")]
    public bool IsFacingRight { get; private set; } = true;

    private float fireCooldown;

    void Start()
    {
        UpdateRangeCircle();
    }

    void Update()
    {
        FindTarget();
        if (hasTarget)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        if (currentTarget != null)
        {
            RotateTowardsTarget();

            if (fireCooldown <= 0f)
            {
                Shoot();
                fireCooldown = fireRate;
            }
        }
        else if (muzzleFlash.activeInHierarchy)
            muzzleFlash.SetActive(false);

        if (fireCooldown > 0f)
            fireCooldown -= Time.deltaTime;
    }

    void UpdateRangeCircle()
    {
        if (!rangeCircle) return;
        float diameter = range * 2f;
        rangeCircle.localScale = new Vector3(diameter, diameter, 1f);
        var sr = rangeCircle.GetComponent<SpriteRenderer>();
        if (sr) sr.color = rangeColor;
    }

    void RotateTowardsTarget()
    {
        if (!currentTarget) return;

        Vector3 direction = currentTarget.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);


        // Determine facing direction (horizontal only)
        bool newFacingRight = direction.x > 0;
        if (newFacingRight != IsFacingRight)
        {
            IsFacingRight = newFacingRight;
            if (player) player.SetFacingDirection(IsFacingRight);
        }
    }

    void Shoot()
    {
        if (!bulletPrefab || !firePoint) return;

        GameObject bullet = ObjectPool.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = firePoint.right * bulletSpeed;

        Bullet b = bullet.GetComponent<Bullet>();
        if (b) b.damage = bulletDamage;

        if (!muzzleFlash.activeInHierarchy) muzzleFlash.SetActive(true);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
