#region License
// Copyright (c) Jeremy Skinner (http://www.jeremyskinner.co.uk)
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at http://fluentvalidation.codeplex.com
#endregion

namespace NServiceKit.FluentValidation.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>Collection of trackings.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public class TrackingCollection<T> : IEnumerable<T>
    {
        readonly List<T> innerCollection = new List<T>();
        /// <summary>Occurs when Item Added.</summary>
        public event Action<T> ItemAdded;

        /// <summary>Adds item.</summary>
        ///
        /// <param name="item">The item to add.</param>
        public void Add(T item) {
            innerCollection.Add(item);

            if (ItemAdded != null) {
                ItemAdded(item);
            }
        }

        /// <summary>Executes the item added action.</summary>
        ///
        /// <param name="onItemAdded">The on item added.</param>
        ///
        /// <returns>An IDisposable.</returns>
        public IDisposable OnItemAdded(Action<T> onItemAdded) {
            ItemAdded += onItemAdded;
            return new EventDisposable(this, onItemAdded);
        }

        /// <summary>Gets the enumerator.</summary>
        ///
        /// <returns>The enumerator.</returns>
        public IEnumerator<T> GetEnumerator() {
            return innerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        class EventDisposable : IDisposable {
            readonly TrackingCollection<T> parent;
            readonly Action<T> handler;

            /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.Internal.TrackingCollection&lt;T&gt;.EventDisposable class.</summary>
            ///
            /// <param name="parent"> The parent.</param>
            /// <param name="handler">The handler.</param>
            public EventDisposable(TrackingCollection<T> parent, Action<T> handler) {
                this.parent = parent;
                this.handler = handler;
            }

            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            public void Dispose() {
                parent.ItemAdded -= handler;
            }
        }
    }
}