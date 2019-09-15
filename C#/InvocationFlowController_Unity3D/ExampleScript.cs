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
