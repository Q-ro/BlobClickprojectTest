using UnityEngine;
using UnityEngine.EventSystems;


public class Block : MonoBehaviour, IPointerClickHandler
{
    public Color colorId; // The color of the block
    [HideInInspector] public int gridX, gridY; // position in grid
    GridManager gridManager; // the manager object


    SpriteRenderer sr;


    void Awake()
    {
        sr = GetComponent<SpriteRenderer>(); // store the reference
    }

    // Init the properties of the block Gameobject
    public void Initialize(Color _colorId, int x, int y, GridManager gm)
    {
        colorId = _colorId;
        gridX = x;
        gridY = y;
        gridManager = gm;
        sr.color = _colorId ;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (gridManager != null)
            gridManager.OnBlockClicked(gridX, gridY);
    }
}