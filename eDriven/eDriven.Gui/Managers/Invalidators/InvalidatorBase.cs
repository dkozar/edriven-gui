using eDriven.Gui.Components;
using eDriven.Gui.Util;

namespace eDriven.Gui.Managers.Invalidators
{
    internal class InvalidatorBase
    {
        public bool Invalid { get; protected set; }
        public bool InvalidClient { get; protected set; }

        private readonly PriorityQueue _updateCompleteQueue;
        
        internal PriorityQueue Queue = new PriorityQueue();

        internal InvalidatorBase(PriorityQueue updateCompleteQueue)
        {
            _updateCompleteQueue = updateCompleteQueue;
        }

        protected void HandleUpdateCompletePendingFlag(InvalidationManagerClient obj)
        {
            if (!obj.UpdateFlag)
            {
                _updateCompleteQueue.AddObject(obj, obj.NestLevel);
                obj.UpdateFlag = true;
            }
        }

        public void Invalidate(InvalidationManagerClient obj, bool invalidateClientFlag)
        {
            /*if (_targetLevel <= obj.NestLevel)
                invalidateClientFlag = true;*/

            Queue.AddObject(obj, obj.NestLevel);
            Invalid = true;
            InvalidClient = invalidateClientFlag;
        }
    }
}