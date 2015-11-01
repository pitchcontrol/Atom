using System;

namespace Atom.Behavior
{
    interface IDropable
    {
        /// Drop data into the collection.
        /// </summary>
        /// <param name="data">The data to be dropped</param>
        /// <param name="index">optional: The index location to insert the data</param>
        void Drop(WebPageBaseViewModel data, int index = -1);
    }
}
