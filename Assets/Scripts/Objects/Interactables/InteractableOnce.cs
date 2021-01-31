namespace Objects.Interactables
{
    public class InteractableOnce : InteractableObject
    {
        public override void Interact()
        {
            base.Interact();
            enabled = false;
        }
    }
}
