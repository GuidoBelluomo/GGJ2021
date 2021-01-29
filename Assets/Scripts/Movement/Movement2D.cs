using Character;
using UnityEngine;

namespace Movement
{
    public class Movement2D : MonoBehaviour
    {
        private PlayerManager _playerManager;

        public PlayerManager GetPlayerManager()
        {
            return _playerManager;
        }

        public void SetPlayerManager(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }
    }
}
