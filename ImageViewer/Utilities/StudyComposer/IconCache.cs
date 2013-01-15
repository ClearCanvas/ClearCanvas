#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer
{
	internal delegate Image IconCreatorDelegate();

	internal delegate void IconCreatedCallback();

	internal sealed class IconCache
	{
		private static readonly Image _placeHolderImage;
		private static readonly Image _nullImage;
		private readonly Dictionary<string, Image> _images;
		private readonly ProducerConsumerQueue<Task> _loadQueue;
		private readonly Thread _loaderThread;

		static IconCache()
		{
			_placeHolderImage = new Bitmap(64, 64);
			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(_placeHolderImage))
			{
				StringFormat sf = new StringFormat();
				Font font = new Font(FontFamily.GenericSansSerif, 8);
				RectangleF rect = new RectangleF(PointF.Empty, _placeHolderImage.Size);
				sf.Alignment = StringAlignment.Center;
				sf.LineAlignment = StringAlignment.Center;
				g.FillRectangle(Brushes.Black, rect.Left, rect.Top, rect.Width - 1, rect.Height - 1);
				g.DrawEllipse(Pens.White, rect.Left, rect.Top, rect.Width - 1, rect.Height - 1);
				g.DrawString("Loading...", font, Brushes.White, rect, sf);
				font.Dispose();
				sf.Dispose();
			}

			_nullImage = new Bitmap(64, 64);
			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(_nullImage))
			{
				StringFormat sf = new StringFormat();
				Font font = new Font(FontFamily.GenericSansSerif, 8);
				RectangleF rect = new RectangleF(PointF.Empty, _nullImage.Size);
				sf.Alignment = StringAlignment.Center;
				sf.LineAlignment = StringAlignment.Center;
				g.FillRectangle(Brushes.Black, rect.Left, rect.Top, rect.Width - 1, rect.Height - 1);
				g.DrawRectangle(Pens.White, rect.Left, rect.Top, rect.Width - 1, rect.Height - 1);
				g.DrawLine(Pens.Red, rect.Left, rect.Top, rect.Right, rect.Bottom);
				g.DrawLine(Pens.Red, rect.Right, rect.Top, rect.Left, rect.Bottom);
				g.DrawString("No Image", font, Brushes.White, rect, sf);
				font.Dispose();
				sf.Dispose();
			}
		}

		public IconCache()
		{
			_images = new Dictionary<string, Image>();
			_loadQueue = new ProducerConsumerQueue<Task>();
			_loaderThread = new Thread(new ThreadStart(Dequeue));
			_loaderThread.IsBackground = true;
			_loaderThread.Priority = ThreadPriority.BelowNormal;
			_loaderThread.Start();
		}

		public void Clear()
		{
			lock (_images)
			{
				_loadQueue.Clear();
				_images.Clear();
			}
		}

		public Image this[string key]
		{
			get
			{
				lock (_images)
				{
					if (_images.ContainsKey(key))
					{
						return _images[key];
					}
					else
					{
						return _nullImage;
					}
				}
			}
			private set
			{
				lock (_images)
				{
					if (_images.ContainsKey(key))
					{
						if (_images[key] != value)
						{
							if (value == null)
								value = _nullImage;
							_images[key] = value;
						}
					}
					else
					{
						_images.Add(key, value);
					}
				}
			}
		}

		public void LoadIcon(string key, Image icon)
		{
			this[key] = icon;
		}

		public void LoadIcon(string key, IconCreatorDelegate creatorDelegate, IconCreatedCallback onCreated)
		{
			this[key] = _placeHolderImage;
			_loadQueue.Produce(new Task(key, creatorDelegate, onCreated));
		}

		private void Dequeue()
		{
			Task task;
			while ((task = _loadQueue.Consume()) != null)
			{
				this[task.Key] = task.CreateImage();

				// TODO Remove this sleep
				//Thread.Sleep(500);

				task.NotifyCreated();
			}
		}

		private class Task
		{
			private IconCreatorDelegate _creatorDelegate;
			private IconCreatedCallback _onCreated;
			private string _key;

			public Task(string key, IconCreatorDelegate creatorDelegate, IconCreatedCallback onCreated)
			{
				_key = key;
				_creatorDelegate = creatorDelegate;
				_onCreated = onCreated;
			}

			public string Key
			{
				get { return _key; }
			}

			public Image CreateImage()
			{
				try
				{
					return _creatorDelegate();
				}
				catch (Exception)
				{
					return null;
				}
			}

			public void NotifyCreated()
			{
				_onCreated();
			}
		}

		/// <summary>
		/// A producer/consumer queue pattern class from http://www.yoda.arachsys.com/csharp/threads/deadlocks.shtml
		/// </summary>
		/// <typeparam name="T"></typeparam>
		private class ProducerConsumerQueue<T>
		{
			private readonly object listLock = new object();
			private readonly Queue<T> queue = new Queue<T>();

			public void Produce(T item)
			{
				lock (listLock)
				{
					queue.Enqueue(item);

					// We always need to pulse, even if the queue wasn't
					// empty before. Otherwise, if we add several items
					// in quick succession, we may only pulse once, waking
					// a single thread up, even if there are multiple threads
					// waiting for items.            
					Monitor.Pulse(listLock);
				}
			}
			
			public void Clear()
			{
				lock (listLock)
				{
					queue.Clear();
					Monitor.Pulse(listLock);
				}
			}

			public T Consume()
			{
				lock (listLock)
				{
					// If the queue is empty, wait for an item to be added
					// Note that this is a while loop, as we may be pulsed
					// but not wake up before another thread has come in and
					// consumed the newly added object. In that case, we'll
					// have to wait for another pulse.
					while (queue.Count == 0)
					{
						// This releases listLock, only reacquiring it
						// after being woken up by a call to Pulse
						Monitor.Wait(listLock);
					}
					return queue.Dequeue();
				}
			}
		}
	}
}