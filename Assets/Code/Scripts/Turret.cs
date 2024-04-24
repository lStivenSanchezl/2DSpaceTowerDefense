using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Turret : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private GameObject upgradeUI;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private SpriteRenderer spriteRenderer; // Add reference to the SpriteRenderer

    [Header("Attributes")]
    [SerializeField] private float targetingRange = 3f;
    [SerializeField] private float bps = 1f; // Bullets Per Second
    [SerializeField] private int baseUpgradeCost;
    [SerializeField] private int maxBullets = 12; // Maximum bullets this turret can shoot

    [Header("Sprites")]
    [SerializeField] private Sprite[] turretSprites; // Array of sprites for each upgrade level

    private float bpsBase;
    private float targetingRangeBase;
    private bool upgradedThisSession = false; // New variable to track if the turret has been upgraded in the current session

    private Transform target;
    private float timeUntilFire;
    private int currentBullets; // Variable to track the current bullets remaining

    private int level = 1;

    private void Start()
    {
        bpsBase = bps;
        targetingRangeBase = targetingRange;

        upgradeButton.onClick.AddListener(Upgrade);
        currentBullets = maxBullets; // Set the initial value of currentBullets
    }

    private void Update()
    {
        if (target == null)
        {
            FindTarget();
            return;
        }

        if (!CheckTargetIsInRange())
        {
            target = null;
        }
        else
        {
            timeUntilFire += Time.deltaTime;

            if (timeUntilFire >= 1f / bps)
            {
                Shoot();
                timeUntilFire = 0f;

                // Reducir la cantidad de balas y ajustar la opacidad del sprite
                currentBullets--;
                AdjustSpriteOpacity();
            }
        }
    }

    // Método para ajustar la opacidad del sprite
    private void AdjustSpriteOpacity()
    {
        float opacity = Mathf.Clamp01((float)currentBullets / maxBullets); // Calcular la opacidad según la cantidad de balas restantes
        opacity = Mathf.Max(opacity, 0.39f); // Limitar la opacidad mínima a 0.39 (aproximadamente 100 en escala de 0 a 1)
        spriteRenderer.color = new Color(1f, 1f, 1f, opacity); // Asignar la nueva opacidad al color del sprite

        // Si la cantidad de usos de congelamiento llega a 0, destruir el GameObject
        if (currentBullets <= 0 && !upgradedThisSession)
        {
            CloseUpgradeUI(); // Cierra el UI de mejora cuando la torreta se destruye
            Destroy(gameObject);
        }
    }


    private void Shoot()
    {
        GameObject bulletObj = Instantiate(bulletPrefab, firingPoint.position, Quaternion.identity);
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        bulletScript.SetTarget(target);
    }

    private void FindTarget()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)transform.position, 0f, enemyMask);

        if (hits.Length > 0)
        {
            target = hits[0].transform;
        }
    }

    private bool CheckTargetIsInRange()
    {
        if (target == null)
        {
            return false;
        }

        return Vector2.Distance(target.position, transform.position) < targetingRange;
    }

    public void OpenUpgradeUI()
    {
        upgradeUI.SetActive(true);
        upgradedThisSession = false; // Reset the upgrade status when the upgrade UI is opened
    }

    public void CloseUpgradeUI()
    {
        upgradeUI.SetActive(false);
        UIManager.main.SetHoveringState(false);
    }

    public void Upgrade()
    {
        if (upgradedThisSession || CalculateCost() > LevelManager.main.currency) return; // Verificar si ya se ha mejorado en esta sesión

        LevelManager.main.SpendCurrency(CalculateCost());

        level++;

        bps = CalculateBPS();
        targetingRange = CalculateRange();

        // Cambiar el sprite al nivel de mejora correspondiente
        if (level <= turretSprites.Length)
        {
            spriteRenderer.sprite = turretSprites[level - 1];
        }

        // Calcular la cantidad total de balas después de la mejora
        int totalBulletsAfterUpgrade = currentBullets + maxBullets + 3;

        // Asignar la nueva cantidad total de balas
        currentBullets = totalBulletsAfterUpgrade;

        upgradedThisSession = true;

        CloseUpgradeUI();
    }


    private int CalculateCost()
    {
        int cost = baseUpgradeCost + Mathf.RoundToInt(baseUpgradeCost * Mathf.Pow(level, 0.5f));
        return cost;
    }

    private float CalculateBPS()
    {
        return bpsBase * Mathf.Pow(level, 0.6f);
    }

    private float CalculateRange()
    {
        return targetingRangeBase * Mathf.Pow(level, 0.4f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, targetingRange);
    }
}
