using Topshelf;

namespace Ach.Fulfillment.Scheduler
{
    /// <summary>
    /// The server's main entry point.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Host host = HostFactory.New(x =>   
            {
                x.Service<IQuartzServer>(s =>               
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

                x.SetDescription(Configuration.Configuration.ServiceDescription);
                x.SetDisplayName(Configuration.Configuration.ServiceDisplayName);
                x.SetServiceName(Configuration.Configuration.ServiceName);                       
            });

            host.Run();
        }

    }
}
