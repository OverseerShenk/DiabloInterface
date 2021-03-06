﻿namespace Zutatensuppe.DiabloInterface.Business.Services
{
    using System;

    using Zutatensuppe.D2Reader;
    using Zutatensuppe.D2Reader.Models;

    public interface IGameService
    {
        /// <summary>
        ///     The targeted (intended) difficulty for a run selected by the player.
        /// </summary>
        GameDifficulty TargetDifficulty { get; set; }

        /// <summary>
        ///     Occurs when a new character is created at level 1.
        /// </summary>
        event EventHandler<CharacterCreatedEventArgs> CharacterCreated;

        /// <summary>
        ///     Occurs when data has been successfully read from the game.
        /// </summary>
        event EventHandler<DataReadEventArgs> DataRead;

        /// <summary>
        ///     Gets the data reader associated with the game service.
        /// </summary>
        D2DataReader DataReader { get; }
    }
}
