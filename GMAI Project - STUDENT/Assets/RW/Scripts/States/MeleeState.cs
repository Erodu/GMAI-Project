using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class MeleeState : GroundedState
    {
        private bool swingMelee;
        private bool sheatheMelee;

        private bool jump;
        private bool crouch;

        public MeleeState(Character character, StateMachine stateMachine) : base(character, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            character.SetAnimationBool(character.isMelee, true);
            speed = character.MovementSpeed;
            rotationSpeed = character.RotationSpeed;
            swingMelee = false;
            sheatheMelee = false;
            //if (!character.alreadyEquipped)
            //{
            //    character.Equip(character.MeleeWeapon);
            //    character.alreadyEquipped = true;
            //}
            character.Equip(character.MeleeWeapon);
            character.isHoldingWeapon = true;
            //character.TriggerDrawParam();
        }

        public override void HandleInput()
        {
            base.HandleInput();
            swingMelee = Input.GetButtonDown("Fire1");
            sheatheMelee = Input.GetKeyDown(KeyCode.V);

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
            else if (sheatheMelee)
            {
                character.isHoldingWeapon = false;
                character.SheathWeapon();
                //character.SetAnimationBool(character.isMelee, false);
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
