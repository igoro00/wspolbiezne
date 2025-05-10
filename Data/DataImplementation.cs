//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System;
using System.Diagnostics;

namespace TP.ConcurrentProgramming.Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        #region ctor

        public DataImplementation(){}

        #endregion ctor

        #region DataAbstractAPI

        public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));
            Random random = new Random();
            for (int i = 0; i < numberOfBalls; i++)
            {
                Vector startingPosition = new(random.Next(100, 400 - 100), random.Next(100, 400 - 100));
                Vector initialVelocity = new(random.Next(-8, 8), random.Next(-8, 8));
                Ball newBall = new(startingPosition, initialVelocity, random.Next(5,30)); //todo : zmienić
                BallsList.Add(newBall);
                upperLayerHandler(startingPosition, newBall);
            }
        }

        public override void UpdateBallPosition(int i)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));
            if (i < 0 || i >= BallsList.Count)
                throw new ArgumentOutOfRangeException(nameof(i), "Index out of range");
            Ball ball = BallsList[i];
            ball.Move();
        }

        public override void SetVelocity(double VelocityX, double VelocityY, IBall ball)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));
            if (ball == null)
            {
                throw new ArgumentNullException(nameof(ball), "Ball cannot be null");
            }
            ball.Velocity = new Vector(VelocityX, VelocityY);
        }

        #endregion DataAbstractAPI
        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    BallsList.Clear();
                }
                Disposed = true;
            }
            else
                throw new ObjectDisposedException(nameof(DataImplementation));
        }

        public override void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        #region private

        //private bool disposedValue;
        private bool Disposed = false;
        private List<Ball> BallsList = [];

        #endregion private

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckBallsList(Action<IEnumerable<IBall>> returnBallsList)
        {
            returnBallsList(BallsList);
        }

        [Conditional("DEBUG")]
        internal void CheckNumberOfBalls(Action<int> returnNumberOfBalls)
        {
            returnNumberOfBalls(BallsList.Count);
        }

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

        #endregion TestingInfrastructure
    }
}