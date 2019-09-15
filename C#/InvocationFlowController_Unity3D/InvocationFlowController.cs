using UnityEditor;
using UnityEngine;

namespace TLM.OpenSource.InvocationFlow.Unity3D
{
    /// <summary>
    /// Will iterate all the delegates used in CustomFunctionInvokes
    /// </summary>
    public class InvocationFlowController : MonoBehaviour
    {
        private static GameObject singleton = null;

        private void Awake()
        {
            if (singleton != gameObject && ReferenceEquals(singleton, null) == false)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            int highestExecutionOrder = 0;
            MonoScript controllerScript = MonoScript.FromMonoBehaviour(this);
            MonoScript[] allScripts = MonoImporter.GetAllRuntimeMonoScripts();
            for (int i = 0; i < allScripts.Length; i++)
            {
                if (allScripts[i] != controllerScript)
                {
                    int scriptExecutionOrder = MonoImporter.GetExecutionOrder(allScripts[i]);
                    if (scriptExecutionOrder > highestExecutionOrder)
                    {
                        highestExecutionOrder = scriptExecutionOrder;
                    }
                }
            }
            MonoImporter.SetExecutionOrder(controllerScript, highestExecutionOrder + 100);
        }
        public static void Initiate()
        {
            InvocationFlow<MonoBehaviour>.IsValid = (MonoBehaviour script) => script == null;
            singleton = new GameObject("InvocationFlowController", typeof(InvocationFlowController));
        }

        // Execution order is expected to be after update but before coroutines as long as execution order is not changed.
        void Update()
        {
            InvocationFlow<MonoBehaviour>.IterateInvocationHandlers(Time.deltaTime, Time.unscaledDeltaTime);
        }
    }
}
