using System;
using Character;
using Movement;
using UnityEngine;

namespace Objects.Environment
{
    public class VerticalBooster : MonoBehaviour
    {
        [SerializeField] private Vector2 force;
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.layer == 7)
            {
                other.gameObject.GetComponent<Rigidbody2D>().AddForce(force);
                return;
            }

            if (other.gameObject.layer != 3) return;
            
            PlayerManager playerManager = other.gameObject.GetComponent<PlayerManager>();
            if (playerManager == null) return;

            Movement2D movementController = playerManager.GetMovementController();
            switch (movementController)
            {
                case CharacterMovement2D characterMovement2D:
                    characterMovement2D.AddExternalMovement(force * Time.deltaTime);
                    break;
                case RollingMovement2D rollingMovement2D:
                    playerManager.GetRigidbody2D().AddForce(force);
                    break;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer != 3) return;
            other.gameObject.GetComponent<PlayerManager>().GetRollingMovement2D().SetBoosted(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer != 3) return;
            other.gameObject.GetComponent<PlayerManager>().GetRollingMovement2D().SetBoosted(false);
        }
    }
}
