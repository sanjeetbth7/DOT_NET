
using System;
class Hello
{
	public static void Main()
	{
		System.Console.WriteLine("Hello Sanjeet!");
		char a = (char)Console.Read();
		char b = (char)Console.Read();

	    (a,b) = (b,a);
		Console.WriteLine("After Swapping: ");
		Console.WriteLine("a = " + a);
		Console.WriteLine("b = " + b);
	}
}