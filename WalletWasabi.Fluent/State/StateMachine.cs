using System.Collections.Generic;

namespace WalletWasabi.Fluent.State;

public class StateMachine<TState, TTrigger> where TTrigger : Enum where TState : Enum
{
	private StateContext _currentState;
	private readonly Dictionary<TState, StateContext> _states;
	private OnTransitionedDelegate? _onTransitioned;

	public delegate void OnTransitionedDelegate(TTrigger trigger, TState from, TState to);

	public TState CurrentState => _currentState.StateId;

	public StateMachine(TState initialState)
	{
		_states = new Dictionary<TState, StateContext>();

		_currentState = Configure(initialState);
	}

	public StateMachine<TState, TTrigger> OnTransitioned(OnTransitionedDelegate onTransitioned)
	{
		_onTransitioned = onTransitioned;

		return this;
	}

	public StateContext Configure(TState state)
	{
		if (!_states.ContainsKey(state))
		{
			var result = new StateContext(this, state);

			_states.Add(state, result);

			return result;
		}

		return _states[state];
	}

	public void Fire(TTrigger trigger)
	{
		if (_currentState.CanTransit(trigger))
		{
			Goto(trigger, _currentState.GetDestination(trigger));
		}
		else if (_currentState.Parent is { } && _currentState.Parent.CanTransit(trigger))
		{
			Goto(trigger, _currentState.Parent.StateId, true, false);
			Goto(trigger, _currentState.GetDestination(trigger));
		}
	}

	public void Start()
	{
		_currentState.Enter();
	}

	private void Goto(TTrigger trigger, TState state, bool exit = true, bool enter = true)
	{
		if (_states.ContainsKey(state))
		{
			if (exit)
			{
				_currentState.Exit();
			}

			var old = _currentState.StateId;

			_currentState = _states[state];

			_onTransitioned?.Invoke(trigger, old, _currentState.StateId);

			if (enter)
			{
				_currentState.Enter();
			}
		}
	}

	public class StateContext
	{
		private Dictionary<TTrigger, TState> _permittedTransitions;
		private readonly StateMachine<TState, TTrigger> _owner;

		private StateContext? _parent;
		private List<Action> _entryActions;
		private List<Action> _exitActions;

		public TState StateId { get; }

		public StateContext? Parent => _parent;

		public StateContext(StateMachine<TState, TTrigger> owner, TState state)
		{
			_owner = owner;
			StateId = state;

			_entryActions = new();
			_exitActions = new();
			_permittedTransitions = new();
		}

		public StateContext SubStateOf(TState parent)
		{
			_parent = _owner._states[parent];

			return this;
		}

		public StateContext Permit(TTrigger trigger, TState state)
		{
			_permittedTransitions[trigger] = state;

			return this;
		}

		public StateContext OnEntry(Action action)
		{
			_entryActions.Add(action);

			return this;
		}

		public StateContext OnExit(Action action)
		{
			_exitActions.Add(action);

			return this;
		}

		internal void Enter()
		{
			foreach (var action in _entryActions)
			{
				action();
			}
		}

		internal void Exit()
		{
			foreach (var action in _exitActions)
			{
				action();
			}
		}

		internal bool CanTransit(TTrigger trigger)
		{
			return _permittedTransitions.ContainsKey(trigger);
		}

		internal TState GetDestination(TTrigger trigger)
		{
			return _permittedTransitions[trigger];
		}
	}
}