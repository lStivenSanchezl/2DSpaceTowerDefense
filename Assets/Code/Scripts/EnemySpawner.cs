using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private EnemyMovement enemyMovementPrefab;

    [Header("Attributes")]
    [SerializeField] private int baseEnemies = 8;
    [SerializeField] private float enemiesPerSecond = 0.5f;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float difficultyScalingFactor = 0.75f;
    [SerializeField] private float enemiesPerSecondCap = 15f;
    [SerializeField] private int baseEnemyHealth = 2; // Vida base del enemigo
    [SerializeField] private float baseEnemySpeed = 2f; // Velocidad base del enemigo
    [SerializeField] private int baseEnemyCurrencyValue = 50; // Valor base de la moneda que otorgan los enemigos

    [Header("Events")]
    public static UnityEvent onEnemyDestroy = new UnityEvent();

    public static EnemySpawner main; // Instancia estática de EnemySpawner

    private int currentWave = 1;
    private float timeSinceLastSpawn;
    private int enemiesAlive;
    private int enemiesLeftToSpawn;
    private float eps; // Enemies Per Second
    private bool isSpawning = false;

    private int currentEnemyCurrencyValue; // Valor actual de la moneda que otorgan los enemigos

    private void Awake()
    {
        main = this; // Asignar la instancia estática al iniciar
        onEnemyDestroy.AddListener(EnemyDestroyed);
        currentEnemyCurrencyValue = baseEnemyCurrencyValue;
    }

    private void Start()
    {
        StartCoroutine(StartWave());
    }

    private void Update()
    {
        if (!isSpawning) return;

        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= (1f / eps) && enemiesLeftToSpawn > 0)
        {
            SpawnEnemy();
            enemiesLeftToSpawn--;
            enemiesAlive++;
            timeSinceLastSpawn = 0f;
        }

        if (enemiesAlive == 0 && enemiesLeftToSpawn == 0)
        {
            EndWave();
        }
    }

    private void EnemyDestroyed()
    {
        enemiesAlive--;

        // Aumentar la moneda del jugador cuando un enemigo es destruido
        LevelManager.main.IncreaseCurrency(currentEnemyCurrencyValue);
        // También actualiza el valor de la moneda para el siguiente enemigo eliminado
        currentEnemyCurrencyValue = baseEnemyCurrencyValue + (currentWave * 10); // Por ejemplo, puedes aumentar el valor en 10 unidades por cada oleada completada
    }

    private IEnumerator StartWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);

        isSpawning = true;
        enemiesLeftToSpawn = EnemiesPerWave();
        eps = EnemiesPerSecond();
        AudioManager.instance.PlaySFX(5);

        // Aumentar la salud y velocidad de los enemigos
        IncreaseEnemyAttributes();

        // Aumentar el valor de la moneda que otorgan los enemigos
        IncreaseEnemyCurrencyValue();
    }

    private void EndWave()
    {
        isSpawning = false;
        timeSinceLastSpawn = 0f;
        currentWave++;
        StartCoroutine(StartWave());
    }

    private void SpawnEnemy()
    {
        int index = Random.Range(0, enemyPrefabs.Length);
        GameObject prefabToSpawn = enemyPrefabs[index];
        GameObject enemyObject = Instantiate(prefabToSpawn, LevelManager.main.startPoint.position, Quaternion.identity);
        EnemyMovement enemyMovement = enemyObject.GetComponent<EnemyMovement>();

        // Establecer la salud y velocidad aumentadas al enemigo que se está instanciando
        SetEnemyAttributes(enemyMovement);

        enemyMovement.enemySpawner = this;
    }

    private int EnemiesPerWave()
    {
        return Mathf.RoundToInt(baseEnemies * Mathf.Pow(currentWave, difficultyScalingFactor));
    }

    private float EnemiesPerSecond()
    {
        return Mathf.Clamp(enemiesPerSecond * Mathf.Pow(currentWave, difficultyScalingFactor), 0f, enemiesPerSecondCap);
    }

    private void IncreaseEnemyAttributes()
    {
        baseEnemyHealth += currentWave - 1; // Aumentar la salud del enemigo
        baseEnemySpeed *= 1.1f; // Aumentar la velocidad del enemigo
        baseEnemyCurrencyValue += currentWave * 5; // Aumentar el valor de la moneda del enemigo por cada oleada completada
        currentEnemyCurrencyValue += currentWave * 5; // Aumentar el valor de la moneda que otorgan los enemigos por cada oleada completada
    }


    private void SetEnemyAttributes(EnemyMovement enemyMovement)
    {
        // Establecer la salud y velocidad aumentadas al enemigo
        enemyMovement.SetMoveSpeed(baseEnemySpeed);
        HealthEnemy healthEnemy = enemyMovement.GetComponent<HealthEnemy>();
        if (healthEnemy != null)
        {
            healthEnemy.SetHealth(baseEnemyHealth);
        }
    }

    private void IncreaseEnemyCurrencyValue()
    {
        // Aumentar el valor de la moneda que otorgan los enemigos
        currentEnemyCurrencyValue += currentWave * 5; // Por ejemplo, puedes aumentar el valor en 10 unidades por cada oleada completada
    }

    public void EnemyReachedEnd(EnemyMovement enemy)
    {
        enemiesAlive--;
        AudioManager.instance.PlaySFX(2);
    }
}
