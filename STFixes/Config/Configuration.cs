/*
    =============================================================================
    CS#Fixes
    Copyright (C) 2023-2024 Charles Barone <CharlesBarone> / hypnos <hyps.dev>
    =============================================================================

    This program is free software; you can redistribute it and/or modify it under
    the terms of the GNU General Public License, version 3.0, as published by the
    Free Software Foundation.

    This program is distributed in the hope that it will be useful, but WITHOUT
    ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
    FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more
    details.

    You should have received a copy of the GNU General Public License along with
    this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.ComponentModel;
using System.Runtime.CompilerServices;
using STFixes.Managers;
using Microsoft.Extensions.Logging;

namespace STFixes.Config
{
    public class Configuration(ILogger<STFixes> logger, FixManager fixManager) : INotifyPropertyChanged
    {
        private bool enableWaterFix = true;
        private bool enableBotNavIgnoreFix = true;
        private bool enableTriggerPushFix = true;
        private bool disableSubTickMovement = true;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if(propertyName == null) return;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            OnConfigChanged(propertyName, GetType().GetProperty(propertyName)?.GetValue(this, null));
        }

        private void OnConfigChanged(string propertyName, object? newValue)
        {
            logger.LogInformation($"[STFixes][Configuration][OnConfigChanged()][Property={propertyName}][Value={newValue}]");
            fixManager.OnConfigChanged(propertyName, newValue);
        }
        
        public void Start()
        {
            logger.LogInformation("[STFixes][Configuration][Start()]");
            
            // Call OnConfigChanged for each property to apply the initial configuration & trigger the fix manager
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(this))
            {
                OnConfigChanged(property.Name, property.GetValue(this));
            }
        }

        public bool EnableWaterFix
        {
            get => enableWaterFix;
            set
            {
                if (enableWaterFix != value)
                {
                    enableWaterFix = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool EnableBotNavIgnoreFix
        {
            get => enableBotNavIgnoreFix;
            set
            {
                if (enableBotNavIgnoreFix != value)
                {
                    enableBotNavIgnoreFix = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool EnableTriggerPushFix
        {
            get => enableTriggerPushFix;
            set
            {
                if (enableTriggerPushFix != value)
                {
                    enableTriggerPushFix = value;
                    OnPropertyChanged();
                }
            }
        }

        // Commented out since it seems to cause crashes every time I test it...
        /* public bool EnableNavmeshLookupLagFix
        {
            get => enableNavmeshLookupLagFix;
            set
            {
                if (enableNavmeshLookupLagFix != value)
                {
                    enableNavmeshLookupLagFix = value;
                    OnPropertyChanged();
                }
            }
        } */

        public bool DisableSubTickMovement
        {
            get => disableSubTickMovement;
            set
            {
                if (disableSubTickMovement != value)
                {
                    disableSubTickMovement = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
