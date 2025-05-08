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
        public Ball(Data.IBall ball)
        {
            dataBall = ball;
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
            NewVelocityNotification?.Invoke(this, new Position(dataBall.Velocity.x - impulse * m2 * nx, dataBall.Velocity.y - impulse * m2 * ny));
            otherBall.NewVelocityNotification?.Invoke(otherBall, new Position(otherBall.dataBall.Velocity.x + impulse * m1 * nx, otherBall.dataBall.Velocity.y + impulse * m1 * ny));
            
        }

        public void HandleBorderCollision(double width, double height)
        {
            if (dataBall.Position.x + dataBall.Radius > width || dataBall.Position.x < 0)
            {
                NewVelocityNotification?.Invoke(this, new Position(-dataBall.Velocity.x, dataBall.Velocity.y));
            }
            if (dataBall.Position.y + dataBall.Radius > height || dataBall.Position.y < 0)
            {
                NewVelocityNotification?.Invoke(this, new Position(dataBall.Velocity.x, -dataBall.Velocity.y));
            }
        }

        public event EventHandler<IPosition>? NewVelocityNotification;

        #region IBall

        public event EventHandler<IPosition>? NewPositionNotification;

        #endregion IBall
        #region private

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