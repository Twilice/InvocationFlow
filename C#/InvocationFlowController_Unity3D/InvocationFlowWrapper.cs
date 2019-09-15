using System;
using TLM.OpenSource.InvocationFlow;
using TLM.OpenSource.InvocationFlow.Unity3D;
using UnityEngine;

namespace InvocationFlow
{
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

        public static void InvokeDelayed(this MonoBehaviour script, float delayTime, Action func, bool scaledTime = false)
        {
            InvocationFlow<MonoBehaviour>.InvokeDelayed(script, delayTime, func, scaledTime);
        }

        public static void TimeLerpValue(this MonoBehaviour script, float lerpTime, float startVal, float endVal, Action<float> func, bool scaledTime = false)
        {
            InvocationFlow<MonoBehaviour>.TimeLerpValue(script, lerpTime, startVal, endVal, func, scaledTime);
        }
        public static void TimeLerpValue<T>(this MonoBehaviour script, float lerpTime, T startVal, T endVal, Func<T, T, float, T> lerpFunction, Action<T> func, bool scaledTime = false)
        {
            InvocationFlow<MonoBehaviour>.TimeLerpValue<T>(script, lerpTime, startVal, endVal, lerpFunction, func, scaledTime);
        }
        public static void TimeLerpValueThen(this MonoBehaviour script, float lerpTime, float startVal, float endVal, Action<float> func, Action onComplete, bool scaledTime = false)
        {
            InvocationFlow<MonoBehaviour>.TimeLerpValueThen(script, lerpTime, startVal, endVal, func, onComplete, scaledTime);
        }
        public static void TimeLerpValueThen<T>(this MonoBehaviour script, float lerpTime, T startVal, T endVal, Func<T, T, float, T> lerpFunction, Action<T> func, Action onComplete, bool scaledTime = false)
        {
            InvocationFlow<MonoBehaviour>.TimeLerpValueThen<T>(script, lerpTime, startVal, endVal, lerpFunction, func, onComplete, scaledTime);
        }
    }
}
