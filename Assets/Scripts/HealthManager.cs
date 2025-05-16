using UnityEngine;
using UnityEngine.UI;

public class Healthmanager : MonoBehaviour
{
    public Image healthBar;
    public float healthAmount = 100f;
    public Vector3 respawnPosition;

    private bool isInWater = false;
    private float waterDamageRate = 1f;
    private float nextWaterDamageTime = 0f;

    void Start()
    {
        healthBar.fillAmount = healthAmount / 100f;
    }

    void Update()
    {
        if (healthAmount <= 0)
        {
            Die();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            TakeDamage(20);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Heal(5);
        }

        if (isInWater && Time.time >= nextWaterDamageTime)
        {
            TakeDamage(5);
            nextWaterDamageTime = Time.time + waterDamageRate;
        }
    }

    public void TakeDamage(float damage)
    {
        if (healthAmount <= 0) return;

        healthAmount -= damage;
        healthAmount = Mathf.Clamp(healthAmount, 0, 100);
        healthBar.fillAmount = healthAmount / 100f;
    }

    public void Heal(float healingAmount)
    {
        if (healthAmount <= 0) return;

        healthAmount += healingAmount;
        healthAmount = Mathf.Clamp(healthAmount, 0, 100);
        healthBar.fillAmount = healthAmount / 100f;
    }

    void Die()
    {
        Debug.Log("Le personnage est mort !");
        gameObject.SetActive(false);
        Invoke("Respawn", 2f);
    }

    void Respawn()
    {
        gameObject.SetActive(true);
        transform.position = respawnPosition;
        healthAmount = 100f;
        healthBar.fillAmount = healthAmount / 100f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            isInWater = true;
            nextWaterDamageTime = Time.time + waterDamageRate;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            isInWater = false;
        }
    }
}
