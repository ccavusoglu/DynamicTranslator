﻿namespace DynamicTranslator.Core.Orchestrators.Organizer
{
    #region using

    using System.Collections.Generic;

    #endregion

    public interface IMeanOrganizerFactory
    {
        ICollection<IMeanOrganizer> GetMeanOrganizers();
    }
}