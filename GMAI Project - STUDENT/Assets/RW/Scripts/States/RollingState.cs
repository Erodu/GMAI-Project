using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class RollingState : State
    {
        private float rollForce = 5f;
        private int rollParam = Animator.StringToHash("Roll");
        public RollingState(Character character, StateMachine stateMachine) : base(character, stateMachine)
        {

        }

        public override void Enter()
        {
            base.Enter();
            character.TriggerAnimation(rollParam);
            character.ApplyImpulse(Vector3.forward * rollForce);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (character.DetectedAnimEnd(rollParam))
            {
                if (character.isHoldingWeapon)
                {
                    stateMachine.ChangeState(character.currentlyMelee);
                }
                else
                {
                    stateMachine.ChangeState(character.standing);
                }
            }
        }
    }
}