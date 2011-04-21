namespace Topshelf.Specs.Internal
{
	using System;
	using Magnum.TestFramework;
	using Topshelf.Internal;

	[Scenario]
	public class FiringThingsOnce_Specs
	{
		int fired = 0;
		Action method;

		[Given]
		public void A_method_to_invoke()
		{
			method = ((Action)(() => fired++)).FireOnceOnly();
		}

		[Then]
		public void The_method_should_only_be_fired_once()
		{
			fired.ShouldBeEqualTo(0);
			method();
			fired.ShouldBeEqualTo(1);
			method();
			fired.ShouldBeEqualTo(1);
		}
	}
}