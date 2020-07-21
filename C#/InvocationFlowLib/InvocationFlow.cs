using System;
using System.Collections.Generic;
using InvocationHandle = System.Func<bool>;

namespace InvocationFlow
{
    /*
        To get the invocationFlow to work properly the function
            * public static void IterateInvocationHandlers(float deltaTime, float unscaledDeltaTime)
        should be called at every frame and tick update. The invocationFLow is not an event based system, it is more like a superwrapper to write
        chainable invocations while still have easily readable code without tempering to much with performance.

        This could be for you if you are either looking to implement chain of event kind of logic in an easy and performant way or if you have a bad habit of writing huge update loops
        with way to many if cases and need to be able to read and structure your code.
    */
    public static class InvocationFlow<TInvokeTarget> where TInvokeTarget : class
    {
        // this delegate is mainly used for Unity3D logic where a behaviour can be destroyed and become invalid for usage.
        // Unity3D is also the reason we use ReferenceEquals instead of normal nullcheck because they have overloaded the null operator...
        public static Func<TInvokeTarget, bool> IsValid = null;
        // ***** invokes "Header" *****

        public static void InvokeWhen(TInvokeTarget target, Action func, Func<bool> condition)
        {
            AddToInvocationFlowDictionary(target, _invokeWhen(func, condition));
        }

        public static void InvokeWhile(TInvokeTarget target, Action func, Func<bool> condition)
        {
            AddToInvocationFlowDictionary(target, _invokeWhile(func, condition));
        }

        public static void InvokeWhileThen(TInvokeTarget target, Action func, Func<bool> condition, Action onComplete)
        {
            AddToInvocationFlowDictionary(target, _invokeWhileThen(func, condition, onComplete));
        }

        public static void InvokeDelayed(TInvokeTarget target, float delayTime, Action func, bool scaledTime = true)
        {
            if (scaledTime)
                AddToInvocationFlowDictionary(target, _invokeDelayedScaled(delayTime, func));
            else
                AddToInvocationFlowDictionary(target, _invokeDelayedUnscaled(delayTime, func));

        }

        public static void TimeLerpValue(TInvokeTarget target, float lerpTime, float startVal, float endVal, Action<float> func, bool scaledTime = true)
        {
            if (scaledTime)
                AddToInvocationFlowDictionary(target, _timeLerpValueScaled(lerpTime, startVal, endVal, func));
            else
                AddToInvocationFlowDictionary(target, _timeLerpValueUnscaled(lerpTime, startVal, endVal, func));
        }
        public static void TimeLerpValue<T>(TInvokeTarget target, float lerpTime, T startVal, T endVal, Func<T, T, float, T> lerpFunction, Action<T> func, bool scaledTime = true)
        {
            if (scaledTime)
                AddToInvocationFlowDictionary(target, _timeLerpValueScaled(lerpTime, startVal, endVal, lerpFunction, func));
            else
                AddToInvocationFlowDictionary(target, _timeLerpValueUnscaled(lerpTime, startVal, endVal, lerpFunction, func));
        }
        public static void TimeLerpValueThen(TInvokeTarget target, float lerpTime, float startVal, float endVal, Action<float> func, Action onComplete, bool scaledTime = true)
        {
            if (scaledTime)
                AddToInvocationFlowDictionary(target, _timeLerpValueScaled(lerpTime, startVal, endVal, func, onComplete));
            else
                AddToInvocationFlowDictionary(target, _timeLerpValueUnscaled(lerpTime, startVal, endVal, func, onComplete));
        }
        public static void TimeLerpValueThen<T>(TInvokeTarget target, float lerpTime, T startVal, T endVal, Func<T, T, float, T> lerpFunction, Action<T> func, Action onComplete, bool scaledTime = true)
        {
            if (scaledTime)
                AddToInvocationFlowDictionary(target, _timeLerpValueScaled(lerpTime, startVal, endVal, lerpFunction, func, onComplete));
            else
                AddToInvocationFlowDictionary(target, _timeLerpValueUnscaled(lerpTime, startVal, endVal, lerpFunction, func, onComplete));
        }

        // ***** invokes "implementation" *****

        private static InvocationHandle _invokeWhen(Action func, Func<bool> condition)
        {
            return () =>
            {
                if (condition() == false)
                    return false;
                func();
                return true;
            };
        }

        private static InvocationHandle _invokeWhile(Action func, Func<bool> condition)
        {
            return () =>
            {
                if (condition() == true)
                {
                    func();
                    return false;
                }
                return true;
            };
        }
        private static InvocationHandle _invokeWhileThen(Action func, Func<bool> condition, Action onComplete)
        {
            return () =>
            {
                if (condition() == true)
                {
                    func();
                    return false;
                }
                onComplete();
                return true;
            };
        }
        private static InvocationHandle _invokeDelayedUnscaled(float delayTime, Action func)
        {
            float timeElapsed = -_unscaledDeltaTime;
            return () =>
            {
                timeElapsed += _unscaledDeltaTime;
                if (timeElapsed < delayTime)
                {
                    return false;
                }
                func();
                return true;
            };
        }

        private static InvocationHandle _invokeDelayedScaled(float delayTime, Action func)
        {
            float timeElapsed = -_deltaTime;
            return () =>
            {
                timeElapsed += _deltaTime;
                if (timeElapsed < delayTime)
                {
                    return false;
                }
                func();
                return true;
            };
        }
        private static InvocationHandle _timeLerpValueUnscaled(float lerpTime, float startVal, float endVal, Action<float> func, Action onComplete = null)
        {
            float timeElapsed = -_unscaledDeltaTime;
            return () =>
            {
                timeElapsed += _unscaledDeltaTime;
                if (timeElapsed < lerpTime)
                {
                    func(Lerp(startVal, endVal, timeElapsed / lerpTime));
                    return false;
                }
                func(endVal);
                if (ReferenceEquals(onComplete, null) == false)
                    onComplete();
                return true;
            };
        }
        private static InvocationHandle _timeLerpValueScaled(float lerpTime, float startVal, float endVal, Action<float> func, Action onComplete = null)
        {
            float timeElapsed = -_deltaTime;
            return () =>
            {
                timeElapsed += _deltaTime;
                if (timeElapsed < lerpTime)
                {
                    func(Lerp(startVal, endVal, timeElapsed / lerpTime));
                    return false;
                }
                func(endVal);
                if (ReferenceEquals(onComplete, null) == false)
                    onComplete();
                return true;
            };
        }

        private static InvocationHandle _timeLerpValueUnscaled<T>(float lerpTime, T startVal, T endVal, Func<T, T, float, T> lerpFunction, Action<T> func, Action onComplete = null)
        {
            float timeElapsed = -_unscaledDeltaTime;
            return () =>
            {
                timeElapsed += _unscaledDeltaTime;
                if (timeElapsed < lerpTime)
                {
                    func(lerpFunction(startVal, endVal, timeElapsed / lerpTime));
                    return false;
                }
                func(endVal);
                if (ReferenceEquals(onComplete, null) == false)
                    onComplete();
                return true;
            };
        }
        private static InvocationHandle _timeLerpValueScaled<T>(float lerpTime, T startVal, T endVal, Func<T, T, float, T> lerpFunction, Action<T> func, Action onComplete = null)
        {
            float timeElapsed = -_deltaTime;

            return () =>
            {
                timeElapsed += _deltaTime;
                if (timeElapsed < lerpTime)
                {
                    func(lerpFunction(startVal, endVal, timeElapsed / lerpTime));
                    return false;
                }
                func(endVal);
                if (ReferenceEquals(onComplete, null) == false)
                    onComplete();
                return true;
            };
        }

        // ***** base functions ******

        private static Dictionary<TInvokeTarget, List<InvocationHandle>> invocationHandles = new Dictionary<TInvokeTarget, List<InvocationHandle>>();

        private static Dictionary<TInvokeTarget, List<InvocationHandle>> handlesAddedDuringIteration = new Dictionary<TInvokeTarget, List<InvocationHandle>>();
        private static void AddToInvocationFlowDictionary(TInvokeTarget behaviour, InvocationHandle func)
        {
            if (iterating)
            {
                if (handlesAddedDuringIteration.ContainsKey(behaviour))
                {
                    handlesAddedDuringIteration[behaviour].Add(func);
                }
                else
                {
                    handlesAddedDuringIteration.Add(behaviour, new List<InvocationHandle> { func });
                }
                func(); // execute this frame in same "timespace" as invocationFlow "parent" that created this while iterating.
            }
            else if (invocationHandles.ContainsKey(behaviour))
            {
                invocationHandles[behaviour].Add(func);
            }
            else
            {
                invocationHandles.Add(behaviour, new List<InvocationHandle> { func });
            }
        }
        // it is assumed that deltaTimes are the same inbetween ticks, meaning timeElapsed must always start at 0.
        private static float _deltaTime;
        private static float _unscaledDeltaTime;
        private static bool iterating = false;
        public static void IterateInvocationHandlers(float deltaTime, float unscaledDeltaTime)
        {
            _deltaTime = deltaTime;
            _unscaledDeltaTime = unscaledDeltaTime;
            List<TInvokeTarget> keysToRemove = new List<TInvokeTarget>();

            iterating = true;
            var keysCache = invocationHandles.Keys;
            foreach (var key in keysCache)
            {
                // if target is disposed of we don't do the invocation
                if (IsValid(key) == false)
                {
                    keysToRemove.Add(key);
                    continue;
                }
                var invocationFlowsCache = invocationHandles[key];
                for (int i = invocationFlowsCache.Count - 1; 0 <= i; i--)
                {
                    if (invocationFlowsCache[i]() == true) // has InvocationFlow completed it's "flowchart"
                    {
                        invocationFlowsCache.RemoveAt(i);
                    }
                }
                if (invocationFlowsCache.Count == 0)
                {
                    keysToRemove.Add(key);
                }
            }
            foreach (var keyToRemove in keysToRemove)
            {
                invocationHandles.Remove(keyToRemove);
            }

            if(handlesAddedDuringIteration.Count != 0)
            {
                foreach (var keyValuePair in handlesAddedDuringIteration)
                {
                    if (invocationHandles.ContainsKey(keyValuePair.Key))
                    {
                        invocationHandles[keyValuePair.Key].AddRange(keyValuePair.Value);
                    }
                    else
                    {
                        invocationHandles.Add(keyValuePair.Key, keyValuePair.Value);
                    }
                }
                handlesAddedDuringIteration.Clear();
            }
            iterating = false;
        }
        private static float Lerp(float x, float y, float samplePoint)
        {
#if HIGH_PRECISION_LERP
            return (1 - samplePoint) * x + samplePoint * y; // in case you have floating point errors with y-x
#else
            return x + samplePoint * (y - x);
#endif
        }
    }
}
