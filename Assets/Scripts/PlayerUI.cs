using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] RectTransform thrustFuelFill;
    private PlayerController controller;

    public void setController(PlayerController _controller)
    {
        controller = _controller;
    }

    private void Update()
    {
        setFuelAmount(controller.GetFuelAmount());
    }

    private void setFuelAmount(float fuelAmount)
    {
        thrustFuelFill.localScale = new Vector3 (1f, fuelAmount, 1f);
    }
}
