//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class Ball : IBall
    {
        public Data.IBall dataBall;

        public Ball(Data.IBall ball, Func<double?, double?, IVector?> cv)
        {
            dataBall = ball;
            createVector = cv;
            ball.NewPositionNotification += RaisePositionChangeEvent;
        }

        public void HandleBallCollision(Ball otherBall)
        {
            if (!IsBallCollision(otherBall)) return;

            // Różnica pozycji i prędkości
            double dx = dataBall.Position.x - otherBall.dataBall.Position.x;
            double dy = dataBall.Position.y - otherBall.dataBall.Position.y;
            double distance = Math.Sqrt(dx * dx + dy * dy);

            // Normalizacja wektora różnicy pozycji
            double nx = dx / distance;
            double ny = dy / distance;

            // Różnice prędkości
            double dvx = dataBall.Velocity.x - otherBall.dataBall.Velocity.x;
            double dvy = dataBall.Velocity.y - otherBall.dataBall.Velocity.y;

            // Iloczyn skalarny różnicy prędkości i wektora normalnego
            double dot = dvx * nx + dvy * ny;
            // Jeżeli kule się oddalają, nie kolidują fizycznie
            if (dot > 0) return;

            // Masa = promień (dla uproszczenia)
            double m1 = dataBall.Radius;
            double m2 = otherBall.dataBall.Radius;

            // Oblicz impuls
            double impulse = (2 * dot) / (m1 + m2);

            // Zaktualizuj 
            IPosition velocity1After = new Position(dataBall.Velocity.x - impulse * m2 * nx, dataBall.Velocity.y - impulse * m2 * ny);
            IPosition velocity2After = new Position(otherBall.dataBall.Velocity.x + impulse * m1 * nx, otherBall.dataBall.Velocity.y + impulse * m1 * ny);

            ILoggerEntry loggerEntry = OnCollision(otherBall, velocity1After, velocity2After);
            ILoggerEntry loggerEntry2 = otherBall.OnCollision(this, velocity2After, velocity1After);
          
            NewVelocityNotification?.Invoke(this, loggerEntry);
            otherBall.NewVelocityNotification?.Invoke(otherBall, loggerEntry2);
        }

        public void HandleBorderCollision(double width, double height, double borderThickness)
        {

            //lewo prawo
            if (dataBall.Velocity.x > 0) // jedzie w prawo
            {
                if (dataBall.Position.x + dataBall.Radius > width - (borderThickness*2)) // za bardzo w prawo
                {
                    NewVelocityNotification?.Invoke(this, OnCollision(null, new Position(-dataBall.Velocity.x, dataBall.Velocity.y), null));
                }
            }
            if (dataBall.Velocity.x < 0) // jedzie w lewo
            {
                if (dataBall.Position.x - dataBall.Radius < 0) // za bardzo w lewo
                {
                    NewVelocityNotification?.Invoke(this, OnCollision(null, new Position(-dataBall.Velocity.x, dataBall.Velocity.y), null));
                }
            }


            // gora dol
            if (dataBall.Velocity.y > 0) // jedzie w dół
            {
                if (dataBall.Position.y + dataBall.Radius > height - (borderThickness*2)) // za bardzo w dół
                {
                    NewVelocityNotification?.Invoke(this, OnCollision(null, new Position(dataBall.Velocity.x, -dataBall.Velocity.y), null));
                }
            }
            if (dataBall.Velocity.y < 0) // jedzie w góre
            {
                if (dataBall.Position.y - dataBall.Radius < 0) // za bardzo w góre
                {
                    NewVelocityNotification?.Invoke(this, OnCollision(null, new Position(dataBall.Velocity.x, -dataBall.Velocity.y), null));
                }
            }
        }

        public ILoggerEntry OnCollision(Ball? otherBall, IPosition newVelocity1, IPosition? newVelocity2)
        {
            return new LoggerEntry
            {
                TimeStamp = DateTime.Now,
                BallId1 = GetHashCode(),
                BallId2 = otherBall?.GetHashCode(),
                Radius1 = dataBall.Radius,
                Radius2 = otherBall?.dataBall?.Radius,
                Position1 = dataBall.Position,
                Position2 = otherBall?.dataBall?.Position,
                Velocity1Before = dataBall.Velocity,
                Velocity2Before = otherBall?.dataBall?.Velocity,
                Velocity1After = createVector(newVelocity1.x, newVelocity1.y)!,
                Velocity2After = createVector(newVelocity2?.x, newVelocity2?.y)
            };
        }

        public event EventHandler<ILoggerEntry>? NewVelocityNotification;

        #region IBall

        public event EventHandler<IPosition>? NewPositionNotification;

        #endregion IBall
        #region private
        private Func<double?, double?, IVector?> createVector;

        private bool IsBallCollision(Ball otherBall)
        {
            double dx = dataBall.Position.x - otherBall.dataBall.Position.x;
            double dy = dataBall.Position.y - otherBall.dataBall.Position.y;
            double distance = Math.Sqrt(dx * dx + dy * dy);
            return distance <= (dataBall.Radius + otherBall.dataBall.Radius);
        }

        private void RaisePositionChangeEvent(object? sender, Data.IVector e)
        {
            NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
        }

        #endregion private
    }
}