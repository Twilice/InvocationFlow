﻿using System;


namespace TLM.InvocationFlow.Example
{
    /* 
    The InvocationFlowController needs to be implemented by the user,
     on a simpler scale it should only call IterateInvocationHandlers every wanted tick/frame
     but specializations might be needed depending on program/engine structure.
     */
	 

    public static class BehaviourExtensions
    {
        public static void InvokeWhen(this Behaviour behaviour, Action func, Func<bool> condition)
        {
            InvocationFlow<Behaviour>.InvokeWhen(behaviour, func, condition);
        }

        public static void TimeLerpValue(this Behaviour behaviour, float lerpTime, float startVal, float endVal, Action<float> func)
        {
            InvocationFlow<Behaviour>.TimeLerpValue(behaviour, lerpTime, startVal, endVal, func);
        }
    }
    public class InvocationFlowController
    {
		public static float deltaTime { get; set; } = 0.1f;
		public static float unscaledDeltaTime { get; set; } = 0.1f;
	
        static InvocationFlowController()
        {
            InvocationFlow<Behaviour>.IsValid = (Behaviour x) => x.Valid;
			InvocationFlow<Behaviour>.LinkDeltaTimeProperties(typeof(InvocationFlowController).GetProperty(nameof(deltaTime)), typeof(InvocationFlowController).GetProperty(nameof(unscaledDeltaTime)));
        }
		
        public void Tick()
        {
            InvocationFlow<Behaviour>.IterateInvocationHandlers();
        }
    }
}