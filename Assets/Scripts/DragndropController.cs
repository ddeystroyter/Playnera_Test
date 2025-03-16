using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Playnera_Test
{
    public class DragndropController : MonoBehaviour
    {
        [SerializeReference] 
        public DragndropActions inputActions = new();

        public CameraController CameraController;
        private Camera ActiveCamera => CameraController.ActiveCamera; 

        private Vector2 pointerScreenPosition;
        private float _raycastDist = 20f;

        private Vector2 targetDragPosition;
        private float _itemFollowSpeed = 10f;

        private IDraggable draggedItem;
        private bool isClickActive = false;

        private float screenCameraMoveZoneFactor = 0.1f;
        private float cameraMoveLeftX = float.MinValue;
        private float cameraMoveRightX = float.MaxValue;

        private void Awake()
        {
            inputActions.Init();
            BindControls(inputActions);
        }
        private void Start()
        {
            if (CameraController.IsUnityNull())
            {
                Camera.main.TryGetComponent(out CameraController);
            }
            cameraMoveLeftX = Camera.main.pixelWidth * screenCameraMoveZoneFactor;
            cameraMoveRightX = Camera.main.pixelWidth - cameraMoveLeftX;
        }

        private void Update()
        {
            if (isClickActive && draggedItem != null)
            {
                if (Vector2.Distance(draggedItem.Position, targetDragPosition) > 0.01f)
                {
                    draggedItem.Position = Vector3.Slerp(draggedItem.Position, targetDragPosition, _itemFollowSpeed * Time.deltaTime);
                }
                // IsDragNearLeftScreenBound
                if (pointerScreenPosition.x < cameraMoveLeftX)
                {
                    CameraController.MoveActiveCameraByDeltaX(-0.1f);
                    targetDragPosition = Physics2D.Raycast(ActiveCamera.ScreenToWorldPoint((Vector3)pointerScreenPosition), Vector2.zero).point - (Vector2)draggedItem.PivotPoint.localPosition * (Vector2)draggedItem.PivotPoint.lossyScale;
                }
                // IsDragNearRightScreenBound
                else if (pointerScreenPosition.x > cameraMoveRightX)
                {
                    CameraController.MoveActiveCameraByDeltaX(0.1f);
                    targetDragPosition = Physics2D.Raycast(ActiveCamera.ScreenToWorldPoint((Vector3)pointerScreenPosition), Vector2.zero).point - (Vector2)draggedItem.PivotPoint.localPosition * (Vector2)draggedItem.PivotPoint.lossyScale;
                }
            }

        }

        private void BindControls(DragndropActions actions)
        {
            actions.ClickAction.started += (context) => OnClickActionStarted();
            actions.ClickAction.canceled += (context) => OnClickActionCancelled();

            actions.PositionAction.performed += (context) => pointerScreenPosition = context.ReadValue<Vector2>();

            actions.LookAction.performed += OnLookActionPerformed;
        }
        private void OnClickActionStarted()
        {
            //Debug.Log("Tap Started");
            if (draggedItem != null)
            {
                draggedItem?.Drop();
                draggedItem = null;
            }

            RaycastHit2D hit = Physics2D.Raycast(ActiveCamera.ScreenToWorldPoint((Vector3)pointerScreenPosition), Vector2.zero, _raycastDist, 1 << Globals.ItemsLayer);

            Debug.DrawRay(ActiveCamera.ScreenToWorldPoint((Vector3)pointerScreenPosition), Vector2.zero, UnityEngine.Color.red, 2f);
            Debug.DrawLine(hit.point, hit.point + new Vector2(0.05f,0.05f), UnityEngine.Color.red, 1f);

            if (!hit.collider.IsUnityNull())
            {
                if (hit.collider.TryGetComponent(out draggedItem))
                {
                    draggedItem.DragStart();
                    targetDragPosition = hit.point - (Vector2)draggedItem.PivotPoint.localPosition * (Vector2)draggedItem.PivotPoint.lossyScale;
                }
                else
                {
                    Debug.LogWarning("Item in ItemsLayer don't have DraggableItem component attached.");
                }
            }

            isClickActive = true;
        }
        private void OnClickActionCancelled()
        {
            //Debug.Log("TapCancelled");

            if (draggedItem != null)
            {
                var hit = Physics2D.Raycast(ActiveCamera.ScreenToWorldPoint((Vector3)pointerScreenPosition), Vector2.zero, _raycastDist, 1 << Globals.PlacementAreaLayer);
                if (hit.collider is not null)
                {
                    //targetDragPosition = hit.point - (Vector2)_draggedItem.PivotPoint.localPosition * (Vector2)_draggedItem.PivotPoint.lossyScale;
                    draggedItem.Place(hit.point - (Vector2)draggedItem.PivotPoint.localPosition * (Vector2)draggedItem.PivotPoint.lossyScale);
                }
                else
                {
                    draggedItem.Drop();
                }

                draggedItem = null;
            }

            isClickActive = false;
        }
        private void OnLookActionPerformed(InputAction.CallbackContext obj)
        {
            if (!isClickActive) return;

            if (draggedItem != null)
            {
                targetDragPosition = Physics2D.Raycast(ActiveCamera.ScreenToWorldPoint((Vector3)pointerScreenPosition), Vector2.zero).point - (Vector2)draggedItem.PivotPoint.localPosition * (Vector2)draggedItem.PivotPoint.lossyScale;
            }
            else
            {
                var delta = obj.ReadValue<Vector2>()/100 * (-1);
                CameraController.MoveActiveCameraByDeltaX(delta.x);
            }

        }


    }
}

