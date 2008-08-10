/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Michael Ruck (<mailto:sharpos@michaelruck.de>)
 */

using System;
using System.Collections.Generic;
using System.Text;
using Mosa.Runtime.Metadata;

namespace Mosa.Runtime.Vm
{
    public sealed class ReadOnlyRuntimeParameterListView :
        ReadOnlyRuntimeListView<RuntimeParameter>
    {
        #region Static members

        /// <summary>
        /// Provides an empty list definition.
        /// </summary>
        public static readonly ReadOnlyRuntimeParameterListView Empty = new ReadOnlyRuntimeParameterListView();

        #endregion // Static members

        #region Construction

        private ReadOnlyRuntimeParameterListView()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ReadOnlyRuntimeFieldListView"/>.
        /// </summary>
        /// <param name="firstIndex">The first index of the list view.</param>
        /// <param name="count">The number of elements in the list view.</param>
        public ReadOnlyRuntimeParameterListView(int firstIndex, int count)
            : base(firstIndex, count)
        {
        }

        #endregion // Construction

        #region Overrides

        /// <summary>
        /// Returns the fields array, which is viewed by this collection.
        /// </summary>
        protected override RuntimeParameter[] Items
        {
            get { return RuntimeBase.Instance.TypeLoader.Parameters; }
        }

        #endregion // Overrides
    }
}
