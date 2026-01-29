using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // Wichtig: Das neue Package einbinden

public class BuildingSystem : MonoBehaviour
{
    [Header("Einstellungen")]
    public GameObject prefabToBuild;
    public float transparency = 0.5f;

    private GameObject currentPreview;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(StartPlacement);
        }
    }

    void Update()
    {
        if (currentPreview != null)
        {
            MovePreviewToMouse();

            // Linksklick zum Platzieren (New Input System)
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                PlaceObject();
            }

            // Rechtsklick zum Abbrechen (New Input System)
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                Destroy(currentPreview);
            }
        }
    }

    public void StartPlacement()
    {
        if (currentPreview != null) return;

        currentPreview = Instantiate(prefabToBuild);
        ApplyTransparency(currentPreview, transparency);

        if (currentPreview.TryGetComponent<Collider>(out Collider col))
        {
            col.enabled = false;
        }
    }

    void MovePreviewToMouse()
    {
        // Mausposition im neuen System abfragen
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = mainCam.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            currentPreview.transform.position = hit.point;
        }
    }

    void PlaceObject()
    {
        Instantiate(prefabToBuild, currentPreview.transform.position, currentPreview.transform.rotation);
        Destroy(currentPreview);
    }

    void ApplyTransparency(GameObject target, float alpha)
    {
        foreach (Renderer rend in target.GetComponentsInChildren<Renderer>())
        {
            foreach (Material mat in rend.materials)
            {
                mat.SetFloat("_Mode", 3);
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.renderQueue = 3000;

                Color color = mat.color;
                color.a = alpha;
                mat.color = color;
            }
        }
    }
}