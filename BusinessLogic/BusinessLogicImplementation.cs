//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Diagnostics;
using UnderneathLayerAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
        #region ctor

        public BusinessLogicImplementation() : this(null) {}

        private void Move(object? x)
        {
            for (int i = 0; i < BallsList.Count; i++)
            {
                layerBellow.UpdateBallPosition(i);
            }
            HandleCollision();
        }


        private readonly Timer MoveTimer;

        internal BusinessLogicImplementation(UnderneathLayerAPI? underneathLayer)
        {
            layerBellow = underneathLayer == null ? UnderneathLayerAPI.GetDataLayer() : underneathLayer;
            MoveTimer = new Timer(Move, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(1000 / 60));
            BallsList = new List<Ball>();
        }

        #endregion ctor

        #region BusinessLogicAbstractAPI

        public override void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            MoveTimer.Dispose();
            layerBellow.Dispose();
            Disposed = true;
        }

        public override void Start(int numberOfBalls, Action<IPosition, double, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));
            this.numberOfBalls = numberOfBalls;
            layerBellow.Start(
                numberOfBalls,
                (startingPosition, databall) =>
                {
                    Ball ball = new Ball(databall);
                    BallsList.Add(ball);
                    ball.NewVelocityNotification +=
                        (sender, args) =>
                        {
                            layerBellow.SetVelocity(args.x, args.y, databall);
                        };
                    upperLayerHandler(
                        new Position(startingPosition.x, startingPosition.x),
                        databall.Radius*2,
                        ball
                    );
                }
            );
        }

        #endregion BusinessLogicAbstractAPI

        #region private

        private int numberOfBalls;

        private bool Disposed = false;

        private readonly UnderneathLayerAPI layerBellow;

        private List<Ball> BallsList;
        private void HandleCollision()
        {
            Parallel.For(0, BallsList.Count, i =>
            {
                lock (BallsList[i])
                {
                    BallsList[i].HandleBorderCollision(600, 300, 4);
                    for (int j = i + 1; j < BallsList.Count; j++)
                    {
                        lock (BallsList[j])
                        {
                            BallsList[i].HandleBallCollision(BallsList[j]);
                        }
                    }
                }
            });
        }

        #endregion private

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

        #endregion TestingInfrastructure
    }
}