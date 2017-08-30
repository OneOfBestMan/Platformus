﻿// Copyright © 2017 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ExtCore.Data.Abstractions;
using Platformus.Globalization.Data.Abstractions;
using Platformus.Globalization.Data.Entities;

namespace Platformus.Globalization
{
  public class DefaultCultureManager : ICultureManager
  {
    private ICache cache;
    private IStorage storage;

    public DefaultCultureManager(ICache cache, IStorage storage)
    {
      this.cache = cache;
      this.storage = storage;
    }

    public Culture GetCulture(int id)
    {
      return this.GetCachedCultures().FirstOrDefault(c => c.Id == id);
    }

    public Culture GetCulture(string code)
    {
      return this.GetCachedCultures().FirstOrDefault(c => string.Equals(c.Code, code, StringComparison.OrdinalIgnoreCase));
    }

    public Culture GetNeutralCulture()
    {
      return this.GetCachedCultures().FirstOrDefault(c => c.IsNeutral);
    }

    public Culture GetDefaultCulture()
    {
      return this.GetCachedCultures().FirstOrDefault(c => c.IsDefault);
    }

    public Culture GetBackendUiCulture()
    {
      return this.GetCachedCultures().FirstOrDefault(c => c.IsBackendUi);
    }

    public Culture GetCurrentCulture()
    {
      Culture currentCulture = this.GetCachedCultures().FirstOrDefault(
        c => string.Equals(c.Code, CultureInfo.CurrentCulture.TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase)
      );

      if (currentCulture == null)
        currentCulture = this.GetCachedCultures().FirstOrDefault(
          c => string.Equals(c.Code, DefaultCulture.Code, StringComparison.OrdinalIgnoreCase)
        );

      return currentCulture;
    }

    public IEnumerable<Culture> GetCultures()
    {
      return this.GetCachedCultures().OrderBy(c => c.Name);
    }

    public IEnumerable<Culture> GetNotNeutralCultures()
    {
      return this.GetCachedCultures().Where(c => !c.IsNeutral).OrderBy(c => c.Name);
    }

    public void InvalidateCache()
    {
      this.cache.Remove("cultures");
    }

    private IEnumerable<Culture> GetCachedCultures()
    {
      return this.cache.GetWithDefaultValue<IEnumerable<Culture>>(
        "cultures", () => this.storage.GetRepository<ICultureRepository>().All(), new CacheEntryOptions(priority: CacheEntryPriority.NeverRemove)
      );
    }
  }
}