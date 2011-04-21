namespace Topshelf.Internal
{
	using System;


	static class FunEx
	{
		/// <summary>
		/// Wraps an action being fired in a mutually-exclusive lock (Monitor)
		/// and makes sure that it's only ever invoked once by keeping an 'has invoked'
		/// boolean.
		/// </summary>
		/// <param name="a"></param>
		/// <returns>A method which can be called multiple times but only actually executes
		/// the action parameter once.</returns>
		public static Action FireOnceOnly(this Action a)
		{
			bool fired = false;
			object locker = new object();
			return () =>
				{
					if (fired)
						return;

					lock (locker)
					{
						if (fired)
							return;

						a();

						fired = true;
					}
				};
		}
	}
}