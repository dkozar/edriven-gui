using System;
using System.Collections.Generic;
using eDriven.Gui.Managers;
using eDriven.Gui.States;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// Base component class
    /// </summary>
    public partial class Component
    {
        /**
         *  
         *  Storage for the currentState property.
         */
        private string _currentState;

        /**
         *  
         *  Pending current state name.
         */
        private string _requestedCurrentState;

        /**
         *  
         *  Flag that is set when the currentState has changed and needs to be
         *  committed.
         *  This property name needs the initial underscore to avoid collisions
         *  with the "currentStateChange" event attribute.
         */
        private bool _currentStateChanged;
        
        /// <summary>
        /// 
        /// </summary>
        public string CurrentState
        {
            get
            {
                return _currentStateChanged ? _requestedCurrentState : _currentState;
            }
            set
            {
                SetCurrentState(value, true);
            }
        }

        /**
         *  
         *  Backing variable for CurrentStateDeferred property
         */
        private string _currentStateDeferred;
    
        /**
         *  
         *  Version of currentState property that defers setting currentState
         *  until commitProperties() time. This is used by SetProperty.remove()
         *  to avoid causing state transitions when currentState is being rolled
         *  back in a state change operation just to be set immediately after to the
         *  actual new currentState value. This avoids unnecessary, and sometimes
         *  incorrect, use of transitions based on this transient state of currentState.
         */
        internal string CurrentStateDeferred
        {
            get
            {
                return _currentStateDeferred ?? _currentState;
            }
            set
            {
                _currentStateDeferred = value;
                if (value != null)
                    InvalidateProperties();
            }
        }

        private List<State> _states = new List<State>();
        /// <summary>
        /// Component states
        /// </summary>
// ReSharper disable once MemberCanBeProtected.Global
        public List<State> States
        {
// ReSharper disable once MemberCanBePrivate.Global
            get { return _states; }
            set { _states = value; }
        }

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="playTransition"></param>
        public virtual void SetCurrentState(string stateName, bool playTransition/* = true*/)
        {
            // Flex 4 has no concept of an explicit base state, so ensure we
            // fall back to something appropriate.
            stateName = IsBaseState(stateName) ? GetDefaultState() : stateName;

            // Only change if the requested state is different. Since the root
            // state can be either null or "", we need to add additional check
            // to make sure we're not going from null to "" or vice-versa.
            if (stateName != _currentState &&
                !(IsBaseState(stateName) && IsBaseState(_currentState)))
            {
                _requestedCurrentState = stateName;
                // Don't play transition if we're just getting started
                // In Flex4, there is no "base state", so if IsBaseState() is true
                // then we're just going into our first real state
                _playStateTransition = /*(this is IStateClient2) && */!IsBaseState(_currentState) && playTransition;
                if (Initialized)
                {
                    CommitCurrentState();
                }
                else
                {
                    _currentStateChanged = true;
                    InvalidateProperties();
                }
            }
        }

        /// <summary>
        /// Sets current state
        /// </summary>
        /// <param name="stateName"></param>
        public virtual void SetCurrentState(string stateName)
        {
            SetCurrentState(stateName, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public bool HasState(string stateName)
        {
            return (GetState(stateName, false) != null);
        }

        /**
         *  
         *  Returns the state with the specified name, or null if it doesn't exist.
         *  If multiple states have the same name the first one will be returned.
         */
        private State GetState(string stateName, bool throwOnUndefined/*=true*/)
        {
            if (null == States || IsBaseState(stateName))
                return null;

            //Debug.Log("GetState: " + this);

            /*if (this is ImageButtonSkin)
                Debug.Log(@"Button states: 
" + ListUtil<State>.Format(States));*/

            // Do a simple linear search for now. This can
            // be optimized later if needed.
            foreach (var t in States)
            {
                if (t.Name == stateName)
                    return t;
            }

            if (throwOnUndefined)
            {
                throw new Exception(string.Format(@"Undefined state ""{0}"": {1}", stateName, this));
            }
            return null;
        }

        /**
         *  
         *  Returns true if the passed in state name is the 'base' state, which
         *  is currently defined as null or ""
         */
        private bool IsBaseState(string stateName)
        {
            return string.IsNullOrEmpty(stateName);
        }

        /**
         *  
         *  Returns the default state. For Flex 4 and later we return the base
         *  the first defined state, otherwise (Flex 3 and earlier), we return
         *  the base (null) state.
         */
        private string GetDefaultState()
        {
            return (States.Count > 0) ? States[0].Name : null;
        }

        /**
         *  
         *  Find the deepest common state between two states. For example:
         *
         *  State A
         *  State B basedOn A
         *  State C basedOn A
         *
         *  findCommonBaseState(B, C) returns A
         *
         *  If there are no common base states, the root state ("") is returned.
         */
        private string FindCommonBaseState(string state1, string state2)
        {
            var firstState = GetState(state1, true);
            var secondState = GetState(state2, true);

            // Quick exit if either state is the base state
            if (null == firstState || null == secondState)
                return string.Empty;

            // Quick exit if both states are not based on other states
            if (IsBaseState(firstState.BasedOn) && IsBaseState(secondState.BasedOn))
                return string.Empty;

            // Get the base states for each state and walk from the top
            // down until we find the deepest common base state.
            var firstBaseStates = GetBaseStates(firstState);
            var secondBaseStates = GetBaseStates(secondState);
            var commonBase = string.Empty;

            if (firstBaseStates.Count > 0 && secondBaseStates.Count > 0)
            {
                while (firstBaseStates[firstBaseStates.Count - 1] ==
                   secondBaseStates[secondBaseStates.Count - 1])
                {
                    var index = secondBaseStates.Count - 1;
                    commonBase = firstBaseStates[index];

                    firstBaseStates.RemoveAt(index);
                    secondBaseStates.RemoveAt(index);

                    if (0 == firstBaseStates.Count || 0 == secondBaseStates.Count)
                        break;
                }
            }

            // Finally, check to see if one of the states is directly based on the other.
            if (firstBaseStates.Count>0 &&
                firstBaseStates[firstBaseStates.Count - 1] == secondState.Name)
            {
                commonBase = secondState.Name;
            }
            else if (secondBaseStates.Count>0 &&
                     secondBaseStates[secondBaseStates.Count - 1] == firstState.Name)
            {
                commonBase = firstState.Name;
            }

            return commonBase;
        }

        /**
         *  
         *  Returns the base states for a given state.
         *  This Array is in high-to-low order - the first entry
         *  is the immediate basedOn state, the last entry is the topmost
         *  basedOn state.
         */
        private List<string> GetBaseStates(State state)
        {
            var baseStates = new List<string>();

            // Push each basedOn name
            while (null != state && null != state.BasedOn)
            {
                baseStates.Add(state.BasedOn);
                state = GetState(state.BasedOn, true);
            }

            return baseStates;
        }

        /**
         *  
         *  Remove the overrides applied by a state, and any
         *  states it is based on.
         */
        private void RemoveState(string stateName, string lastState)
        {
            var state = GetState(stateName, true);

            if (stateName == lastState)
                return;

            // Remove existing state overrides.
            // This must be done in reverse order
            if (null != state)
            {
                // Dispatch the "exitState" event
                state.DispatchExitState();

                var overrides = state.Overrides;

                try
                {
                    for (var i = overrides.Count - 1; i >= 0; i--)
                        overrides[i].Remove(this);
                }
                catch (ArgumentException ex)
                {
                    throw new Exception("Cannot remove state", ex);
                }

                // Remove any basedOn deltas last
                if (state.BasedOn != lastState)
                    RemoveState(state.BasedOn, lastState);
            }
        }

        /**
         *  
         *  Apply the overrides from a state, and any states it
         *  is based on.
         */
        private void ApplyState(string stateName, string lastState)
        {
            var state = GetState(stateName, true);

            if (stateName == lastState)
                return;

            if (null != state)
            {
                // Apply "basedOn" overrides first
                if (state.BasedOn != lastState)
                    ApplyState(state.BasedOn, lastState);

                // Apply new state overrides
                var overrides = state.Overrides;

                try
                {
                    foreach (IOverride @override in overrides)
                        @override.Apply(this);
                }
                catch (ArgumentException ex)
                {
                    throw new Exception("Cannot apply state", ex);
                }

                // Dispatch the "enterState" event
                state.DispatchEnterState();
            }
        }

        /**
         *  
         *  Initialize the state, and any states it is based on
         */
        private void InitializeState(string stateName)
        {
            var state = GetState(stateName, true);

            while (null != state)
            {
                state.Initialize();
                state = GetState(state.BasedOn, true);
            }
        }

        private void CommitCurrentState()
        {
            var nextTransition = _playStateTransition ? GetTransition(_currentState, _requestedCurrentState) : null;
            var commonBaseState = FindCommonBaseState(_currentState, _requestedCurrentState);
// ReSharper disable once UnusedVariable.Compiler
            StateChangeEvent e;
            var oldState = _currentState ?? string.Empty;
// ReSharper disable once UnusedVariable
            var destination = GetState(_requestedCurrentState, true);

            // Stop any transition that may still be playing
// ReSharper disable once UnusedVariable.Compiler
            float prevTransitionFraction;
            if (null != _currentTransition)
            {
                /*if (_currentTransition.AutoReverse &&
                    transitionFromState == _requestedCurrentState &&
                    transitionToState == _currentState)
                {
                    if (_currentTransition.Effect.Duration == 0)
                        prevTransitionFraction = 0;
                    else
                        prevTransitionFraction = 
                            _currentTransition.Effect.playheadTime /
                            getTotalDuration(_currentTransition.Effect);
                }
                _currentTransition.Effect.end();*/
            }

            // Initialize the state we are going to.
            InitializeState(_requestedCurrentState);

            // Capture transition start values
            /*if (null != nextTransition)
                nextTransition.Effect.captureStartValues();*/

            // Dispatch currentStateChanging event
            if (HasEventListener(StateChangeEvent.CURRENT_STATE_CHANGING)) 
            {
                StateChangeEvent sce = new StateChangeEvent(StateChangeEvent.CURRENT_STATE_CHANGING)
                {
                    OldState = oldState,
                    NewState = _requestedCurrentState ?? ""
                };
                DispatchEvent(sce);
            }
        
            // If we're leaving the base state, send an exitState event
            if (IsBaseState(_currentState) && HasEventListener(FrameworkEvent.EXIT_STATE))
                DispatchEvent(new FrameworkEvent(FrameworkEvent.EXIT_STATE));

            // Remove the existing state
            RemoveState(_currentState, commonBaseState);
            _currentState = _requestedCurrentState;

            // Check for state specific styles
            StateChanged(oldState, _currentState, true);

            // If we're going back to the base state, dispatch an
            // enter state event, otherwise apply the state.
            if (IsBaseState(CurrentState)) 
            {
                if (HasEventListener(FrameworkEvent.ENTER_STATE))
                    DispatchEvent(new FrameworkEvent(FrameworkEvent.ENTER_STATE)); 
            }
            else
                ApplyState(_currentState, commonBaseState);

            // Dispatch currentStateChange
            if (HasEventListener(StateChangeEvent.CURRENT_STATE_CHANGE))
            {
                StateChangeEvent sce = new StateChangeEvent(StateChangeEvent.CURRENT_STATE_CHANGE)
                {
                    OldState = oldState,
                    NewState = _currentState ?? string.Empty
                };
                DispatchEvent(sce);
            }
        
            if (null != nextTransition)
            {
                // Force a validation before playing the transition effect
                InvalidationManager.Instance.ValidateNow();
                _currentTransition = nextTransition;
                _transitionFromState = oldState;
                _transitionToState = _currentState;
                /*nextTransition.Effect.addEventListener(EffectEvent.EFFECT_END, 
                    transition_effectEndHandler);
                nextTransition.effect.play();
                if (null != prevTransitionFractionion && 
                    nextTransition.Effect.duration != 0)
                    nextTransition.Effect.playheadTime = (1 - prevTransitionFraction) * 
                        getTotalDuration(nextTransition.Effect);*/
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldState"></param>
        /// <param name="newState"></param>
        /// <param name="recursive"></param>
// ReSharper disable once MemberCanBePrivate.Global
// ReSharper disable UnusedParameter.Global
        protected void StateChanged(string oldState, string newState, bool recursive)
// ReSharper restore UnusedParameter.Global
        {
            RegenerateStyleCache(recursive);
            // This test only checks for pseudo conditions on the subject of the selector.
            // Pseudo conditions on ancestor selectors are not detected - eg:
            //    List ScrollBar:inactive #track
            // The track styles will not change when the scrollbar is in the inactive state.
            /*if (CurrentCSSState && oldState != newState &&
                   (StyleManager.Instance.hasPseudoCondition(oldState) ||
                    StyleManager.Instance.hasPseudoCondition(newState)))
            {
                RegenerateStyleCache(recursive);
                StyleChanged(null, null);
                NotifyStyleChangeInChildren(null, null, recursive);
            }*/
        }

        #endregion

        #region Transitions

        /**
         *  
         *  Flag to play state transition
         */
        private bool _playStateTransition;

        /**
         *  
         *  Transition currently playing.
         */
        private Transition _currentTransition;

        private List<Transition> _transitions = new List<Transition>();
        /// <summary>
        /// Component states
        /// </summary>
// ReSharper disable once MemberCanBePrivate.Global
        public List<Transition> Transitions
        {
            get { return _transitions; }
// ReSharper disable once UnusedMember.Global
            set { _transitions = value; }
        }

        /**
         * These variables cache the transition state from/to information for
         * the transition currently running. This information is used when
         * determining what to do with a new transition that interrupts the
         * running transition.
         */
        private string _transitionFromState;
        private string _transitionToState;

        /**
         *  
         *  Find the appropriate transition to play between two states.
         */
        private Transition GetTransition(string oldState, string newState)
        {
            Transition result = null;   // Current candidate
            var priority = 0;           // Priority     fromState   toState
                                            //    1             *           *
                                            //    2           match         *
                                            //    3             *         match
                                            //    4           match       match

            if (null == Transitions)
                return null;

            if (null == oldState)
                oldState = string.Empty;

            if (null == newState)
                newState = string.Empty;

            foreach (var t in Transitions)
            {
                if (t.FromState == Transition.ALL_STATES && t.ToState == Transition.ALL_STATES && priority < 1)
                {
                    result = t;
                    priority = 1;
                }
                else if (t.FromState == oldState && t.ToState == Transition.ALL_STATES && priority < 2)
                {
                    result = t;
                    priority = 2;
                }
                else if (t.FromState == Transition.ALL_STATES && t.ToState == newState && priority < 3)
                {
                    result = t;
                    priority = 3;
                }
                else if (t.FromState == oldState && t.ToState == newState && priority < 4)
                {
                    result = t;
                    priority = 4;

                    // Can't get any higher than this, let's go.
                    break;
                }
            }
            // If Transition does not contain an effect, then don't return it
            // because there is no transition effect to run
            if (null != result && null == result.Effect)
                result = null;

            return result;
        }

        #endregion

    }
}
