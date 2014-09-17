using System;
using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Rendering.GDI
{
    internal class MruFactory<TKey, TObject> : IDisposable where TObject : IDisposable
    {
        private readonly int _maxObjects;
        private readonly Func<TKey, TObject> _createObject;
        private readonly LinkedList<KeyValuePair<TKey, TObject>> _objects;

        public MruFactory(int maxObjects, Func<TKey, TObject> createObject)
        {
            _maxObjects = maxObjects;
            _createObject = createObject;
            _objects = new LinkedList<KeyValuePair<TKey, TObject>>();
        }

        public TObject Create(TKey key)
        {
            var entry = _objects.First;
            while (entry != null && !entry.Value.Key.Equals(key))
                entry = entry.Next;

            if (entry == null)
            {
                var value = _createObject(key);
                entry = _objects.AddFirst(new KeyValuePair<TKey, TObject>(key, value));
            }
            else
            {
                //Put most recently used at the front of the list.
                _objects.Remove(entry);
                _objects.AddFirst(entry.Value);
            }

            Cleanup();
            return entry.Value.Value;
        }

        private void Cleanup()
        {
            while (_objects.Count > _maxObjects)
            {
                var lastEntry = _objects.Last.Value;
                _objects.RemoveLast();
                try
                {
                    lastEntry.Value.Dispose();
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e);
                }
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            foreach (var entry in _objects)
            {
                try
                {
                    entry.Value.Dispose();
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e);
                }
            }

            _objects.Clear();
        }

        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
            }
        }
    }
}
