using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class MeleeState : GroundedState
    {
        private bool swingMelee;
        private bool sheatheMelee;
        private bool blockMelee;

        private bool jump;
        private bool crouch;

        public MeleeState(Character character, StateMachine stateMachine) : base(character, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            speed = character.MovementSpeed;
            rotationSpeed = character.RotationSpeed;
            swingMelee = false;
            sheatheMelee = false;
            blockMelee = false;
            character.Equip(character.MeleeWeapon);
            character.isHoldingWeapon = true;
        }

        public override void HandleInput()
        {
            base.HandleInput();
            swingMelee = Input.GetButtonDown("Fire1");
            sheatheMelee = Input.GetKeyDown(KeyCode.V);
            blockMelee = Input.GetKey(KeyCode.F);

            jump = Input.GetButtonDown("Jump");
            crouch = Input.GetButtonDown("Fire3");
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (swingMelee)
            {
                character.Swing();
            }
            else if (blockMelee)
            {
                character.Block();
            }
            else if (!blockMelee)
            {
                character.StopBlock();
            }
            else if (sheatheMelee)
            {
                character.isHoldingWeapon = false;
                character.SheathWeapon();
                stateMachine.ChangeState(character.standing);
            }
            else if (jump)
            {
                stateMachine.ChangeState(character.jumping);
            }
            else if (crouch)
            {
                stateMachine.ChangeState(character.ducking);
            }
        }
    }
}
