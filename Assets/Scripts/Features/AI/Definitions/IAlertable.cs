using UnityEngine;

namespace Features.AI.Definitions
{
    public interface IAlertable
    {
        void OnHearNoise(Vector3 position);
    }
}
