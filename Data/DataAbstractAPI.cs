//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.ComponentModel.DataAnnotations;

namespace TP.ConcurrentProgramming.Data
{
    public abstract class DataAbstractAPI : IDisposable
    {
        #region Layer Factory

        public static DataAbstractAPI GetDataLayer()
        {
            return modelInstance.Value;
        }

        public static Logger GetLogger()
        {
            return loggerInstance.Value;
        }

        #endregion Layer Factory

        #region public API

        public abstract void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler);

        public abstract void UpdateBallPosition(int i);

        public abstract void SetVelocity(double VelocityX, double VelocityY, IBall ball);

        public abstract IVector? CreateVector(double? x, double? y);
        
        #endregion public API

        #region IDisposable

        public abstract void Dispose();

        #endregion IDisposable

        #region private

        private static Lazy<DataAbstractAPI> modelInstance = new(() => new DataImplementation());
        private static Lazy<Logger> loggerInstance = new(() => new Logger());

        #endregion private
    }

    public interface IVector
    {
        /// <summary>
        /// The X component of the vector.
        /// </summary>
        double x { get; init; }

        /// <summary>
        /// The y component of the vector.
        /// </summary>
        double y { get; init; }
    }

    public interface IBall
    {

        event EventHandler<IVector> NewPositionNotification;

        IVector Velocity { get; set; }
        IVector Position { get; set; }
        double Radius { get; set; }
    }

    public interface ICollision
    {
        DateTime TimeStamp { get; init; }
        int BallId1 { get; init; }
        int? BallId2 { get; init; }
        double Radius1 { get; init; }
        double? Radius2 { get; init; }
        IVector Position1 { get; init; }
        IVector? Position2 { get; init; }
        IVector Velocity1Before { get; init; }
        IVector? Velocity2Before { get; init; }
        IVector Velocity1After { get; init; }
        IVector? Velocity2After { get; init; }
    }
}