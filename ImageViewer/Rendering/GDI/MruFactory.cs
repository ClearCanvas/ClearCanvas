using System;
using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Rendering.GDI
{
    internal class MruFactory<TKey, TObject> : IDisposable where TObject : IDisposable
    {
        private readonly int _maxObjects;
        private readonly Func<TKey, TObject> _createObject;

        private readonly Dictionary<TKey, TObject> _objects = new Dictionary<TKey, TObject>();
        private readonly LinkedList<TKey> _mruKeys = new LinkedList<TKey>();
        private KeyValuePair<TKey, TObject>? _last;

        public MruFactory(int maxObjects, Func<TKey, TObject> createObject)
        {
            _maxObjects = maxObjects;
            _createObject = createObject;
        }

        public TObject Create(TKey key)
        {
            if (_last.HasValue && _last.Value.Key.Equals(key))
                return _last.Value.Value;

            TObject obj;
            if (!_objects.TryGetValue(key, out obj))
            {
                obj = _createObject(key);
                _objects[key] = obj;
            }
            else
            {
                _mruKeys.Remove(key);
            }

            _mruKeys.AddFirst(key);
            _last = new KeyValuePair<TKey, TObject>(key, obj);
            Cleanup();
            return obj;
        }

        private void Cleanup()
        {
            if (_mruKeys.Count <= _maxObjects)
                return;

            _last = null;

            do
            {
                var lastKey = _mruKeys.Last.Value;
                TObject last = _objects[lastKey];
                _mruKeys.RemoveLast();
                _objects.Remove(lastKey);

                try
                {
                    last.Dispose();
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e);
                }
            } while (_mruKeys.Count > _maxObjects);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            foreach (var obj in _objects.Values)
            {
                try
                {
                    obj.Dispose();
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e);
                }
            }
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
