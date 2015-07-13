using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Coroutines;


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
	class TestFunc : Coroutine
	{
		int int1;
		string string1;
		void Stop()
		{
			Console.Write("MessageReceived\n"); 
		}

		public TestFunc(int argInt, string argString) 
		{
			int1 = argInt;
			string1 = argString;
			mapper.mappedMessages.Add(typeof(StopMessage), new MessageDelegate(Stop)); //link Stop function to the StopMessage, these could be any type or name
			attributes |= Attributes.Looping; //Make this coroutine loop when it ends
		}
		override public IEnumerable function()
		{
			//returning strings and integers do nothing, they just serve demonstration purposes.
			yield return 1;
			yield return int1;
			yield return new TestFunc2(); //Start running TestFunc2 from this point
			yield return string1; 
			yield return 15;
		}
	}

	class TestFunc2 : Coroutine
	{
		public TestFunc2()
		{
			attributes |= Attributes.Looping;
		}
		override public IEnumerable function()
		{
			int fib1 = TestMath.Fibonacci(14);
			int fib2 = TestMath.Fibonacci(13);
			yield return TestMath.Fibonacci(12);
			yield return fib2;
			yield return fib1;
			yield return "Hello from testFunction2";
		}
	}
	class Program
	{
		static void Main(string[] args)
		{
			CoroutineSystem system = new CoroutineSystem(); //create the coroutine system
			Coroutine test = new TestFunc(65535, "Hi from Main through TestFunc"); //create initial coroutine, I recommend setting the first coroutine to loop
			system.AddCoroutine(test); //add the coroutine to the system as entry point
			int c = 0;

			while (true)
			{
				c++;
				system.Update();
				if (c % 10 == 0) //send stop message every 10 updates
					system.Send(typeof(StopMessage));
			}
		}
	}
}