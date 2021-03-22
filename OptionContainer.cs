﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiskOfOptions
{
    /// <summary>
    /// Store options from a specific mod in one container.
    /// 
    /// Probably doesn't need to be an object tbh.
    /// </summary>
    internal class OptionContainer
    {
        public string ModGUID { get; private set; }

        internal List<OptionBase> Options;

        private int lastAmount = 0;

        private List<RiskOfOption> modOptions;

        private List<OptionCategory> Categories;

        public OptionContainer(string ModGUID)
        {
            this.ModGUID = ModGUID;

            Options = new List<OptionBase>();
        }

        internal List<RiskOfOption> GetModOptionsCached()
        {
            if (modOptions == null || Options.Count != lastAmount)
            {
                modOptions = Options.Where(a => a.GetType() == typeof(RiskOfOption)).Cast<RiskOfOption>().ToList();

                lastAmount = Options.Count;
            }

            return modOptions;
        }

        internal List<OptionCategory> GetCategoriesCached()
        {
            if (Categories == null || Options.Count != lastAmount)
            {
                Categories = Options.Where(a => a.GetType() == typeof(OptionCategory)).Cast<OptionCategory>().ToList();

                lastAmount = Options.Count;
            }

            return Categories;
        }

        internal void Add(ref OptionBase option)
        {
            Options.Add(option);
        }

        internal void Add(ref RiskOfOption option)
        {
            Options.Add(option);
        }
        internal void Add(ref OptionCategory option)
        {
            Options.Add(option);
        }
    }
}
