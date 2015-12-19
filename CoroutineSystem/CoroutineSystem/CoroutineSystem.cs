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
	public delegate void MessageDelegate(Message message);
	public class MessageMapper
	{
		public Dictionary<Type, MessageDelegate> mappedMessages = new Dictionary<Type, MessageDelegate>();
	}
	public class Coroutine
	{
		public Coroutine()
		{
            
		}
		public IEnumerable Function
		{
			get
			{
				return coroutinefunction;
			}
			set
			{
				coroutinefunction = value;
				curEnumeration = value.GetEnumerator();
			}
		}
		public IEnumerator CurEnumeration
		{
			get
			{
				return curEnumeration;
			}
		}
		private IEnumerable coroutinefunction;
		private IEnumerator curEnumeration;
		
		
        public bool shouldTerminate;
        public void Terminate()
        {
            shouldTerminate = true;
        }
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
		public void Send(Message message)
		{
			for (int i = 0; i < coroutines.Count; i++)
			{
				Coroutine coroutine = coroutines.ElementAt(i);
				if (coroutine.mapper.mappedMessages.ContainsKey(message.GetType())) //if this coroutine contains the message
				{
					for (int j = 0; j < i; j++)//pop all child coroutines off the stack
					{
						coroutines.Pop();
					}
					MessageDelegate del;
					coroutine.mapper.mappedMessages.TryGetValue(message.GetType(), out del);
					del(message); //call function attached to mapped message
                    break;
				}
			}
		}
		IEnumerable Update()
		{
			Redo:
			shouldSwitch = false;			
			if (coroutines.Count == 0) //early out
				yield break;
			
			while (coroutines.Peek().CurEnumeration.MoveNext()) //move to next yield
			{
				var value = coroutines.Peek().CurEnumeration.Current; //accept any return value, this is because we don't want a InvalidCastException
				Type type = value.GetType();
				if (type == typeof(Coroutine)) //if type is a subclass of coroutine, add it to the stack and set switch flag
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
				goto Redo;
			}
		}
		public void Tick()
		{
			foreach(var value in Update())
			{
				//Console.WriteLine(value);
			}
		}

	}

}