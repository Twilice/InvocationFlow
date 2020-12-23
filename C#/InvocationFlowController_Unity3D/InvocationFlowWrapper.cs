using System;
using TLM.InvocationFlow;
using TLM.InvocationFlow.Unity3D;
using UnityEngine;

namespace TLM.InvocationFlow.Unity3D
{
    // Use Flow.InvokeX() if you don't want the invocation to be bound to a specific gameobject. If bound to a gameobject, the invocation will not be executed if gameobject is destroyed.
    public static class Flow
    {
        private static InvocationFlowController _staticController = null;
        private static InvocationFlowController StaticController
        {
            get
            {
                if (ReferenceEquals(_staticController, null))
                {
                    _staticController = InvocationFlowController.singleton.GetComponent<InvocationFlowController>();
                }
                return _staticController;
            }
        }
        static Flow()
        {
            InvocationFlowController.Initiate();
        }
        
        public static void InvokeWhen(Action func, Func<bool> condition)
        {
            InvocationFlow<MonoBehaviour>.InvokeWhen(StaticController, func, condition);
        }

        public static void InvokeWhile(Action func, Func<bool> condition)
        {
            InvocationFlow<MonoBehaviour>.InvokeWhile(StaticController, func, condition);
        }

        public static void InvokeWhileThen(Action func, Func<bool> condition, Action onComplete)
        {
            InvocationFlow<MonoBehaviour>.InvokeWhileThen(StaticController, func, condition, onComplete);
        }

        public static void InvokeDelayed(float delayTime, Action func, bool scaledTime = true)
        {
            InvocationFlow<MonoBehaviour>.InvokeDelayed(StaticController, delayTime, func, scaledTime);
        }
        public static void InvokeDelayedFrames(int delayFrames, Action func, bool scaledTime = true)
        {
            InvocationFlow<MonoBehaviour>.InvokeDelayedFrames(StaticController, delayFrames, func, scaledTime);
        }

        // delay 1 frame
        public static void InvokeDelayed(Action func, bool scaledTime = true)
        {
            InvocationFlow<MonoBehaviour>.InvokeDelayedFrames(StaticController, 1, func, scaledTime);
        }

        public static void TimeLerpValue(float lerpTime, float startVal, float endVal, Action<float> func, bool scaledTime = true)
        {
            InvocationFlow<MonoBehaviour>.TimeLerpValue(StaticController, lerpTime, startVal, endVal, func, scaledTime);
        }
        public static void TimeLerpValue<T>(float lerpTime, T startVal, T endVal, Func<T, T, float, T> lerpFunction, Action<T> func, bool scaledTime = true)
        {
            InvocationFlow<MonoBehaviour>.TimeLerpValue<T>(StaticController, lerpTime, startVal, endVal, lerpFunction, func, scaledTime);
        }
        public static void TimeLerpValueThen(float lerpTime, float startVal, float endVal, Action<float> func, Action onComplete, bool scaledTime = true)
        {
            InvocationFlow<MonoBehaviour>.TimeLerpValueThen(StaticController, lerpTime, startVal, endVal, func, onComplete, scaledTime);
        }
        public static void TimeLerpValueThen<T>(float lerpTime, T startVal, T endVal, Func<T, T, float, T> lerpFunction, Action<T> func, Action onComplete, bool scaledTime = true)
        {
            InvocationFlow<MonoBehaviour>.TimeLerpValueThen<T>(StaticController, lerpTime, startVal, endVal, lerpFunction, func, onComplete, scaledTime);
        }
    }

    public static class InvocationFlowWrapper
    {
        static InvocationFlowWrapper()
        {
            InvocationFlowController.Initiate();
        }
        public static void InvokeWhen(this MonoBehaviour script, Action func, Func<bool> condition)
        {
            InvocationFlow<MonoBehaviour>.InvokeWhen(script, func, condition);
        }

        public static void InvokeWhile(this MonoBehaviour script, Action func, Func<bool> condition)
        {
            InvocationFlow<MonoBehaviour>.InvokeWhile(script, func, condition);
        }

        public static void InvokeWhileThen(this MonoBehaviour script, Action func, Func<bool> condition, Action onComplete)
        {
            InvocationFlow<MonoBehaviour>.InvokeWhileThen(script, func, condition, onComplete);
        }

        public static void InvokeDelayed(this MonoBehaviour script, float delayTime, Action func, bool scaledTime = true)
        {
            InvocationFlow<MonoBehaviour>.InvokeDelayed(script, delayTime, func, scaledTime);
        }

        public static void InvokeDelayed(this MonoBehaviour script, int delayFrames, Action func, bool scaledTime = true)
        {
            InvocationFlow<MonoBehaviour>.InvokeDelayedFrames(script, delayFrames, func, scaledTime);
        }

        // delay 1 frame
        public static void InvokeDelayed(this MonoBehaviour script, Action func, bool scaledTime = true)
        {
            InvocationFlow<MonoBehaviour>.InvokeDelayedFrames(script, 1, func, scaledTime);
        }

        public static void TimeLerpValue(this MonoBehaviour script, float lerpTime, float startVal, float endVal, Action<float> func, bool scaledTime = true)
        {
            InvocationFlow<MonoBehaviour>.TimeLerpValue(script, lerpTime, startVal, endVal, func, scaledTime);
        }

        public static void TimeLerpValue(this MonoBehaviour script, float lerpTime, Vector3 startVal, Vector3 endVal, Action<Vector3> func, bool scaledTime = true)
        {
            InvocationFlow<MonoBehaviour>.TimeLerpValue<Vector3>(script, lerpTime, startVal, endVal, Vector3.Lerp, func, scaledTime);
        }

        public static void TimeLerpValue<T>(this MonoBehaviour script, float lerpTime, T startVal, T endVal, Func<T, T, float, T> lerpFunction, Action<T> func, bool scaledTime = true)
        {
            InvocationFlow<MonoBehaviour>.TimeLerpValue<T>(script, lerpTime, startVal, endVal, lerpFunction, func, scaledTime);
        }
        public static void TimeLerpValueThen(this MonoBehaviour script, float lerpTime, float startVal, float endVal, Action<float> func, Action onComplete, bool scaledTime = true)
        {
            InvocationFlow<MonoBehaviour>.TimeLerpValueThen(script, lerpTime, startVal, endVal, func, onComplete, scaledTime);
        }
        public static void TimeLerpValueThen<T>(this MonoBehaviour script, float lerpTime, T startVal, T endVal, Func<T, T, float, T> lerpFunction, Action<T> func, Action onComplete, bool scaledTime = true)
        {
            InvocationFlow<MonoBehaviour>.TimeLerpValueThen<T>(script, lerpTime, startVal, endVal, lerpFunction, func, onComplete, scaledTime);
        }
    }
}
