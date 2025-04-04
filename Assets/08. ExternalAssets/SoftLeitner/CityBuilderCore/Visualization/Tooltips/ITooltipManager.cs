﻿namespace CityBuilderCore
{
    /// <summary>
    /// manages displaying tooltips in the UI
    /// </summary>
    public interface ITooltipManager
    {
        /// <summary>
        /// the pointer has entered the owners area
        /// </summary>
        /// <param name="owner">an object that has a tooltip</param>
        void Enter(ITooltipOwner owner);
        /// <summary>
        /// the pointer has exited the owners area
        /// </summary>
        /// <param name="owner">an object that has a tooltip</param>
        void Exit(ITooltipOwner owner);

        /// <summary>
        /// text of the tooltip owner has changed<br/>
        /// if it is active it needs to be refreshed in the ui
        /// </summary>
        /// <param name="owner">an object that has a tooltip</param>
        void Refresh(ITooltipOwner owner);
    }
}
