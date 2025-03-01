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
    interface IGraphics
    {
        /// <summary>
        /// Returns the GL instance if supported.
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns>GL instance.</returns>
        GL GetGLInterface()
        {
            throw new NotSupportedException("This application does not support OpenGL or hasn't implemented IGraphics.GetGLInterface(), this method can be implemented for lower-level graphics functionality.");
        }

        /// <summary>
        /// Gets the framebuffer ready for drawing, optionally clearing it when ready.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        void BeginDraw(bool clearBuffer = false)
        {
            throw new NotImplementedException("NOT IMPLEMENTED: IGraphics.BeginDraw()+IGraphics.GetGLInterface()\nCategory-Importance: BaseGraphics-HIGH+LOW");
        }

        void EndDraw()
        {
            throw new NotImplementedException("NOT IMPLEMENTED: IGraphics.EndDraw()\nCategory-Importance: BaseGraphics-HIGH");
        }
    }
}
