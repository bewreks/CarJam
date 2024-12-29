using CarJam.Scripts.Queues.Parking.Models;
using UnityEngine;
using Zenject;
namespace CarJam.Scripts.Queues.Parking.Presenters
{
    public class ParkingPresenter
    {
        [Inject] private ParkingModel.Factory _modelFactory;
        
        private ParkingModel _model;

        public ParkingModel Model => _model;

        [Inject]
        private void Construct(Vector3 rbPoint, Vector3 ltPoint)
        {
            _model = _modelFactory.Create();
            
            var xMax = Mathf.Max(rbPoint.x, ltPoint.x);
            var xMin = Mathf.Min(rbPoint.x, ltPoint.x);
            var yMax = Mathf.Max(rbPoint.z, ltPoint.z);
            var yMin = Mathf.Min(rbPoint.z, ltPoint.z);
            
            _model.LtPoint = new Vector3(xMin, 0, yMax);
            _model.RtPoint = new Vector3(xMax, 0, yMax);
            _model.RbPoint = new Vector3(xMax, 0, yMin);
            _model.LbPoint = new Vector3(xMin, 0, yMin);

            _model.LeftPlane = new Plane(Vector3.right, _model.LtPoint);
            _model.RightPlane = new Plane(Vector3.left, _model.RbPoint);
            _model.TopPlane = new Plane(Vector3.back, _model.RtPoint);
            _model.BottomPlane = new Plane(Vector3.forward, _model.LbPoint);
        }

        public Plane GetNearestPlaneByDirection(Vector3 direction, Vector3 position)
        {

            var dirNormalized = direction;
            var ltNormalized = _model.LtPoint - position;
            var rbNormalized = _model.RbPoint - position;
            var rtNormalized = _model.RtPoint - position;
            var lbNormalized = _model.LbPoint - position;
            
            var ltDot = Vector3.Dot(dirNormalized, ltNormalized.normalized);
            var rtDot = Vector3.Dot(dirNormalized, rtNormalized.normalized);
            var rbDot = Vector3.Dot(dirNormalized, rbNormalized.normalized);
            var lbDot = Vector3.Dot(dirNormalized, lbNormalized.normalized);

            if (ltDot > rtDot && ltDot > rbDot && ltDot > lbDot)
            {
                return _model.LeftPlane;
            }
            if (rtDot > ltDot && rtDot > rbDot && rtDot > lbDot)
            {
                return _model.TopPlane;
            }
            if (rbDot > ltDot && rbDot > rtDot && rbDot > lbDot)
            {
                return _model.RightPlane;
            }
            if (lbDot > ltDot && lbDot > rtDot && lbDot > rbDot)
            {
                return _model.BottomPlane;
            }

            return _model.BottomPlane;
        }

        public class Factory : PlaceholderFactory<Vector3, Vector3, ParkingPresenter>
        {
        }
    }
}
