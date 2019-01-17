using RAGE;
using RAGE.Game;
using RageMpClientHelpers;
using RageMpClientShared;
using System;
using System.Collections.Generic;

namespace ScreenFx
{
    public class ScreenFxTester : Events.Script
    {
        private bool _on;
        private int _index = 0;
        private string[] _effects;
        private string _currEffect;

        public ScreenFxTester()
        {
            //Stop any running
            RAGE.Game.Graphics.StopAllScreenEffects();

            //Get as array so can cycle through them
            _effects = Enum.GetNames(typeof(ScreenEffect));            

            Events.Tick += OnTick;
            _on = false;
        }

        private void OnTick(List<Events.TickNametagData> nametags)
        {
            if (Pad.IsControlJustPressed(0, (int)Control.ReplayStartStopRecordingSecondary))
            {                
                if (!_on)
                {
                    _on = true;
                    GfxHelper.ScreenFxStart(_currEffect);
                }
                else
                {
                    GfxHelper.ScreenFxStop(_currEffect);                    
                    _on = false;
                }

                UiHelper.ShowSubtitle($"ScreenFX On:{_on}", 1000);
            }

            if (Pad.IsControlJustPressed(0, (int)Control.PhoneLeft))
            {
                _index--;
                UpdateIndex();
            }                
            else if (Pad.IsControlJustPressed(0, (int)Control.PhoneRight))
            {
                _index++;
                UpdateIndex();
            }
        }

        private void UpdateIndex()
        {
            if (_index > _effects.Length - 1)
                _index = 0;
            else if (_index < 0)
                _index = _effects.Length - 1;

            _currEffect = _effects[_index];

            if (_on)
                GfxHelper.ScreenFxStart(_currEffect);

            UiHelper.ShowSubtitle($"ScreenFX Effect:{_currEffect}", 1000);
        }
    }
}
