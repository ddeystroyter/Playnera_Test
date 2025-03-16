using UnityEngine;
using UnityEngine.InputSystem;

namespace Playnera_Test
{
    public class DragndropActions
    {
        [Header("InputActionAsset")]
        [SerializeReference] public InputActionAsset inputActionAsset;

        public string actionMapName = "Player";
        private InputActionMap actionMap;

        [HideInInspector] public InputAction ClickAction;
        [HideInInspector] public InputAction PositionAction;
        [HideInInspector] public InputAction LookAction;

        private string clickActionName = "Click";
        private string positionActionName = "Position";
        private string lookActionName = "Look";

        public bool Init()
        {
            actionMap = inputActionAsset.FindActionMap(actionMapName);

            ClickAction = actionMap.FindAction(clickActionName);
            PositionAction = actionMap.FindAction(positionActionName);
            LookAction = actionMap.FindAction(lookActionName);

            return true;
        }

    }
}