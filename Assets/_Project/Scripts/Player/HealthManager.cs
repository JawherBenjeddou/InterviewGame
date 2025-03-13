using UnityEngine;
using System;

namespace InterviewGame.player
{
    public class HealthManager : MonoBehaviour
    {
        [SerializeField] private float m_maxHealth = 100f;
        [SerializeField] private float m_currentHealth;

        public event Action OnHealthChanged; // For UI updates or other listeners
        public event Action OnDeath; // Triggered when health reaches 0

        private void Awake()
        {
            m_currentHealth = m_maxHealth; // Start at full health
        }

        public void TakeDamage(float damage)
        {
            m_currentHealth = Mathf.Max(m_currentHealth - damage, 0);
            OnHealthChanged?.Invoke();
            if (m_currentHealth <= 0)
            {
                Die();
            }
        }

        public void Heal(float amount)
        {
            m_currentHealth = Mathf.Min(m_currentHealth + amount, m_maxHealth);
            OnHealthChanged?.Invoke();
        }

        private void Die()
        {
            OnDeath?.Invoke();
        }

        public float GetCurrentHealth() => m_currentHealth;
        public float GetMaxHealth() => m_maxHealth;
        public float GetHealthPercentage() => m_currentHealth / m_maxHealth;
    }
}