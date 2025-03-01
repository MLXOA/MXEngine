using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MXEngine.Graphics.AppSupport
{
    /// <summary>
    /// Communication for the OpenGL graphical features.
    /// </summary>
    public interface IGraphics
    {
        /// <summary>
        /// Returns the GL instance if supported.
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns>GL instance.</returns>
        public GL GetGlInterface();
    }
}
