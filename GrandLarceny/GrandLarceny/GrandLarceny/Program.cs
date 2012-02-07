using System;

namespace GrandLarceny
{
#if WINDOWS || XBOX
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			using (Game game = Game.getInstance())
			{
				game.Run();
			}
		}
	}
#endif
}

