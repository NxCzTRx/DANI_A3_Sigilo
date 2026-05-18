using UnityEngine;

namespace Features.AI.Definitions
{
    public interface IDamageableByPlayer
    {
        Transform Transform { get; }
        void TakeDamage();
    }
}
