using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class NodeTile : MonoBehaviour, IPointerClickHandler
{
    public TileType tileType;
    public List<Direction> connections;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        SetInvalidColor();
    }

    // New Input System (touch + mouse)
    public void OnPointerClick(PointerEventData eventData)
    {
        RotateTile();
    }

    void RotateTile()
    {
        transform.Rotate(0, 0, -90f);

        for (int i = 0; i < connections.Count; i++)
            connections[i] = (Direction)(((int)connections[i] + 1) % 4);

        LevelManager.Instance?.CheckLevel();
    }

    public NodeTile GetNeighbor(Direction dir)
    {
        Vector2 dirVec = Vector2.zero;

        switch (dir)
        {
            case Direction.Up: dirVec = Vector2.up; break;
            case Direction.Down: dirVec = Vector2.down; break;
            case Direction.Left: dirVec = Vector2.left; break;
            case Direction.Right: dirVec = Vector2.right; break;
        }

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position + (Vector3)dirVec * 0.6f,
            dirVec,
            0.6f
        );

        return hit.collider ? hit.collider.GetComponent<NodeTile>() : null;
    }

    public void SetValidColor() => sr.color = Color.green;
    public void SetInvalidColor() => sr.color = Color.white;
}
