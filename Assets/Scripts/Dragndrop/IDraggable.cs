using UnityEngine;

namespace Playnera_Test
{
    public interface IDraggable
    {
        Vector2 Position { get; set; }
        Transform PivotPoint { get; }
        void DragStart();
        void Drop();
        void Place(Vector2 position);
    }
}