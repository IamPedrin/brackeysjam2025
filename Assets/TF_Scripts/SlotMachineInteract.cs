using UnityEngine;
using UnityEngine.InputSystem;

public class SlotMachineInteract : MonoBehaviour
{
    public TF_cacaniqueis slotMachine;
    public int[] betOptions = new int[] { 500, 1000, 2000 };
    private int betIndex = 0;
    private int betAmount = 500;
    private InputAction interactAction;
    private InputAction changeBetAction;
    private bool playerInRange = false;
    private PlayerController playerController;
    private PlayerStats playerStats;

    void Awake()
    {
        interactAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/e");
        changeBetAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/f");
        changeBetAction.performed += ctx => OnChangeBetPerformed(ctx);
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerController = playerObj.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerStats = playerController.stats;
            }
        }
    }

    void OnEnable()
    {
        interactAction.Enable();
        changeBetAction.Enable();
    }

    void OnDisable()
    {
        interactAction.Disable();
        changeBetAction.Disable();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void Update()
    {
        if (!playerInRange) return;
        if (interactAction.WasPressedThisFrame())
        {
            int chips = playerStats != null ? playerStats.chips : 0;
            if (playerController == null || playerStats == null)
            {
                Debug.Log("PlayerStats não encontrado!");
            }
            else if (chips < betAmount)
            {
                Debug.Log("Dinheiro insuficiente para apostar!");
            }
            else
            {
                slotMachine.Apostar(betAmount);
                Debug.Log($"Bet placed: {betAmount}");
            }
        }
    }

    private void OnChangeBetPerformed(InputAction.CallbackContext ctx)
    {
        if (!playerInRange) return;
        betIndex = (betIndex + 1) % betOptions.Length;
        betAmount = betOptions[betIndex];
        Debug.Log($"Bet amount changed to {betAmount}");
    }
}
