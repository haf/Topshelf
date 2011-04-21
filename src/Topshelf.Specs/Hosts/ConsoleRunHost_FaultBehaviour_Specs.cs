namespace Topshelf.Specs.Hosts
{
	using System;
	using System.Threading;
	using Exceptions;
	using HostConfigurators;
	using Magnum.TestFramework;
	using Model;
	using Rhino.Mocks;
	using Stact;
	using Topshelf.Hosts;


	[Scenario(Description = "Sometimes the service fails on start, and the console host needs to handle failures correctly.",
		TypeArgs = new[]{typeof(ConsoleRunHost)})]
	public class ConsoleRunHost_FaultBehaviour_Specs
	{
		class CoordinatorStub : IServiceCoordinator
		{
			readonly ManualResetEvent _waitFor;
			readonly ManualResetEvent _startedWaiting;

			public CoordinatorStub(ManualResetEvent waitFor, ManualResetEvent startedWaiting)
			{
				_waitFor = waitFor;
				_startedWaiting = startedWaiting;
			}

			public void Send<T>(T message)
			{
			}

			public void Dispose()
			{
			}

			public void Start()
			{
				_startedWaiting.Set();
				_waitFor.WaitOne();
				throw new TopshelfException("services failed to start");
			}

			public void Stop()
			{
			}

			public void CreateService(string serviceName, Func<Inbox, IServiceChannel, IServiceController> serviceFactory)
			{
			}
		}

		ConsoleRunHost _consoleHost;
		ManualResetEvent _beforeException;
		ManualResetEvent _coordinatorStarted;
		MockRepository repo = new MockRepository();

		[Given]
		public void a_service_failing_on_start_and_its_description_and_the_host()
		{
			_beforeException = new ManualResetEvent(false);
			_coordinatorStarted = new ManualResetEvent(false); // make sure we have reached blocking Run()
			// and a host using this coordinator
			_consoleHost = new ConsoleRunHost(new WindowsServiceDescription("ugly", "ugly$service"),
			                                  new CoordinatorStub(_beforeException, _coordinatorStarted));

			// and a thread pool with at least "enough" threads (one for test, one for cancel (GUI) and one to wait on
			ThreadPool.SetMinThreads(5, 5);
		}

		[Then]
		public void when_we_call_start_and_then_try_to_exit_before_start_is_complete_The_mre_should_only_be_set_once()
		{
			var runDone = new ManualResetEvent(false);

			ThreadPool.QueueUserWorkItem(_ =>
				{
					_consoleHost.Run();
					runDone.Set();
				});
			
			ThreadPool.QueueUserWorkItem(_ =>
				{
					_coordinatorStarted.WaitOne();
					_consoleHost.HandleCancelKeyPress(null,  /* this little bastard is sealed and internal c'tor */ null);
					_consoleHost.HandleCancelKeyPress(null, null);
					_beforeException.Set();
				});

			runDone.WaitOne();
		}
	}
}