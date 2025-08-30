using UnityEngine;

public class TF_shield : MonoBehaviour
{
    public GameObject shieldPrefab;

    private GameObject shieldInstance;

    public Transform firePointUp;
    public Transform firePointDown;
    public Transform firePointLeft;
    public Transform firePointRight;

    private Coroutine shieldCoroutine;
    private PlayerController playerController;

    private Vector2 lastShieldDirection;

    private bool shieldEnabled = false;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        if (playerController == null && transform.parent != null)
            playerController = transform.parent.GetComponent<PlayerController>();
    }

    void Start()
    {
       
    }

    void Update()
    {
        if (shieldEnabled && playerController != null)
        {
            Vector2 currentOppositeDir = -playerController.LastMoveDirection;
            if (currentOppositeDir != lastShieldDirection)
            {
                lastShieldDirection = currentOppositeDir;
                SpawnShield(currentOppositeDir, 3f);
            }
        }
        UpdateShieldPosition();
    }

    private void UpdateShieldPosition()
    {
        if (shieldInstance != null && playerController != null)
        {
            Vector2 oppositeDir = -playerController.LastMoveDirection;
            Transform selectedFirePoint = SelectFirePoint(oppositeDir);
            if (selectedFirePoint != null)
            {
                shieldInstance.transform.position = selectedFirePoint.position;
                shieldInstance.transform.rotation = selectedFirePoint.rotation;
                shieldInstance.transform.localPosition = new Vector3(0, 0, -1f);
                shieldInstance.transform.localRotation = Quaternion.identity;
            }
        }
    }

    public void SpawnShield(Vector2 direction, float duration = 3f)
    {
        shieldEnabled = true;
        Transform selectedFirePoint = SelectFirePoint(direction);
        if (shieldPrefab != null && selectedFirePoint != null)
        {
            if (shieldInstance != null)
                Destroy(shieldInstance);
            shieldInstance = Instantiate(shieldPrefab, selectedFirePoint.position, selectedFirePoint.rotation, selectedFirePoint);
            shieldInstance.transform.localPosition = new Vector3(0, 0, -1f);
            shieldInstance.transform.localRotation = Quaternion.identity;
            if (shieldCoroutine != null)
                StopCoroutine(shieldCoroutine);
            shieldCoroutine = StartCoroutine(ShieldTimer(duration));
        }
    }

    private System.Collections.IEnumerator ShieldTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (shieldInstance != null)
            Destroy(shieldInstance);
        shieldEnabled = false;
    }

    public void SpawnShieldOpposite(Vector2 shootDirection)
    {
        Vector2 oppositeDir = -shootDirection;
        Transform selectedFirePoint = SelectFirePoint(oppositeDir);
        if (selectedFirePoint == null || shieldPrefab == null) return;
        if (shieldInstance != null)
            Destroy(shieldInstance);
        shieldInstance = Instantiate(shieldPrefab, selectedFirePoint.position, selectedFirePoint.rotation, selectedFirePoint);
        var pos = shieldInstance.transform.localPosition;
        shieldInstance.transform.localPosition = new Vector3(pos.x, pos.y, -1f);
    }

    private Transform SelectFirePoint(Vector2 direction)
    {
        if (Vector2.Angle(direction, Vector2.up) < 45f) return firePointUp;
        if (Vector2.Angle(direction, Vector2.down) < 45f) return firePointDown;
        if (Vector2.Angle(direction, Vector2.left) < 45f) return firePointLeft;
        if (Vector2.Angle(direction, Vector2.right) < 45f) return firePointRight;
        return firePointRight;
    }
}
