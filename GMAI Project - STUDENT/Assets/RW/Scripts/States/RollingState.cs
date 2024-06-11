using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class RollingState : State
    {
        private bool rolling;
        private float rollForce = 5f;
        private int rollParam = Animator.StringToHash("Roll");
        public RollingState(Character character, StateMachine stateMachine) : base(character, stateMachine)
        {

        }

        public override void Enter()
        {
            base.Enter();
            rolling = true;
            character.TriggerAnimation(rollParam);
            character.ApplyImpulse(Vector3.forward * rollForce);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

        }
    }
}