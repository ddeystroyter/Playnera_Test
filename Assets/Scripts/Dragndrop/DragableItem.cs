using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace Playnera_Test 
{
    [RequireComponent(typeof(Rigidbody2D))]
    [DisallowMultipleComponent]
    public class DraggableItem : MonoBehaviour, IDraggable
    {
        public Vector2 Position { get => transform.position; set => transform.position = value; }
        public Transform PivotPoint { get => _pivotPoint; }
        [SerializeField] private Transform _pivotPoint;

        private Vector2 originalScale;
        private Vector2 pickedScale;
        private const float picked_ScaleAnimationDuration = 0.1f;
        private const float picked_ScaleMultiplier = 1.2f;

        private float dropStartY;
        private const float maxDropHeight = 2f;

        private Rigidbody2D _rigidbody;
        private Vector2 dragStartPosition;

        public void DragStart()
        {
            dragStartPosition = Position;
            StopRigidbody2D();
            transform.DOScale(pickedScale, 0.2f);
        }
        public void Drop()
        {
            dropStartY = Position.y;
            transform.DOScale(originalScale, picked_ScaleAnimationDuration);
           
            if (!IsItemFallInPlacementArea(Position))
            {
                transform.position = IsItemFallInPlacementArea(dragStartPosition) ? dragStartPosition : Globals.DefaultSpawnPoint;
            }
            StartRigidbody2D();
        }
        public void Place(Vector2 position)
        {
            transform.DOScale(originalScale, picked_ScaleAnimationDuration);
            Position = position;
        }

        protected void Start()
        {
            originalScale = transform.localScale;
            pickedScale = transform.localScale * picked_ScaleMultiplier;

            if (PivotPoint.IsUnityNull())
            {
                _pivotPoint = transform.Find("PivotPoint");
                if (PivotPoint.IsUnityNull())
                {
                    _pivotPoint = this.transform;
                }
            }

            _rigidbody = GetComponent<Rigidbody2D>();
            Drop();
        }
        protected void OnCollisionEnter2D(Collision2D collision)
        {
            //Debug.Log(collision.collider.name);
            // If item in "Drop" state
            if (_rigidbody.bodyType == RigidbodyType2D.Dynamic && collision.collider.gameObject.layer == Globals.PlacementAreaLayer)
            {
                StopRigidbody2D();
                DoSquashEffect();
            }
        }

        protected bool IsItemFallInPlacementArea(Vector2 startPosition)
        {
            var hit = Physics2D.Raycast(startPosition, Vector2.down, 20f, 1 << Globals.PlacementAreaLayer);
            if (hit.collider.IsUnityNull()) return false;
            return true;
            
        }

        protected void StartRigidbody2D()
        {
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody.constraints |= RigidbodyConstraints2D.FreezePositionX;
        }
        protected void StopRigidbody2D()
        {
            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            _rigidbody.angularVelocity = 0;
            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
        }

        protected void DoSquashEffect()
        {
            var dropHeight = Mathf.Abs(dropStartY - Position.y);
            var squashFactor = Mathf.Clamp(dropHeight / 2f, 0.1f, 0.6f);
            var squashTime = Mathf.Clamp(squashFactor, 0.1f, 0.3f);
            // Squash effect
            transform.DOScale(new Vector3(originalScale.x * (1 + squashFactor), originalScale.y * (1 - squashFactor), 1f), 0.1f)
                .OnComplete(() =>
                {
                    transform.DOScale(originalScale, squashTime).SetEase(Ease.InOutElastic);
                });
        }
    }
}

