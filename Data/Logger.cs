using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP.ConcurrentProgramming.Data
{

    public class Logger: IDisposable
    {
        public Logger()
        {
            logger();
        }
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        public void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    logQueue.CompleteAdding();
                    isLogQueueEmpty.WaitOne(500);
                    isLogQueueEmpty.Dispose();
                }
                Disposed = true;
            }
            else
                throw new ObjectDisposedException(nameof(DataImplementation));
        }


        private BlockingCollection<ICollision> logQueue = new();
        private readonly ManualResetEvent isLogQueueEmpty = new(false);
        private bool Disposed = false;
        private void logger()
        {
            Task.Run(() => {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss");
                string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log_" + timestamp + ".csv");
                bool fileExists = File.Exists(logFilePath);

                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    if (!fileExists)
                    {
                        writer.WriteLine("Timestamp,Ball1,Ball2,r1,r2,x1,y1,x2,y2,vX1old,vY1old,vX2old,vY2old,vX1new,vY1new,vX2new,vY2new");
                    }

                    foreach (var item in logQueue.GetConsumingEnumerable())
                    {
                        StringBuilder sb = new();
                        sb.Append($"{item.TimeStamp:yyyy-MM-dd HH:mm:ss.fff}");
                        sb.Append($",{item.BallId1.ToString()}");

                        sb.Append($",{item.BallId2?.ToString() ?? ""}");

                        sb.Append($",{item.Radius1.ToString()}");
                        sb.Append($",{item.Radius2?.ToString() ?? ""}");

                        sb.Append($",{item.Position1.x.ToString()}");
                        sb.Append($",{item.Position1.y.ToString()}");
                        sb.Append($",{item.Position2?.x.ToString() ?? ""}");
                        sb.Append($",{item.Position2?.x.ToString() ?? ""}");

                        sb.Append($",{item.Velocity1Before.x.ToString()}");
                        sb.Append($",{item.Velocity1Before.y.ToString()}");
                        sb.Append($",{item.Velocity2Before?.x.ToString() ?? ""}");
                        sb.Append($",{item.Velocity2Before?.y.ToString() ?? ""}");

                        sb.Append($",{item.Velocity1After.x.ToString()}");
                        sb.Append($",{item.Velocity1After.y.ToString()}");
                        sb.Append($",{item.Velocity2After?.x.ToString() ?? ""}");
                        sb.Append($",{item.Velocity2After?.y.ToString() ?? ""}");

                        writer.WriteLine(sb.ToString());
                    }
                    writer.Flush();
                }
                isLogQueueEmpty.Set(); // Signal that the log queue is empty
            });
        }
        public void LogCollision(ICollision? collision)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));
            if (collision == null)
            {
                return;
            }
            logQueue.Add(collision);
        }
    }
}
