﻿/************************************************************************
   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at https://opensource.org/licenses/MS-PL
 ************************************************************************/

using System;
using System.Windows.Threading;
using AvalonDock.Layout;

namespace AvalonDock.Controls
{
	internal class AutoHideWindowManager
	{
		#region Members

		private DockingManager _manager;
		private WeakReference _currentAutohiddenAnchor = null;
		private DispatcherTimer _closeTimer = null;

		#endregion

		#region Constructors

		internal AutoHideWindowManager(DockingManager manager)
		{
			_manager = manager;
			this.SetupCloseTimer();
		}

		#endregion

		#region Private Methods

		public void ShowAutoHideWindow(LayoutAnchorControl anchor)
		{
			if (_currentAutohiddenAnchor.GetValueOrDefault<LayoutAnchorControl>() != anchor)
			{
				StopCloseTimer();
				_currentAutohiddenAnchor = new WeakReference(anchor);
				_manager.AutoHideWindow.Show(anchor);
				StartCloseTimer();
			}
		}

		public void HideAutoWindow(LayoutAnchorControl anchor = null)
		{
			if (anchor == null ||
				anchor == _currentAutohiddenAnchor.GetValueOrDefault<LayoutAnchorControl>())
			{
				StopCloseTimer();
			}
			else
				System.Diagnostics.Debug.Assert(false);
		}

		private void SetupCloseTimer()
		{
			_closeTimer = new DispatcherTimer(DispatcherPriority.Background);
			_closeTimer.Interval = TimeSpan.FromMilliseconds(_manager.AutoHideDelay);
			_closeTimer.Tick += (s, e) =>
			{
				if (_manager.AutoHideWindow.IsWin32MouseOver ||
					((LayoutAnchorable)_manager.AutoHideWindow.Model).IsActive ||
					_manager.AutoHideWindow.IsResizing)
					return;

				StopCloseTimer();
			};
		}

		private void StartCloseTimer()
		{
			_closeTimer.Start();
		}

		private void StopCloseTimer()
		{
			_closeTimer.Stop();
			_manager.AutoHideWindow.Hide();
			_currentAutohiddenAnchor = null;
		}

		#endregion
	}
}
