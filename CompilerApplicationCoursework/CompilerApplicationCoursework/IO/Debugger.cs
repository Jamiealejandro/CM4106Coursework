using System;
namespace Compiler.IO

public static class Debugger
{
	private const bool DEBUG = false;

	public static void Write(string message)
	{
		if (DEBUG)
			System.Console.WriteLine($"DEBUGGING INFO: {message}");
	}
}
