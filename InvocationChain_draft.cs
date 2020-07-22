var invokeChain = this.Delay(3).When(cond1).Invoke(action1).Delay(5).Loop(5).Invoke(action2).When(cond2).Delay(1).EndLoop().Invoke(action3) // this."stuff" = autostart
and/or
var invokeChain = new InvokeChain(gameObject); // new InvokeChain = manual, will not autostart

Loop() => inf
Loop(5) => 5 times
Loop(cond) => loop until cond

EndLoop() => exit point of loop 
	- repeats at start of loop, unless Loop has state "exiting"
EndLoop(cond) => force break if condition true

Break(cond) => force break if condition true - will skip chains until EndLoop

invokeChain.Start(); // start at current location (0 default)
invokeChain.Pause(); // stop execution at curren location and resume same place when starting.
invokeChain.Stop(); // stop execution and set current location to 0
InvokeChain."stuff" Invoke/Loop/When etc. // will ADD that state at the end
invokeChain.Abort(); // Will stop execution and reset to 0
invokeChain.Reset(); // Will reset to 0 and start execution
invokeChain.Break(); // will jump to statement after EndLoop();
invokeChain.Skip(); // skip next state, if next state is loop or endLoop, will do a break.
invokeChain.Pop(); // will REMOVE the state at the end.
*** more advanced, do last. Even if it means we need to remake lots of code. ***
invokeChain.RemoveAt(i); // will REMOVE state at index, if loop or endloop it will remove that whole segment
invokeChain.InsertAt(i)."stuff"; // will ADD states followed by "stuff" at index
invokeChain.JumpTo(i); // Will jump to state at index
invokeChain.Concat(invokeChain); // will copy states in invokeChain and add them to the end
invokeChain.Loop(i/cond, invokeChain); // will loop with concated versioninvokeChain and insert endloop * ok or bad syntax?

an invokeChain can be repeated, and will set itself to stop() and index to 0 when executionchain is finished

include explicit lerp? - probably!
wanted overloads: 
Lerp(float duration, Func<float alpha> func, Func<> onComplete);
Lerp(float duration, T from, T to, (* Func<float alpha, T val> lerpFunc), Func<T val> func); * lerpFunc for non-common dataTypes, int/float/double can have native default overloads - Vectors can have extensions (e.g. UnityEngine.Vector3)
Lerp(float duration, T from, T to, Func<float alpha, float timedAlpha> timingFunc,Func<T val> func);
Lerp(float duration, Func<float alpha, float timedAlpha> timingFunc, Func<float t> func);

*include onComplete and generic LerpFunc

include explicit animations? - skip for now.

*****
experience from standard InvocationFlow.
Want some kind of repeatable statements:

 "YieldInvokeWhileThen" -> if invoked multiple times, it will still continue in same command unless completelly finished.
 E.G. instead of to prevent multiple Invokes
	if (dashes == maxDashes)
	{
		this.InvokeWhileThen(() => dashCooldown -= Time.deltaTime, () => 0 < dashCooldown, () => dashes = maxDashes);
	}
 Can do only	
	this.YieldInvokeWhileThen(() => dashCooldown -= Time.deltaTime, () => 0 < dashCooldown, () => dashes = maxDashes);
	
Maybe this can be innate to Chain and be a setting? Yield is bad name? Can do normal yield logic, but normal use case is probably a "start" like chain that won't do anything if it is started again.


"TickingInvoke" -> 
This.InvokeWhenStopTick(FunctionToBeCalledWhenThisWasNotTicked);

"ConditionalInvoke" -> basically just spinning Ifs
instead of "bloating" update:
   public void Update()
    {        
        if(isFiring && cooldown <= 0)
        {
			Fire();
		}
	}
	
Can do:
var FireWhenPossible = this.ConditinalInvoke(cooldown <= 0, Fire);
...
isFiring = true;
FireWhenPossible.Start();
...
isFiring = false;
FireWhenPossible.Stop();

"InvokeDuration / InvokeFor" -> invert of InvokeWhile, Invoke for X seconds.
******	
	

example of simplified code

with chain:
var invokeChain = this.Delay(3).When(cond1).Invoke(action1).Delay(5).Loop(5).Invoke(action2).When(cond2).Delay(1).EndLoop().Invoke(action3)

without any code design (need example of better "standard" implementation?):
var logicComplete = false;
var timeDelay1 = 0;
var timeDelay2 = 0;
var timeDelay3 = 0;
var cond1Fullfilled = false;
var cond2Fullfilled = false;
var loops = 0;
var doLoopAction = true;

update()
{
	if(logicComplete == false)
	{		
		time1 += Time.deltaTime;
		if(3 <= time1)
		{
			if(cond1() || cond1Fullfilled)
			{		
				cond1Fullfilled = true;
				action1();
				time2 += Time.deltaTime;
				if(5 <= time2)
				{
					if(loops < 5)
					{
						if(doLoopAction)
						{
							action2();
							doLoopAction = false;
						}
						if(cond2() || cond2Fullfilled)
						{
							cond2Fullfilled = true;
							time3 += Time.deltaTime;
							if(1 <= time3)
							{
								cond2Fullfilled = false;
								doLoopAction = true;
								loops++;
							}
						}						
					}
					else if(loops == 5)
					{
						action3();
						logicComplete = true;
					}			
				}
			}	
		}
	}
}