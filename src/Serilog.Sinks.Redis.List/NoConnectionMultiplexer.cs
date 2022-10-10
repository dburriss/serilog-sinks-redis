using StackExchange.Redis;
using StackExchange.Redis.Profiling;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Serilog.Sinks.Redis.List
{
    public class NoConnectionMultiplexer : IConnectionMultiplexer
    {
        private readonly string _redisUris;

        public NoConnectionMultiplexer(string redisUris)
        {
            _redisUris = redisUris;
        }


        public void RegisterProfiler(Func<ProfilingSession> profilingSessionProvider)
        {

        }

        public int GetHashSlot(RedisKey key)
        {
            return default(int);
        }

        public void ExportConfiguration(Stream destination, ExportOptions options = (ExportOptions)(-1))
        {

        }

        public void BeginProfiling(object forContext)
        {
            
        }

        public ProfiledCommandEnumerable FinishProfiling(object forContext, bool allowCleanupSweep = true)
        {
            return default(ProfiledCommandEnumerable);
        }

        public ServerCounters GetCounters()
        {
            return null;
        }

        public EndPoint[] GetEndPoints(bool configuredOnly = false)
        {
            return null;
        }

        public void Wait(Task task)
        {
            
        }

        public T Wait<T>(Task<T> task)
        {
            return default(T);
        }

        public void WaitAll(params Task[] tasks)
        {
            
        }

        public int HashSlot(RedisKey key)
        {
            return default(int);
        }

        public ISubscriber GetSubscriber(object asyncState = null)
        {
            return null;
        }

        public IDatabase GetDatabase(int db = -1, object asyncState = null)
        {
            return null;
        }

        public IServer GetServer(string host, int port, object asyncState = null)
        {
            return null;
        }

        public IServer GetServer(string hostAndPort, object asyncState = null)
        {
            return null;
        }

        public IServer GetServer(IPAddress host, int port)
        {
            return null;
        }

        public IServer GetServer(EndPoint endpoint, object asyncState = null)
        {
            return null;
        }

        public IServer[] GetServers()
        {
            throw new NotImplementedException();
        }

        public Task<bool> ConfigureAsync(TextWriter log = null)
        {
            return new Task<bool>(() => false);
        }

        public bool Configure(TextWriter log = null)
        {
            return false;
        }

        public string GetStatus()
        {
            return string.Empty;
        }

        public void GetStatus(TextWriter log)
        {
            
        }

        public void Close(bool allowCommandsToComplete = true)
        {
            
        }

        public Task CloseAsync(bool allowCommandsToComplete = true)
        {
            return new Task(() => { });
        }

        public void Dispose()
        {
            
        }

        public string GetStormLog()
        {
            return string.Empty;
        }

        public void ResetStormLog()
        {
            
        }

        public long PublishReconfigure(CommandFlags flags = CommandFlags.None)
        {
            return default(long);
        }

        public Task<long> PublishReconfigureAsync(CommandFlags flags = CommandFlags.None)
        {
            return new Task<long>(() => default(long));
        }

        public string ClientName { get; }
        public string Configuration { get; }
        public int TimeoutMilliseconds { get; }
        public long OperationCount { get; }
        public bool PreserveAsyncOrder { get; set; }
        public bool IsConnected { get; }
        public bool IncludeDetailInExceptions { get; set; }
        public int StormLogThreshold { get; set; }

        public bool IsConnecting { get; }

        public event EventHandler<RedisErrorEventArgs> ErrorMessage;
        public event EventHandler<ConnectionFailedEventArgs> ConnectionFailed;
        public event EventHandler<InternalErrorEventArgs> InternalError;
        public event EventHandler<ConnectionFailedEventArgs> ConnectionRestored;
        public event EventHandler<EndPointEventArgs> ConfigurationChanged;
        public event EventHandler<EndPointEventArgs> ConfigurationChangedBroadcast;
        public event EventHandler<HashSlotMovedEventArgs> HashSlotMoved;
        public ValueTask DisposeAsync()
        {
            return new ValueTask();
        }
    }
}