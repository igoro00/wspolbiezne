//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Data;
using System.Diagnostics;
using System.Numerics;
using TP.ConcurrentProgramming.Data;
using UnderneathLayerAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
        #region ctor

        public BusinessLogicImplementation() : this(null)
        {
            BallsList = new List<Ball>();
        }

        private void Move(object? x)
        {
            for (int i = 0; i < numberOfBalls; i++)
            {
                layerBellow.UpdateBallPosition(i);
            }
            HandleCollision();
        }


        private readonly Timer MoveTimer;

        internal BusinessLogicImplementation(UnderneathLayerAPI? underneathLayer)
        {
            layerBellow = underneathLayer == null ? UnderneathLayerAPI.GetDataLayer() : underneathLayer;
            MoveTimer = new Timer(Move, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(1000 / 25));
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

        public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
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
                BallsList[i].HandleBorderCollision(380, 400);
                for (int j = i + 1; j < BallsList.Count; j++)
                {
                    BallsList[i].HandleBallCollision(BallsList[j]);
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