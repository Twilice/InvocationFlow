// todo ::
/* 
    reduce garbage created upon start even more - performance per tick is very nice now, much faster than normal Unity update.

ideas:
    - create pooling of the "InvokeWhenFlows" to reuse and save more garbage?
    we don't want to sacrifice to much performance to get close to 0 garbage

    - try IEnumerator, is it stackallocating?

    - try seperate lists for each flow, but instead using structs

    - mix pools with struct and have a pool limit with array of structs, if you go over the pool then it uses an extra list? (e.g. static size object pool)
*/
using System;
using System.Collections.Generic;

namespace TLM.InvocationFlow
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
        public static Func<TInvokeTarget, bool> IsValid = (t) => true;
        public static Func<TInvokeTarget, bool> IsEnabled = (t) => true;
        public static TInvokeTarget _specialTargetIgnoreValidation = null;
        // note : originally input deltaTime when iterating. Faster, but issues arose because deltaTime might differ when -creating -the invocations and when executing them. Now fetches the current deltaTime when creating new flows, then use a cached deltaTime when iterating.
        public static void LinkDeltaTimeProperties(System.Reflection.PropertyInfo deltaTimeProperty, System.Reflection.PropertyInfo unscaledDeltaTimeProperty)
        {
            var del = deltaTimeProperty.GetGetMethod().CreateDelegate(typeof(Func<float>));
            _getDeltaTime = (Func<float>)del;

            del = deltaTimeProperty.GetGetMethod().CreateDelegate(typeof(Func<float>));
            _getUnscaledDeltaTime = (Func<float>)del;
        }
        private static Func<float> _getDeltaTime;
        private static Func<float> _getUnscaledDeltaTime;
        private static float _deltaTime;
        private static float _unscaledDeltaTime;


        // ***** Invoke Flows *****

        private abstract class InvokeFlow
        {
            public TInvokeTarget target;
            public abstract bool StepInvoke();
            public InvokeFlow(TInvokeTarget target)
            {
                this.target = target;
            }
        }


        private class InvokeWhenFlow : InvokeFlow
        {
            public Func<bool> condition;
            public Action func;
            public InvokeWhenFlow(TInvokeTarget target, Action func, Func<bool> condition) : base(target)
            {
                this.func = func;
                this.condition = condition;
            }

            public override bool StepInvoke()
            {
                if (condition() == false)
                    return false;
                func();
                return true;
            }
        }

        private class InvokeWhileFlow : InvokeFlow
        {
            public Func<bool> condition;
            public Action func;
            public Action onComplete;
            public InvokeWhileFlow(TInvokeTarget target, Action func, Func<bool> condition, Action onComplete = null) : base(target)
            {
                this.func = func;
                this.condition = condition;
                this.onComplete = onComplete;
            }

            public override bool StepInvoke()
            {
                if (condition() == true)
                {
                    func();
                    return false;
                }
                onComplete?.Invoke();
                return true;
            }
        }

        private class InvokeDelayedFlow : InvokeFlow
        {
            private float delayTime;
            private float timeElapsed;
            public Action func;
            public InvokeDelayedFlow(TInvokeTarget target, float delayTime, Action func) : base(target)
            {
                timeElapsed = -_getDeltaTime();
                this.func = func;
                this.delayTime = delayTime;
            }

            public override bool StepInvoke()
            {
                timeElapsed += _deltaTime;
                if (timeElapsed < delayTime)
                {
                    return false;
                }
                func();
                return true;
            }
        }

        private class InvokeDelayedUnscaledFlow : InvokeFlow
        {
            private float delayTime;
            private float timeElapsed;
            public Action func;
            public InvokeDelayedUnscaledFlow(TInvokeTarget target, float delayTime, Action func) : base(target)
            {
                timeElapsed = -_getUnscaledDeltaTime();
                this.func = func;
                this.delayTime = delayTime;
            }

            public override bool StepInvoke()
            {
                timeElapsed += _unscaledDeltaTime;
                if (timeElapsed < delayTime)
                {
                    return false;
                }
                func();
                return true;
            }
        }
        private class InvokeDelayedFramesFlow : InvokeFlow
        {
            private float delayFrames;
            private float framesElapsed;
            public Action func;
            public InvokeDelayedFramesFlow(TInvokeTarget target, int delayFrames, Action func) : base(target)
            {
                framesElapsed = -1;
                this.func = func;
                this.delayFrames = delayFrames;
            }

            public override bool StepInvoke()
            {
                framesElapsed++;
                if (framesElapsed < delayFrames)
                {
                    return false;
                }
                func();
                return true;
            }
        }

        private static float Lerp(float x, float y, float samplePoint)
        {
#if HIGH_PRECISION_LERP
            return (1 - samplePoint) * x + samplePoint * y; // in case you have floating point errors with y-x
#else
            return x + samplePoint * (y - x);
#endif
        }

        private class TimeLerpValueFlow : InvokeFlow
        {
            private float timeElapsed;
            public float lerpTime;
            public float startVal;
            public float endVal;
            public Action<float> func;
            public Action onComplete;
            public TimeLerpValueFlow(TInvokeTarget target, float lerpTime, float startVal, float endVal, Action<float> func, Action onComplete = null) : base(target)
            {
                this.timeElapsed = -_getDeltaTime();
                this.lerpTime = lerpTime;
                this.startVal = startVal;
                this.endVal = endVal;
                this.func = func;
                this.onComplete = onComplete;
            }

            public override bool StepInvoke()
            {
                timeElapsed += _deltaTime;
                if (timeElapsed < lerpTime)
                {
                    func(Lerp(startVal, endVal, timeElapsed / lerpTime));
                    return false;
                }
                func(endVal);
                onComplete?.Invoke();
                return true;
            }
        }

        private class TimeLerpValueUnscaledFlow : InvokeFlow
        {
            private float timeElapsed;
            public float lerpTime;
            public float startVal;
            public float endVal;
            public Action<float> func;
            public Action onComplete;
            public TimeLerpValueUnscaledFlow(TInvokeTarget target, float lerpTime, float startVal, float endVal, Action<float> func, Action onComplete = null) : base(target)
            {
                this.timeElapsed = -_getUnscaledDeltaTime();
                this.lerpTime = lerpTime;
                this.startVal = startVal;
                this.endVal = endVal;
                this.func = func;
                this.onComplete = onComplete;
            }

            public override bool StepInvoke()
            {
                timeElapsed += _unscaledDeltaTime;
                if (timeElapsed < lerpTime)
                {
                    func(Lerp(startVal, endVal, timeElapsed / lerpTime));
                    return false;
                }
                func(endVal);
                onComplete?.Invoke();
                return true;
            }
        }

        private class TimeLerpValueFlow<T> : InvokeFlow
        {
            private float timeElapsed;
            public float lerpTime;
            public T startVal;
            public T endVal;
            public Action<T> func;
            public Action onComplete;
            Func<T, T, float, T> lerpFunction;
            public TimeLerpValueFlow(TInvokeTarget target, float lerpTime, T startVal, T endVal, Func<T, T, float, T> lerpFunction, Action<T> func, Action onComplete = null) : base(target)
            {
                this.timeElapsed = -_getDeltaTime();
                this.lerpTime = lerpTime;
                this.startVal = startVal;
                this.endVal = endVal;
                this.func = func;
                this.onComplete = onComplete;
                this.lerpFunction = lerpFunction;
            }

            public override bool StepInvoke()
            {
                timeElapsed += _deltaTime;
                if (timeElapsed < lerpTime)
                {
                    func(lerpFunction(startVal, endVal, timeElapsed / lerpTime));
                    return false;
                }
                func(endVal);
                onComplete?.Invoke();
                return true;
            }
        }

        private class TimeLerpValueUnscaledFlow<T> : InvokeFlow
        {
            private float timeElapsed;
            public float lerpTime;
            public T startVal;
            public T endVal;
            public Action<T> func;
            public Action onComplete;
            Func<T, T, float, T> lerpFunction;
            public TimeLerpValueUnscaledFlow(TInvokeTarget target, float lerpTime, T startVal, T endVal, Func<T, T, float, T> lerpFunction, Action<T> func, Action onComplete = null) : base(target)
            {
                this.timeElapsed = -_getUnscaledDeltaTime();
                this.lerpTime = lerpTime;
                this.startVal = startVal;
                this.endVal = endVal;
                this.func = func;
                this.onComplete = onComplete;
                this.lerpFunction = lerpFunction;
            }

            public override bool StepInvoke()
            {
                timeElapsed += _unscaledDeltaTime;
                if (timeElapsed < lerpTime)
                {
                    func(lerpFunction(startVal, endVal, timeElapsed / lerpTime));
                    return false;
                }
                func(endVal);
                onComplete?.Invoke();
                return true;
            }
        }

        public static void InvokeWhen(TInvokeTarget target, Action func, Func<bool> condition)
        {
            AddToInvocationFlows(new InvokeWhenFlow(target, func, condition));
        }

        public static void InvokeWhile(TInvokeTarget target, Action func, Func<bool> condition)
        {
            AddToInvocationFlows(new InvokeWhileFlow(target, func, condition));
        }

        public static void InvokeWhileThen(TInvokeTarget target, Action func, Func<bool> condition, Action onComplete)
        {
            AddToInvocationFlows(new InvokeWhileFlow(target, func, condition, onComplete));
        }

        public static void InvokeDelayed(TInvokeTarget target, float delayTime, Action func, bool scaledTime = true)
        {
            if (scaledTime)
                AddToInvocationFlows(new InvokeDelayedFlow(target, delayTime, func));
            else
                AddToInvocationFlows(new InvokeDelayedUnscaledFlow(target, delayTime, func));
        }

        public static void InvokeDelayedFrames(TInvokeTarget target, int delayFrames, Action func)
        {
            AddToInvocationFlows(new InvokeDelayedFramesFlow(target, delayFrames, func));
        }


        public static void TimeLerpValue(TInvokeTarget target, float lerpTime, float startVal, float endVal, Action<float> func, bool scaledTime = true)
        {
            if (scaledTime)
                AddToInvocationFlows(new TimeLerpValueFlow(target, lerpTime, startVal, endVal, func));
            else
                AddToInvocationFlows(new TimeLerpValueUnscaledFlow(target, lerpTime, startVal, endVal, func));
        }
        public static void TimeLerpValue<T>(TInvokeTarget target, float lerpTime, T startVal, T endVal, Func<T, T, float, T> lerpFunction, Action<T> func, bool scaledTime = true)
        {
            if (scaledTime)
                AddToInvocationFlows(new TimeLerpValueFlow<T>(target, lerpTime, startVal, endVal, lerpFunction, func));
            else
                AddToInvocationFlows(new TimeLerpValueUnscaledFlow<T>(target, lerpTime, startVal, endVal, lerpFunction, func));
        }
        public static void TimeLerpValueThen(TInvokeTarget target, float lerpTime, float startVal, float endVal, Action<float> func, Action onComplete, bool scaledTime = true)
        {
            if (scaledTime)
                AddToInvocationFlows(new TimeLerpValueFlow(target, lerpTime, startVal, endVal, func, onComplete));
            else
                AddToInvocationFlows(new TimeLerpValueUnscaledFlow(target, lerpTime, startVal, endVal, func, onComplete));
        }
        public static void TimeLerpValueThen<T>(TInvokeTarget target, float lerpTime, T startVal, T endVal, Func<T, T, float, T> lerpFunction, Action<T> func, Action onComplete, bool scaledTime = true)
        {
            if (scaledTime)
                AddToInvocationFlows(new TimeLerpValueFlow<T>(target, lerpTime, startVal, endVal, lerpFunction, func, onComplete));
            else
                AddToInvocationFlows(new TimeLerpValueUnscaledFlow<T>(target, lerpTime, startVal, endVal, lerpFunction, func, onComplete));
        }


        // ***** base functions ******

        private static List<InvokeFlow> invokeFlows = new List<InvokeFlow>();

        private static void AddToInvocationFlows(InvokeFlow flow)
        {
            if (iterating)
            {
                bool complete = flow.StepInvoke(); // execute this frame in same frame as invocationFlow "parent" that created this while iterating.
                if (complete)
                    return;
            }
            invokeFlows.Add(flow);
        }

        // it is assumed that deltaTimes are the same inbetween ticks, meaning timeElapsed must always start at 0.
        private static bool iterating = false;
        public static void IterateInvocationHandlers()
        {
            // cache deltaTime
            _deltaTime = _getDeltaTime();
            _unscaledDeltaTime = _getUnscaledDeltaTime();


            iterating = true;
            for (int i = invokeFlows.Count - 1; 0 <= i; i--)
            {
                var invokeFlow = invokeFlows[i];

                // special case when target is "global" we don't validate check - should fix to not have reference to Unity3D to keep library "generic"
                if (ReferenceEquals(invokeFlow.target, _specialTargetIgnoreValidation) == false)
                {
                    //if target is disposed of we don't do the invocation
                    if (IsValid(invokeFlows[i].target) == false)
                    {
                        invokeFlows.RemoveAt(i);
                        continue;
                    }
                    // if target is paused, skip
                    if (IsEnabled(invokeFlows[i].target) == false)
                        continue;
                }

                if (invokeFlows[i].StepInvoke() == true) // has InvocationFlow completed it's "flowchart"
                {
                    invokeFlows.RemoveAt(i);
                }
            }
            iterating = false;
        }
    }
}
