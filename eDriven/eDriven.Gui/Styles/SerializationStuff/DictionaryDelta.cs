using System.Collections.Generic;

namespace eDriven.Gui.Styles
{
    /// <summary>
    /// Evaluates the change in the (serialized) style declaration<br/>
    /// The serialized value could change only when using Editor<br/>
    /// The delta contains:<br/>
    /// 1. additions (new styles, added via the edit dialog)<br/>
    /// 2. removals (removed styles)<br/>
    /// 3. styles having changed values<br/>
    /// The delta is used by the framework to change styles on the fly<br/>
    /// The implementation differs depending of the styling module used
    /// </summary>
    public class DictionaryDelta
    {
        private readonly Dictionary<string, object> _before = new Dictionary<string, object>();
        private readonly Dictionary<string, object> _after = new Dictionary<string, object>();

        /// <summary>
        /// Added properties
        /// </summary>
        public readonly Dictionary<string, object> Additions = new Dictionary<string, object>();

        /// <summary>
        /// Removed properties
        /// </summary>
        public readonly Dictionary<string, object> Removals = new Dictionary<string, object>();

        /// <summary>
        /// Changed properties
        /// </summary>
        public readonly Dictionary<string, object> Updates = new Dictionary<string, object>();

        /// <summary>
        /// Snapshot before
        /// </summary>
        /// <param name="array"></param>
        public void SnapshotBefore(NameValueBase[] array)
        {
            _before.Clear();
            foreach (NameValueBase styleProperty in array)
            {
                var value = styleProperty.Value;
                if (!Equals(null, value)) // we are considering null to act as the StyleDeclaration.UNDEFINED here, because we want to fall back to hardcoded values
                    _before[styleProperty.Name] = value; // TODO: Examine why the null comparison allows this
            }
        }

        /// <summary>
        /// Snapshot after
        /// </summary>
        /// <param name="array"></param>
        public void SnapshotAfter(NameValueBase[] array)
        {
            _after.Clear();
            foreach (NameValueBase styleProperty in array)
            {
                var value = styleProperty.Value;
                if (!Equals(null, value)) // we are considering null to act as the StyleDeclaration.UNDEFINED here, because we want to fall back to hardcoded values
                    _after[styleProperty.Name] = value;       // TODO: Examine why the null comparison allows this
            }
        }

        /// <summary>
        /// Finds the added, removed and updated properties
        /// </summary>
        public void Process()
        {
            Additions.Clear();
            Removals.Clear();
            Updates.Clear();

            // 1. additions
            foreach (string s in _after.Keys)
            {
                if (!_before.ContainsKey(s))
                    Additions[s] = _after[s];
            }
            // 2. removals
            foreach (string s in _before.Keys)
            {
                if (!_after.ContainsKey(s))
                    Removals[s] = _before[s];
            }
            // 3. updates
            foreach (string s in _after.Keys)
            {
                if (_before.ContainsKey(s) && !_before[s].Equals(_after[s]))
                    Updates[s] = _after[s];
            }
        }
    }
}