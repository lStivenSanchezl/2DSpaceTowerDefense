using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretSlomo : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private GameObject upgradeUI;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private SpriteRenderer spriteRenderer; // Add reference to the SpriteRenderer

    [Header("Attributes")]
    [SerializeField] private float targetingRange = 3f;
    [SerializeField] private float aps = 4f; // Attacks Per Second
    [SerializeField] private float freezeTime = 1f;
    [SerializeField] private int baseUpgradeCost;
    [SerializeField] private int maxFreezeCount = 7; // Maximum freeze count for this turret

    [Header("Sprites")]
    [SerializeField] private Sprite[] turretSprites; // Array of sprites for each upgrade level

    private float timeUntilFire;
    private int level = 1;
    private bool upgradedThisSession = false; // Variable to track if the turret has been upgraded in the current session
    private int currentFreezeCount; // Variable to track the current freeze count remaining

    private float apsBase;
    private float targetingRangeBase;
    private float freezeTimeBase;

    private void Start()
    {
        apsBase = aps;
        targetingRangeBase = targetingRange;
        freezeTimeBase = freezeTime;

        upgradeButton.onClick.AddListener(Upgrade);
        currentFreezeCount = maxFreezeCount; // Set the initial value of currentFreezeCount
    }

    private void Update()
    {
        timeUntilFire += Time.deltaTime;

        if (timeUntilFire >= 1f / aps)
        {
            FreezeEnemies();
            timeUntilFire = 0f;

            // Reducir la cantidad de usos de congelamiento y ajustar la opacidad del sprite
            currentFreezeCount--;
            AdjustSpriteOpacity();
        }
    }

    // Método para ajustar la opacidad del sprite
    // Método para ajustar la opacidad del sprite
    private void AdjustSpriteOpacity()
    {
        // Calcular la opacidad según la cantidad de usos de congelamiento restantes, limitada a un mínimo de 100
        float opacity = Mathf.Clamp01((float)currentFreezeCount / maxFreezeCount);
        opacity = Mathf.Max(opacity, 0.39f); // Limitar la opacidad mínima a 0.39 (aproximadamente 100 en escala de 0 a 1)
        spriteRenderer.color = new Color(1f, 1f, 1f, opacity); // Asignar la nueva opacidad al color del sprite

        // Si la cantidad de usos de congelamiento llega a 0, destruir el GameObject
        if (currentFreezeCount <= 0 && !upgradedThisSession)
        {
            CloseUpgradeUI(); // Cierra el UI de mejora cuando la torreta se destruye
            Destroy(gameObject);
        }
    }


    private void FreezeEnemies()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)transform.position, 0f, enemyMask);

        if (hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit2D hit = hits[i];

                EnemyMovement em = hit.transform.GetComponent<EnemyMovement>();
                em.SetMoveSpeed(0.5f); // Change UpdateSpeed to SetMoveSpeed

                StartCoroutine(ResetEnemySpeed(em));
            }
        }
    }

    private IEnumerator ResetEnemySpeed(EnemyMovement em)
    {
        yield return new WaitForSeconds(freezeTime);

        em.ResetSpeed();
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

        aps = CalculateAPS();
        targetingRange = CalculateRange();

        // Cambiar el sprite al nivel de mejora correspondiente
        if (level <= turretSprites.Length)
        {
            spriteRenderer.sprite = turretSprites[level - 1];
        }

        // Calcular la cantidad total de usos de congelamiento después de la mejora
        int totalFreezeCountAfterUpgrade = currentFreezeCount + maxFreezeCount + 3;

        // Asignar la nueva cantidad total de usos de congelamiento
        currentFreezeCount = totalFreezeCountAfterUpgrade;

        upgradedThisSession = true;

        CloseUpgradeUI();
    }


    private int CalculateCost()
    {
        int cost = baseUpgradeCost + Mathf.RoundToInt(baseUpgradeCost * Mathf.Pow(level, 0.5f));
        return cost;
    }

    private float CalculateAPS()
    {
        return apsBase * Mathf.Pow(level, 0.6f);
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
