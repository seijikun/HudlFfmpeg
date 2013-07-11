﻿using System;
using System.Linq;
using System.Collections.Generic;
using Hudl.Ffmpeg.Command;
using Hudl.Ffmpeg.Resources.BaseTypes;

namespace Hudl.Ffmpeg.Filters.BaseTypes
{
    /// <summary>
    /// The base implementation of Filter's, will store the basic information on setup, and makes it accessible to parents.
    /// </summary>
    public abstract class BaseFilter : IFilter
    {
        protected BaseFilter(string type, int maxInputs)
        {
            Type = type;
            MaxInputs = maxInputs;
            CommandResources = new List<CommandResource<IResource>>();
        }
    
        /// <summary>
        /// Defines the filter type, name that is given to ffmpeg
        /// </summary>
        public string Type { get; protected set; }

        /// <summary>
        /// Defines the maximum number of allowable inputs to the filter
        /// </summary>
        public int MaxInputs { get; protected set; }

        /// <summary>
        /// Available at [Render] time, brings the resources as available objects to the filters
        /// </summary>
        protected List<CommandResource<IResource>> CommandResources { get; set; }

        /// <summary>
        /// Method, called during [Render] to bring forward all the necessary resources, necessary action for maximum abstraction from the user.
        /// </summary>
        /// <param name="command">The command chain the current filter belongs to.</param>
        /// <param name="filterchain">The filterchain that the filter belongs to</param>
        public void Setup(Command<IResource> command, Filterchain<IResource> filterchain) 
        {
            CommandResources = command.ResourcesFromReceipts(new List<CommandResourceReceipt>(filterchain.Resources));

            if (CommandResources.Count == 0)
            {
                throw new InvalidOperationException("Cannot setup filter with a resource count of zero.");
            }
            if (CommandResources.Count > MaxInputs)
            {
                throw new InvalidOperationException("The filter has exceeded the maximum allowed number of inputs.");
            }
        }

        /// <summary>
        /// Quick way to calculate the output length after a filter has been applied.
        /// </summary>
        public virtual TimeSpan? LengthFromInputs(List<CommandResource<IResource>> resources)
        {
            var totalSeconds = resources.Sum(r => r.Resource.Length.TotalSeconds);
            return totalSeconds > 0d
                       ? (TimeSpan?)TimeSpan.FromSeconds(totalSeconds)
                       : null;
        }
    }
}
