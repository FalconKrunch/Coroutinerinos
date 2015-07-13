//The MIT License (MIT)

//Copyright (c) 2015 Joppe van der Meij

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Coroutines
{
	
	public class Message
	{

	}
	public delegate void MessageDelegate();
	public class MessageMapper
	{
		public Dictionary<Type, MessageDelegate> mappedMessages = new Dictionary<Type, MessageDelegate>();
	}
	public class Coroutine
	{
		[Flags]
		public enum Attributes
		{
			Looping = 1,
		}
		public Coroutine()
		{
			curEnumeration = function().GetEnumerator();
		}
		public virtual IEnumerable function() { return "Empty routine"; }
		public IEnumerator curEnumeration;

		public MessageMapper mapper = new MessageMapper();
		~Coroutine()
		{

		}
	}

	class CoroutineSystem
	{
		public Stack<Coroutine> coroutines = new Stack<Coroutine>();
		private bool shouldSwitch = false;
		public void AddCoroutine(Coroutine coroutine)
		{
			coroutines.Push(coroutine);
		}
		public void Send(Type message)
		{
			for (int i = 0; i < coroutines.Count; i++)
			{
				Coroutine coroutine = coroutines.ElementAt(i);
				if (coroutine.mapper.mappedMessages.ContainsKey(message)) //if this coroutine contains the message
				{
					for (int j = 0; j < i; j++)//pop all child coroutines off the stack
					{
						coroutines.Pop();
					}
					MessageDelegate del;
					coroutine.mapper.mappedMessages.TryGetValue(message, out del);
					del(); //call function attached to mapped message
				}
			}
		}
		IEnumerable Update()
		{
			shouldSwitch = false;
			if (coroutines.Count == 0) //early out
				yield break;
			
			while (coroutines.Peek().curEnumeration.MoveNext()) //move to next yield
			{
				var value = coroutines.Peek().curEnumeration.Current; //accept any return value, this is because we don't want a InvalidCastException
				Type type = value.GetType();
				if (type.IsSubclassOf(typeof(Coroutine))) //if type is a subclass of coroutine, add it to the stack and set switch flag
				{
					coroutines.Push((Coroutine)value);
					shouldSwitch = true;
					Console.WriteLine("Switching to " + value);
					break;
				}
				else //print any uncaught values
				{
					Console.WriteLine("Uncaught type " + value.GetType().ToString() + ": " + value);
				}
				yield break;
			}
			if (!shouldSwitch)
			{
				coroutines.Pop();
			}
		}
		public void Tick()
		{
			foreach(var value in Update())
			{
				Console.WriteLine(value);
			}
		}

	}

}