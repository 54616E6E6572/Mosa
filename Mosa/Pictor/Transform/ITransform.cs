/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Simon Wollwage (<mailto:rootnode@mosa-project.org>)
 */
using Pictor.Transform;

namespace Pictor.Transform
{
    public interface ITransform
    {
        void Transform(ref double x, ref double y);
    };
}