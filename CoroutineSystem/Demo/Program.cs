using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Coroutines;
using System.Threading;


namespace Program
{
	class StopMessage : Message
	{

	}
	class TestMath
	{
		public static int Fibonacci(int n)
		{
			int a = 0;
			int b = 1;
			// In N steps compute Fibonacci sequence iteratively.
			for (int i = 0; i < n; i++)
			{
				int temp = a;
				a = b;
				b = temp + b;
			}
			return a;
		}
	}
	//class TestFunc : Coroutine
	//{
	//	int int1;
	//	string string1;
	//	void Stop(Message stopMessage)
	//	{
	//		Console.Write("MessageReceived\n"); 
	//	}

	//	public TestFunc(int argInt, string argString) 
	//	{
	//		int1 = argInt;
	//		string1 = argString;
	//		mapper.mappedMessages.Add(typeof(StopMessage), new MessageDelegate(Stop)); //link Stop function to the StopMessage, these could be any type or name
	//	}
	//	override public IEnumerable function()
	//	{
	//		while (true)
	//		{
	//			//returning strings and integers do nothing, they just serve demonstration purposes.
	//			yield return 1;
	//			yield return int1;
	//			yield return new TestFunc2(); //Start running TestFunc2 from this point
	//			yield return string1;
	//			yield return 15;
	//		}
	//	}
	//}

	//class TestFunc2 : Coroutine
	//{
	//	public TestFunc2()
	//	{
			
	//	}
	//	override public IEnumerable function()
	//	{
	//		while (true)
	//		{
	//			int fib1 = TestMath.Fibonacci(14);
	//			int fib2 = TestMath.Fibonacci(13);
	//			yield return TestMath.Fibonacci(12);
	//			yield return fib2;
	//			yield return fib1;
	//			yield return "Hello from testFunction2";
	//		}
	//	}
	//}
    
	class Program
	{
        
		static void Main(string[] args)
		{
			
			//Coroutine test = new TestFunc(65535, "Hi from Main through TestFunc"); //create initial coroutine, I recommend setting the first coroutine to loop
			//system.AddCoroutine(test); //add the coroutine to the system as entry point
			
            //StopMessage message = new StopMessage();

           
			Main main = new Main();
			main.Init();
			while (true)
			{
				main.Update();
			}
		}
	}
    class Main
    {
		IEnumerable Bye()
		{
			Console.Write("Current Iteration: " + c + "\n");
			yield return "ByeBye!\n";
		}
		IEnumerable Hello()
		{
			while (true)
			{
				Console.Write("Hello\nWe are currently at iteration: " + c + "\n");
				yield return new Coroutine { Function = Bye() };
			}
		}
		CoroutineSystem system;
		int c;
        public void Init()
		{
			c = 0;
			system = new CoroutineSystem(); //create the coroutine system
			Coroutine routine = new Coroutine { Function = Hello() };
			system.AddCoroutine(routine);
		}
        public void Update()
        {
			//Thread.Sleep(1);
			StopMessage message = new StopMessage();
			c++;
			system.Tick();
			if (c % 10 == 0) //send stop message every 10 updates
				system.Send(message);
		}
    }
}