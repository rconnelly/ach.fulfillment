namespace Ach.Fulfillment.Scheduler
{
    using Ach.Fulfillment.Scheduler.Common;
    using Ach.Fulfillment.Scheduler.Configuration;

    using Topshelf;

    // todo: check msi installs windows service
    // todo: check unit tests
    // todo: check big nacha file generation
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = HostFactory.New(x =>   
            {
                x.Service<QuartzServer>(s =>
                    {
                        s.ConstructUsing(builder =>
                            {
                                var server = new QuartzServer();
                                server.Initialize();
                                return server;
                            });
                        s.WhenStarted(server => server.Start());
                        s.WhenPaused(server => server.Pause());
                        s.WhenContinued(server => server.Resume());
                        s.WhenStopped(server => server.Stop());
                    });

                x.RunAsLocalSystem();                            

                x.SetDescription(Registry.ServiceDescription);        
                x.SetDisplayName(Registry.ServiceDisplayName);                      
                x.SetServiceName(Registry.ServiceName);
            });

            host.Run();
        }
    }
}
