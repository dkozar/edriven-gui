/*
 
eDriven.Gui
Copyright (c) 2010-2014 Danko Kozar
 
*/

using System;
using eDriven.Gui.Components;

namespace eDriven.Gui.Plugins
{
    /// <summary>
    /// Plugin interface
    /// </summary>
    public interface IPlugin : IDisposable
    {
        /// <summary>
        /// Initializes the plugin
        /// </summary>
        /// <param name="component"></param>
        void Initialize(InvalidationManagerClient component);
    }
}