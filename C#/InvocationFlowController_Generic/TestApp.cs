namespace TLM.InvocationFlow.Example
{
    class TestApp
    {
        static void Main()
        {
            var controller = new InvocationFlowController();

            Behaviour b1 = new Behaviour();
            Behaviour b2 = new Behaviour();

            int testInt = 0;

            b1.InvokeWhen(() => System.Console.WriteLine("invoked b1"), () => testInt == 1);
            b1.TimeLerpValue(0.2f, 0, 1, (x) =>
            {
                b1.position.x = x;
                System.Console.WriteLine($"b1.position.x == {b1.position.x}");
            });
            b2.InvokeWhen(() => System.Console.WriteLine("invoked b2"), () => testInt == 1);
            
            System.Console.WriteLine("Frame 1");
            controller.Tick();
            System.Console.WriteLine("TestInt = 1");
            testInt = 1;
            System.Console.WriteLine("Destroy b2");
            b2.Destroy();
            System.Console.WriteLine("Frame 2");
            controller.Tick();
            System.Console.WriteLine("Frame 3");
            controller.Tick();
        }
    }
}
