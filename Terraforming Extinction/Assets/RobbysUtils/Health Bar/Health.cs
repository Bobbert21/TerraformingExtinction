using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] HealthBar healthBar;

    [SerializeField] int currentHealth;
    [SerializeField] int maxHealth;

    [SerializeField] CustomUnityEventInt OnDeath;

    //[SerializeField] Properties properties;

    public int CurrentHealth 
    {
        get => currentHealth;
        set => currentHealth = value;
    }

    void Start()
    {
        currentHealth = maxHealth;

        healthBar.SetFillColor(Color.green);
        healthBar.SetHealth((currentHealth / maxHealth) * 100);
    }

    void OnEnable()
    {
        healthBar.gameObject.SetActive(false);

        currentHealth = maxHealth;

        healthBar.SetFillColor(Color.green);
        healthBar.SetHealth((currentHealth / maxHealth) * 100);
    }

    void Update()
    {
        
    }

    public void SetMaxHealth(int maxHealth, bool setCurrentHealth)
    {
        this.maxHealth = maxHealth;

        if (setCurrentHealth)
            currentHealth = maxHealth;
    }

    public void TakeDamage(int damage, /*DamageType damageTypes,*/ int playerId, out bool isImmune, bool ignoreImmunities = false)
    {
        if (!healthBar.gameObject.activeInHierarchy)
            healthBar.gameObject.SetActive(true);

        if (ignoreImmunities)
        {
            isImmune = false;
        }
        else
        {
            //bool result = (properties.DamageTypes & damageTypes) != 0;
            //if (!result)
            //{
            //    isImmune = true;
            //    return;
            //}
            //else
                isImmune = false;
        }

        currentHealth -= damage;
        if (currentHealth < 0)
            currentHealth = 0;

        healthBar.SetHealth((float)currentHealth / (float)maxHealth * 100);

        if (currentHealth <= 0)
        {
            OnDeath?.Invoke(playerId);
            //gameObject.SetActive(false);
        }
    }

    public void TakeDamage(int damage, /*DamageType damageTypes,*/ int playerId, out bool isImmune, out bool isDead, bool ignoreImmunities = false)
    {
        if (!healthBar.gameObject.activeInHierarchy)
            healthBar.gameObject.SetActive(true);

        if (ignoreImmunities)
        {
            isImmune = false;
        }
        else
        {
            //bool result = (properties.DamageTypes & damageTypes) != 0;
            //if (!result)
            //{
            //    isImmune = true;
            //    isDead = false;
            //    return;
            //}
            //else
                isImmune = false;
        }

        currentHealth -= damage;
        if (currentHealth < 0)
            currentHealth = 0;

        healthBar.SetHealth((float)currentHealth / (float)maxHealth * 100);

        isDead = currentHealth <= 0;

        if (currentHealth <= 0)
        {
            OnDeath?.Invoke(playerId);
            //gameObject.SetActive(false);
        }
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public void Die(GameObject gameObject)
    {
        GameObject.Destroy(gameObject);
    }
}

[System.Serializable]
public class CustomUnityEventInt : UnityEvent<int>
{

}