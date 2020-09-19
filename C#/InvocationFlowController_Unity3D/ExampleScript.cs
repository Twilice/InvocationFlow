using UnityEngine;
using InvocationFlow;

public class ExampleScript : MonoBehaviour
{
    public void Start()
    {
        // with methods
        this.InvokeWhen(PrintFunc, AboveMagicTreshold);

        // with lambdas
        this.InvokeWhen(() => print("hello"), () => transform.position.y > 1.0f);

        // Other Invokes exist as well
        this.InvokeWhile(AboveMagicTreshold, () => print("We are above the treshold!"));
        this.TimeLerpValueThen(2f, 0f, 100f, (value) => print($"Loading {value}%"), () => print("Loading completed in 2 seconds!"));
        
        // If you need to do an invocation and the gameobject might be destroyed, it can be bound to a hidden Flow Gameobject.
        Flow.InvokeDelayed(5f, () => print("5 seconds passed."));
    }

    private void PrintFunc()
    {
        print("hello2");
    }

    private bool AboveMagicTreshold()
    {
        return transform.position.y > 1.0f;
    }
}
