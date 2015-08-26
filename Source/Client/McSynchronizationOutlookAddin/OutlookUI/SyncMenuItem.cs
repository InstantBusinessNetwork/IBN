using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using Mediachase.Sync.Core.Common;

namespace OutlookAddin.OutlookUI
{
	public class SyncMenuItem : VistaMenuItem
	{
		private Timer _animatedTimer;
		private ImageList _imgList;

		public eSyncStatus PrevState { get; set; }
		private int _tickCount = 0; //0.1 sec
		private int _animateDuration = -1; //0.1 sec,  -1 infinite

		private StateMachine<eSyncStatus> _syncStatusSM;
		private Dictionary<eSyncStatus, CycleCollection<int>> _statusImageMaping = new Dictionary<eSyncStatus, CycleCollection<int>>();


		public SyncMenuItem(VistaMenuControl owner, ImageList imgList)
			: base(owner)
		{
			PrevState = eSyncStatus.Unknow;

			_imgList = imgList;

			State<eSyncStatus> unknowState = new State<eSyncStatus>(eSyncStatus.Unknow, ChangeStateHandler);
			State<eSyncStatus> readyState = new State<eSyncStatus>(eSyncStatus.Ready, ChangeStateHandler);
			State<eSyncStatus> canceledState = new State<eSyncStatus>(eSyncStatus.Canceled, ChangeStateHandler);
			State<eSyncStatus> conflicState = new State<eSyncStatus>(eSyncStatus.UnresolvedConflict, ChangeStateHandler);
			State<eSyncStatus> inProgressState = new State<eSyncStatus>(eSyncStatus.InProgress, ChangeStateHandler);
			State<eSyncStatus> failState = new State<eSyncStatus>(eSyncStatus.Failed, ChangeStateHandler);
			State<eSyncStatus> readyProgressState = new State<eSyncStatus>(eSyncStatus.ReadyProgress, ChangeStateHandler);
			State<eSyncStatus> okState = new State<eSyncStatus>(eSyncStatus.Ok, ChangeStateHandler);
			State<eSyncStatus> skippedChangeItemsState = new State<eSyncStatus>(eSyncStatus.SkipedChangesDetected, ChangeStateHandler);

			var allStates = new State<eSyncStatus>[] { okState, readyState, canceledState, conflicState, inProgressState,
													   failState, readyState, unknowState, readyProgressState, skippedChangeItemsState };
			//Пока позаоляем переход из любого состояни в любое
			foreach (State<eSyncStatus> state in allStates)
			{
				state.AvailTransitions.AddRange(allStates);
			}

			_syncStatusSM = new StateMachine<eSyncStatus>(unknowState);
			_syncStatusSM.RegisteredStates.AddRange(allStates);

			_animatedTimer = new Timer();
			_animatedTimer.Interval = 150;
			_animatedTimer.Tick += animatedTimer_Tick;
			_animatedTimer.Enabled = false;
		}

		public eSyncStatus CurrentSyncStatus
		{
			get
			{
				return _syncStatusSM.CurrentState.stateName;
			}
			set
			{
				_syncStatusSM.SetState(value);
			}
		}

		public int AnimateDuration
		{
			get { return _animateDuration; }
			set { _animateDuration = value; }
		}

		public bool AnimateStatusImg
		{
			get { return _animatedTimer.Enabled; }
			set
			{
				_animatedTimer.Enabled = value;
				SetCurrentImage(CurrentSyncStatus);
			}

		}

		private void ChangeStateHandler(State<eSyncStatus> prevState)
		{
			SetCurrentImage(CurrentSyncStatus);
		}

		public void RegisterStatusImages(eSyncStatus status, params int[] imgIndexes)
		{
			CycleCollection<int> imgIndexColl = CycleCollection<int>.CreateInstance(0, imgIndexes);
			_statusImageMaping[status] = imgIndexColl;
		}

		protected virtual void animatedTimer_Tick(object sender, EventArgs e)
		{
			if (AnimateDuration != -1)
			{
				_tickCount++;
				if (_tickCount > AnimateDuration)
				{
					_tickCount = 0;
					AnimateStatusImg = false;
				}
			}

			SetCurrentImage(CurrentSyncStatus);
		}

		private void SetCurrentImage(eSyncStatus status)
		{
			CycleCollection<int> activeStateImgColl;

			if (_statusImageMaping.TryGetValue(status, out activeStateImgColl))
			{
				activeStateImgColl.EnableCycling = _animatedTimer.Enabled;
				IEnumerator<int> cycleIterator = activeStateImgColl.GetEnumerator();
				cycleIterator.MoveNext();

				int index = cycleIterator.Current;
				if (_imgList.Images.Count > index)
				{
					Mediachase.Sync.Core.DebugAssistant.Log("Show animated image " + ((eSyncMenuItem_Icon)index).ToString());
					this.Image = _imgList.Images[index];
				}
			}
		}
	}
}
